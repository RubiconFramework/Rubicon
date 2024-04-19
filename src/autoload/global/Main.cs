global using GameplayScene = Rubicon.gameplay.GameplayScene;
global using Conductor = Rubicon.autoload.global.Conductor;
global using Main = Rubicon.autoload.global.Main;
global using Godot;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DiscordRPC;
using DiscordRPC.Logging;
using Godot.Sharp.Extras;
using Newtonsoft.Json;
using Rubicon.autoload.global.elements;
using Rubicon.gameplay.objects.resources;
using Rubicon.scenes.options.elements;
using FileAccess = Godot.FileAccess;

namespace Rubicon.autoload.global;

[Icon("res://assets/miscicons/autoload.png")]
public partial class Main : CanvasLayer
{
	[Export] public string DiscordRpcClientID = "1218405526677749760";
	[Export] public string SettingsFilePath = "user://settings.json";
	[Export] private Color InfoNotificationColor { get; set; } = new(0.32f,0.32f, 0.32f);
	[Export] private Color WarningNotificationColor { get; set; } = new(0.79f,0.79f, 0);
	[Export] private Color ErrorNotificationColor { get; set; } = new(0.70f, 0f, 0f);

	public static Chart Song; //i still have no clue why we need this here -dunine
	public static Main Instance { get; private set; } = new();
	
 	public static readonly string[] AudioFormats = { "mp3", "ogg", "wav" , "flac" };
	public static readonly Vector2 WindowSize = new((float)ProjectSettings.GetSetting("display/window/size/viewport_width"), (float)ProjectSettings.GetSetting("display/window/size/viewport_height"));

	public static RubiconSettings GameSettings { get; set; } = new();
	public static DiscordRpcClient DiscordRpcClient = new(Instance.DiscordRpcClientID);
	
	[NodePath("Notification")] private Panel NotificationInstance;
	private Queue<(Panel, double)> notificationQueue = new();
	private readonly Dictionary<Panel, float> notificationPositions = new();
	private float YOffset;

	public override void _Ready()
	{
		this.OnReady();
		Instance = this;
		RenderingServer.SetDefaultClearColor(new(0,0,0));
		TranslationServer.SetLocale(GameSettings.Misc.Languages.ToString().ToLower());
        
		if ((bool)ProjectSettings.GetSetting("use_project_name_user_dir",true)){
			var dir = ProjectSettings.GetSetting("application/config/custom_user_dir_name", "Rubicon/Engine").ToString();
			var projectName = ProjectSettings.GetSetting("application/config/name", "Rubicon").ToString();

			if (dir == "Rubicon/Engine" && projectName != "Rubicon")
			{
				Alert("New project name has been found. Reload project.godot for it to apply.");
				ProjectSettings.SetSetting("application/config/custom_user_dir_name", $"Rubicon/{projectName}");
				ProjectSettings.Save();
			}
			else
			{
				if (dir != "Rubicon/Engine" && projectName == "Rubicon")
				{
					Alert("Base engine detected. Reload project.godot for it to apply.");
					ProjectSettings.SetSetting("application/config/custom_user_dir_name", "Rubicon/Engine");
					ProjectSettings.Save();
				}
				else Alert($"Data stored at: user://{dir}");
			}
		}
		
		DiscordRPC(true);
		LoadSettings(SettingsFilePath);
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		GameSettings = null;
		DiscordRPC(false);
	}

    public override void _Process(double delta)
    {
        for (int i = 0; i < notificationQueue.Count; i++)
        {
            var (panel, duration) = notificationQueue.Dequeue();
            duration -= delta;

            var progressBar = panel.GetNode<ProgressBar>("DurationBar");
            var animationPlayer = panel.GetNode<AnimationPlayer>("animalationtolongplayer");

            progressBar.Value = (float)duration;

            if (Mathf.Abs(duration - 0.5f) < 0.01f)
                animationPlayer.Play("out");

            if (duration <= 0) OnNotificationTimeout(panel);
            else notificationQueue.Enqueue((panel, duration));
            
            void OnNotificationTimeout(Panel p)
            {
	            notificationQueue = new(notificationQueue.Where(item => item.Item1 != p));
	            p.QueueFree();
	            notificationPositions.Remove(p);
	            UpdateNotificationPositions();
            }
        }
    }

    public void Alert(string message, bool printToConsole = true, NotificationType type = NotificationType.Info, float duration = 5.0f)
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
                    if (printToConsole) GD.Print(fullMessage);
                    progressBar.AddThemeColorOverride("bg_color", InfoNotificationColor);
                    break;
                case NotificationType.Warning:
	                if (printToConsole)
	                {
		                GD.PushWarning(fullMessage);
		                GD.PrintRich($"[color={WarningNotificationColor}][pulse]{fullMessage}[/pulse][/color]");
	                }
                    progressBar.AddThemeColorOverride("bg_color", WarningNotificationColor);
                    break;
                case NotificationType.Error:
	                if (printToConsole)
	                {
		                GD.PushError(fullMessage);
		                GD.PrintRich($"[color={ErrorNotificationColor}][pulse]{fullMessage}[/pulse][/color]");
	                }
                    progressBar.AddThemeColorOverride("bg_color", ErrorNotificationColor);
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

	public static void LoadSettings(string path)
	{
		try
		{
			RubiconSettings rubiconSettings = new();
			if (FileAccess.FileExists(path))
			{
				var jsonData = FileAccess.Open(path, FileAccess.ModeFlags.Read);
				string json = jsonData.GetAsText();

				if (!string.IsNullOrEmpty(json))
				{
					rubiconSettings = JsonConvert.DeserializeObject<RubiconSettings>(json);
					if (rubiconSettings != null)
					{
						GameSettings = rubiconSettings;
						GD.Print($"Settings loaded from file. [{path}]");
					}
				}
			}
			else
			{
				Instance.Alert("Settings file not found. Writing default settings to file.");
				rubiconSettings.GetDefaultSettings().Save();
				GameSettings = rubiconSettings;
			}
		}
		catch (Exception e)
		{
			GD.PrintErr($"Failed to load or write default settings: {e.Message}");
			throw;
		}
	}

	public void DiscordRPC(bool enable)
	{
		try
		{
			if (enable)
			{
				if (DiscordRpcClient.IsDisposed) DiscordRpcClient = new(DiscordRpcClientID);
				
				if (!DiscordRpcClient.IsInitialized)
				{
					DiscordRpcClient.Logger = new ConsoleLogger() { Level = LogLevel.Warning };
					DiscordRpcClient.OnReady += (_, e) => GD.Print($"Discord RPC: Received Ready from user: {e.User.Username}");
					DiscordRpcClient.Initialize();
				}
				
				DiscordRpcClient.SetPresence(new()
				{
					Details = GetTree().CurrentScene?.Name ?? "Unknown Scene",
					Assets = new()
					{
						LargeImageKey = "image_large",
						LargeImageText = $"Framework Version {ProjectSettings.Singleton.GetSetting("application/config/version", "1.0").ToString()} {(OS.IsDebugBuild() ? "[Debug]" : "[Release]")}",
					}
				});
			}
			else
			{
				if (DiscordRpcClient.IsInitialized)
				{
					DiscordRpcClient.ClearPresence();
					DiscordRpcClient.Dispose();
				}
			}
		}
		catch (Exception ex)
		{
			GD.PrintErr($"Error {(enable ? "initializing" : "disabling")} Discord RPC: {ex.Message}");
		}
	}
}
