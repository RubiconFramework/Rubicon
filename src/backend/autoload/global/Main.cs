global using GameplayScene = Rubicon.Gameplay.GameplayScene;
global using Conductor = Rubicon.Backend.Autoload.Global.Conductor;
global using Main = Rubicon.Backend.Autoload.Global.Main;
global using Godot;
global using System;
global using Godot.Sharp.Extras;

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using DiscordRPC;
using DiscordRPC.Logging;
using Newtonsoft.Json;
using Rubicon.Backend.Autoload.Debug.ScreenNotifier;
using Rubicon.Gameplay.Elements.Resources;
using Rubicon.Scenes.Options.Elements;
using FileAccess = Godot.FileAccess;

namespace Rubicon.Backend.Autoload.Global;

[Icon("res://assets/miscicons/autoload.png")]
public partial class Main : CanvasLayer
{
	[Export] public string DiscordRpcClientID = "1218405526677749760";
	[Export] public string SettingsFilePath = "user://settings.json";
	[Export] private Color InfoNotificationColor { get; set; } = new(0.32f,0.32f, 0.32f);
	[Export] private Color WarningNotificationColor { get; set; } = new(0.79f,0.79f, 0);
	[Export] private Color ErrorNotificationColor { get; set; } = new(0.70f, 0f, 0f);

	public static Chart Song; // the fuck is this for ? -duo
							  // ^ so we can access it from anywhere without having to reference gameplay :) -lego
							  // stop talking -duo
							  // :( -lego
							  // im sorry that was rude -duo
							  
	public static Main Instance { get; private set; } = new();
	
 	public static readonly string[] AudioFormats = { "mp3", "ogg", "wav" , "flac" };
    public static readonly string[] DefaultNoteDirections = { "left", "down", "up", "right" };
	public static readonly Vector2 EngineWindowSize = new((float)ProjectSettings.GetSetting("display/window/size/viewport_width"), (float)ProjectSettings.GetSetting("display/window/size/viewport_height"));

	public static SettingsData GameSettings { get; set; } = new();
	public static DiscordRpcClient DiscordRpcClient = new(Instance.DiscordRpcClientID);
	
	[NodePath("Notification")] private Panel NotificationInstance;
	private Queue<(Panel, double)> notificationQueue = new();
	private float YOffset;

	public override void _Ready()
	{
		this.OnReady();
		
		Instance = this;
		RenderingServer.SetDefaultClearColor(new(0,0,0));
		TranslationServer.SetLocale(GameSettings.Misc.Languages.ToString().ToLower());
        
		if ((bool)ProjectSettings.GetSetting("use_project_name_user_dir",true)){
			var customUserDir = ProjectSettings.GetSetting("application/config/custom_user_dir_name", "Rubicon/Engine").ToString();
			var projectName = ProjectSettings.GetSetting("application/config/name", "Rubicon").ToString();
			
			switch (customUserDir)
			{
				case "Rubicon/Engine" when projectName != "Rubicon":
					Alert("New project name has been found. Reload project.godot for it to apply.");
					ProjectSettings.SetSetting("application/config/custom_user_dir_name", $"Rubicon/{projectName}");
					ProjectSettings.Save();
					break;
				default:
				{
					if (customUserDir != "Rubicon/Engine" && projectName == "Rubicon")
					{
						Alert("Base engine detected. Reload project.godot for it to apply.");
						ProjectSettings.SetSetting("application/config/custom_user_dir_name", "Rubicon/Engine");
						ProjectSettings.Save();
					}
					else Alert($"Data stored at: user://{customUserDir}");
					break;
				}
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
			var animalationtolongplayer = panel.GetNode<AnimationPlayer>("animalationtolongplayer");
			progressBar.Value = duration;

			if (Mathf.Abs(duration - 0.5f) < 0.01f) animalationtolongplayer.Play("out");
			if (duration <= 0) OnNotificationTimeout(panel);
			else notificationQueue.Enqueue((panel, duration));
		}
        
		void OnNotificationTimeout(Panel panel)
		{ 
			notificationQueue = new(notificationQueue.Where(item => item.Item1 != panel));
			panel.QueueFree();
			YOffset -= panel.GetRect().Size.Y + 10;
    
			float yOffset = YOffset;
			foreach (var (remainingPanel, _) in notificationQueue)
			{
				remainingPanel.Position = new(remainingPanel.Position.X, yOffset);
				yOffset += remainingPanel.GetRect().Size.Y + 10;
			}
		}
	}

	public static void LoadSettings(string path)
	{
		try
		{
			SettingsData settings = new();
			if (FileAccess.FileExists(path))
			{
				var jsonData = FileAccess.Open(path, FileAccess.ModeFlags.Read);
				string json = jsonData.GetAsText();

				if (!string.IsNullOrEmpty(json))
				{
					settings = JsonConvert.DeserializeObject<SettingsData>(json);
					if (settings != null)
					{
						GameSettings = settings;
						GD.Print($"Settings loaded from file. [{path}]");
					}
				}
			}
			else
			{
				Instance.Alert("Settings file not found. Writing default settings to file.");
				settings.GetDefaultSettings().Save();
				GameSettings = settings;
			}
		}
		catch (Exception e)
		{
			GD.PrintErr($"Failed to load or write default settings: {e.Message}");
			throw;
		}
	}

	public static IEnumerable<string> FilesInDirectory(string path)
    {
	    List<string> files = new();
		var directory = DirAccess.Open(path);
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
    
    public static string CompressString(string text)
    {
	    var bytes = Encoding.UTF8.GetBytes(text);
	    using var mso = new MemoryStream();
	    using (var gzs = new GZipStream(mso, CompressionMode.Compress)) gzs.Write(bytes, 0, bytes.Length);
	    return Convert.ToBase64String(mso.ToArray());
    }

    public static string DecompressString(string compressedText)
    {
	    var bytes = Convert.FromBase64String(compressedText);
	    using var msi = new MemoryStream(bytes);
	    using var mso = new MemoryStream();
	    using (var gzs = new GZipStream(msi, CompressionMode.Decompress)) gzs.CopyTo(mso);
	    return Encoding.UTF8.GetString(mso.ToArray());
    }

	public static float LinearToDb(float linear)
	{
		if (linear <= 0) return -80.0f;
		return (float)Math.Log10(linear) * 20;
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
	
	public void Alert(string message, bool printToConsole = true, NotificationType type = NotificationType.Info, float duration = 5.0f)
    {
        StackTrace stackTrace = new();
        StackFrame stackFrame = stackTrace.GetFrame(1);
        
        string fullMessage = $"[{type.ToString().ToUpper()} - {stackFrame!.GetMethod()?.Name}] -> {message}";
        
        if (NotificationInstance.Duplicate() is Panel notificationInstance)
        {
            if (YOffset == 0) YOffset = 32;
            else YOffset += notificationInstance.GetRect().Size.Y + 10;

            notificationInstance.Visible = true;
            notificationInstance.Position = new(notificationInstance.GetRect().Position.X, YOffset);

            var progressBar = notificationInstance.GetNode<ProgressBar>("DurationBar");
            var messageLabel = notificationInstance.GetNode<Label>("Message");
            var animalationtolongplayer = notificationInstance.GetNode<AnimationPlayer>("animalationtolongplayer");

            switch (type)
            {
                case NotificationType.Info:
                    if (printToConsole) GD.Print(fullMessage);
                    progressBar.AddThemeColorOverride("bg_color", InfoNotificationColor);
                    break;
                case NotificationType.Warning:
                    if (printToConsole) GD.Print(fullMessage);
                    progressBar.AddThemeColorOverride("bg_color", WarningNotificationColor);
                    break;
                case NotificationType.Error:
                    if (printToConsole) GD.PrintErr(fullMessage);
                    progressBar.AddThemeColorOverride("bg_color", ErrorNotificationColor);
                    break;
            }

            messageLabel.Text = fullMessage;
            progressBar.MaxValue = duration;
            progressBar.Value = duration;
            animalationtolongplayer.Play("in");

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
}
