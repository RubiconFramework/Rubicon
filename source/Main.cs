global using Godot;
global using System;
global using Godot.Sharp.Extras;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Rubicon.Backend.Autoload;

namespace Rubicon;

public enum NotificationType
{
    Info,
    Warning,
    Error
}

public partial class Main : Node
{
    /*   The average global class that has utilities and sometimes holds useful variables.	*/

    public static readonly string RubiconVersion = ProjectSettings.Singleton.GetSetting("application/config/version", "1").ToString();
    public static Vector2 WindowSize { get; set; } = new Vector2((float)ProjectSettings.GetSetting("display/window/size/viewport_width"), (float)ProjectSettings.GetSetting("display/window/size/viewport_height"));
    public static string[] AudioFileTypes = { ".ogg", ".mp3", ".wav", ".flac" };
    
    public override void _Ready()
    {
        RubiconSettings.Instance.Load("user://settings.json");
        RenderingServer.SetDefaultClearColor(new Color(0, 0, 0, 1)); //godot should have an editor-only background override or something this shits annoying
    }

    // i know this is in FlashImporter too
    // its done this way so the FlashImporter plugin can be used in other projects
    // (as an actual plugin and stuff)
    public static List<string> FilesInDirectory(string path)
    {
        List<string> files = new();
        DirAccess directory = DirAccess.Open(path);
        if (directory != null)
        {
            try
            {
                directory.ListDirBegin();
                while (true)
                {
                    string file = directory.GetNext();
                    if (file == "") break;
                    if (!file.StartsWith(".")) files.Add(file);
                }
            }
            finally
            {
                directory.ListDirEnd();
            }
        }
        return files;
    }

    public static AudioStream SearchAllAudioFormats(string filePath, bool throwError = true)
    {
        string finalPath = "";

        foreach (string type in AudioFileTypes)
        {
            if (ResourceLoader.Exists(filePath + type))
            {
                if (finalPath == "") finalPath = filePath + type;
                else GD.Print("Another audio file was found, but will not be used");
            }
        }
        if (ResourceLoader.Exists(finalPath)) return GD.Load<AudioStream>(finalPath);
        else
        {
            if (throwError) GD.PrintErr($"No audio file was found with the provided path: {filePath}");
            return null;
        }
    }
    
    #region Notification Shit
    [Export] private Color InfoNotificationColor { get; set; } = new(1f,1f, 1f);
    [Export] private Color WarningNotificationColor { get; set; } = new(0.79f,0.79f, 0);
    [Export] private Color ErrorNotificationColor { get; set; } = new(0.70f, 0f, 0f);
    
    [NodePath("Notification")] private Panel NotificationInstance;
    private Queue<(Panel, double)> notificationQueue = new();
    private readonly Dictionary<Panel, float> notificationPositions = new();
    private float YOffset;
    
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
		                GD.PrintRich($"[color=ORANGE][pulse]{fullMessage}[/pulse][/color]");
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
            AddChild(notificationInstance);
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
    #endregion
}
