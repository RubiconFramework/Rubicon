global using GameplayScene = Rubicon.scenes.gameplay.GameplayScene;
global using Conductor = Rubicon.autoload.Conductor;
global using Godot; 
global using System;
global using Godot.Sharp.Extras;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Rubicon.autoload.enums;
using Rubicon.backend.notification;
using AudioManager = Rubicon.autoload.AudioManager;
using Chart = Rubicon.scenes.gameplay.objects.classes.Chart;
using DiscordRichPresence = Rubicon.autoload.DiscordRichPresence;

namespace Rubicon;

[Icon("res://assets/miscicons/autoload.png")]
public partial class Main : CanvasLayer
{
	[Export] private Color InfoNotificationColor { get; set; } = new(1f,1f, 1f);
	[Export] private Color WarningNotificationColor { get; set; } = new(0.79f,0.79f, 0);
	[Export] private Color ErrorNotificationColor { get; set; } = new(0.70f, 0f, 0f);
	[Export] public PackedScene alphabetScene;

	public static Chart Song; //i still have no clue why we need this here -dunine
	public static Main Instance { get; private set; } = new();
	
 	public static readonly string[] AudioFormats = { "mp3", "ogg", "wav" , "flac" };
    public static readonly string RubiconVersion = ProjectSettings.Singleton.GetSetting("application/config/version", "1").ToString();
    public static readonly Vector2 WindowSize = new((float)ProjectSettings.GetSetting("display/window/size/viewport_width"), (float)ProjectSettings.GetSetting("display/window/size/viewport_height"));

    public const string SettingsFilePath = "user://settings.json";
    public static RubiconSettings RubiconSettings { get; set; } = new();
	
	[NodePath("Notification")] private Panel NotificationInstance;
	private Queue<(Panel, double)> notificationQueue = new();
	private readonly Dictionary<Panel, float> notificationPositions = new();
	private float YOffset;

	public override void _Ready()
	{
		this.OnReady();
		Instance = this;
		AudioManager.Instance.PlayAudio(AudioType.Music, "rubiconMenu", 0.5f, true);
		
		RubiconSettings.Load(SettingsFilePath);
		RenderingServer.SetDefaultClearColor(new(0,0,0));
		TranslationServer.SetLocale(RubiconSettings.Misc.Languages.ToString().ToLower());

		if ((bool)ProjectSettings.GetSetting("use_project_name_user_dir",true)){
			var dir = ProjectSettings.GetSetting("application/config/custom_user_dir_name", "Rubicon/Engine").ToString();
			var projectName = ProjectSettings.GetSetting("application/config/name", "Rubicon").ToString();

			if (dir == "Rubicon/Engine" && projectName != "Rubicon")
			{
				SendNotification("New project name has been found. Reload project.godot for it to apply.");
				ProjectSettings.SetSetting("application/config/custom_user_dir_name", $"Rubicon/{projectName}");
				ProjectSettings.Save();
			}
			else
			{
				if (dir != "Rubicon/Engine" && projectName == "Rubicon")
				{
					SendNotification("Base engine detected. Reload project.godot for it to apply.");
					ProjectSettings.SetSetting("application/config/custom_user_dir_name", "Rubicon/Engine");
					ProjectSettings.Save();
				}
				else SendNotification($"Data stored at: user://{dir}");
			}
		}

		DiscordRichPresence.Instance.Toggle(true);
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		RubiconSettings = null;
		DiscordRichPresence.Instance.Toggle(true);
	}

    public override void _Process(double delta)
    {
        for (int i = 0; i < notificationQueue.Count; i++)
        {
            var (panel, duration) = notificationQueue.Dequeue();
            duration -= delta;
            
            panel.GetNode<ProgressBar>("DurationBar").Value = (float)duration;
            if (Mathf.Abs(duration - 0.5f) < 0.01f) panel.GetNode<AnimationPlayer>("animalationtolongplayer").Play("out");
            
            if (duration <= 0) OnNotificationTimeout(panel);
            else notificationQueue.Enqueue((panel, duration));
        }
    }
    
    private void OnNotificationTimeout(Panel p)
    {
	    notificationQueue = new(notificationQueue.Where(item => item.Item1 != p));
	    p.QueueFree();
	    notificationPositions.Remove(p);
	    UpdateNotificationPositions();
    }
    
    public Node CreateAlphabet(Node parentNode, string text, bool bold, bool isMenuItem, int targetY)
    {
	    PackedScene scene = GD.Load<PackedScene>(alphabetScene.ResourcePath);
	    Node alphabetNode = scene.Instantiate();
	    alphabetNode.Set("text", text);
	    alphabetNode.Set("bold", bold);
	    alphabetNode.Set("is_menu_item", isMenuItem);
	    alphabetNode.Set("target_y", targetY);
	    parentNode.AddChild(alphabetNode);
	    return alphabetNode;
    }

    public void SendNotification(string message, bool printToConsole = true, NotificationType type = NotificationType.Info, float duration = 5.0f)
    {
        StackTrace stackTrace = new();
        StackFrame stackFrame = stackTrace.GetFrame(1);
        string fullMessage = $"[{type.ToString().ToUpper()} - {stackFrame!.GetMethod()?.Name}] -> {message}";
        if (NotificationInstance.Duplicate() is Panel notificationInstance)
        {
            float yPosition = 32;
            if (notificationPositions.Count > 0) yPosition = notificationPositions.Values.Max() + 10 + notificationInstance.GetRect().Size.Y;

            notificationInstance.Visible = true;
            notificationInstance.Position = new(notificationInstance.GetRect().Position.X, yPosition);
            notificationPositions.Add(notificationInstance, yPosition);

            var progressBar = notificationInstance.GetNode<ProgressBar>("DurationBar");
            var messageLabel = notificationInstance.GetNode<Label>("Message");
            var animationPlayer = notificationInstance.GetNode<AnimationPlayer>("animalationtolongplayer");

            switch (type)
            {
                case NotificationType.Info:
                    if (printToConsole) GD.PrintRich($"[color=WHITE]{fullMessage}[/color]");
                    progressBar.Modulate = InfoNotificationColor;
                    break;
                case NotificationType.Warning:
	                if (printToConsole)
	                {
		                GD.PushWarning(fullMessage);
		                GD.PrintRich($"[color=YELLOW][pulse]{fullMessage}[/pulse][/color]");
	                }
	                progressBar.Modulate = WarningNotificationColor;
                    break;
                case NotificationType.Error:
	                if (printToConsole)
	                {
		                GD.PushError(fullMessage);
		                GD.PrintRich($"[color=RED][pulse]{fullMessage}[/pulse][/color]");
	                }
	                progressBar.Modulate = ErrorNotificationColor;
                    break;
            }

            messageLabel.Text = fullMessage;
            progressBar.MaxValue = duration;
            progressBar.Value = duration;
            animationPlayer.Play("in");
            UpdateNotificationPositions();

            messageLabel.MinimumSizeChanged += MessageLabelOnMinimumSizeChanged;
            void MessageLabelOnMinimumSizeChanged()
            {
                messageLabel.MinimumSizeChanged -= MessageLabelOnMinimumSizeChanged;
                notificationInstance.Size = new(messageLabel.Size.X + 20, messageLabel.Size.Y);
                progressBar.Size = new(messageLabel.Size.X + 20, progressBar.Size.Y);
                messageLabel.Position = new((notificationInstance.Size.X - messageLabel.Size.X) / 2, (notificationInstance.Size.Y - messageLabel.Size.Y) / 2);
            }

            notificationQueue.Enqueue((notificationInstance, duration));
            Instance.AddChild(notificationInstance);
        }
    }

    private void UpdateNotificationPositions()
    {
        float yOffset = 32;
        foreach (var kvp in notificationPositions.OrderBy(kvp => kvp.Value))
        {
            var (panel, _) = kvp;
            panel.Position = new Vector2(panel.Position.X, yOffset);
            yOffset += panel.GetRect().Size.Y + 10;
        }
    }
}
