global using GameplayScene = BaseRubicon.Gameplay.GameplayScene;
global using Conductor = BaseRubicon.Backend.Autoload.Conductor;
global using Godot;
global using System;
global using Godot.Sharp.Extras;

using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using BaseRubicon.Backend.Autoload.Debug.ScreenNotifier;
using BaseRubicon.Backend.Scripts;
using BaseRubicon.Gameplay.Elements.Resources;
using BaseRubicon.Scenes.Options.Elements;
using DiscordRPC;
using DiscordRPC.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using FileAccess = Godot.FileAccess;

namespace BaseRubicon.Backend.Autoload;

[Icon("res://assets/miscicons/autoload.png")]
public partial class Global : Node
{
	[Export] public string DiscordRpcClientID = "1218405526677749760";
	[Export] public string SettingsFilePath = "user://settings.json";

	public static Chart Song; // the fuck is this for ? -duo
							  // ^ so we can access it from anywhere without having to reference gameplay :) -lego
							  // stop talking -duo
							  // :( -lego
							  // im sorry that was rude -duo
							  
	public static Global Instance { get; private set; } = new();
	
 	public static readonly string[] AudioFormats = { "mp3", "ogg", "wav" , "flac" };
    public static readonly string[] DefaultNoteDirections = { "left", "down", "up", "right" };
	public static readonly Vector2 EngineWindowSize = new((float)ProjectSettings.GetSetting("display/window/size/viewport_width"), (float)ProjectSettings.GetSetting("display/window/size/viewport_height"));

	public static SettingsData Settings { get; set; } = new();
	public static DiscordRpcClient DiscordRpcClient = new(Instance.DiscordRpcClientID);

	public override void _EnterTree()
    {
        base._EnterTree();

        Instance = this;
        DiscordRPC(true);
        LoadSettings(SettingsFilePath);
        RenderingServer.SetDefaultClearColor(new(0,0,0));
        TranslationServer.SetLocale(Settings.Misc.Languages.ToString().ToLower());
        
        if ((bool)ProjectSettings.GetSetting("use_project_name_user_dir",true)){
			var customUserDir = ProjectSettings.GetSetting("application/config/custom_user_dir_name", "Rubicon/Engine").ToString();
			var projectName = ProjectSettings.GetSetting("application/config/name", "Rubicon").ToString();
			
			switch (customUserDir)
			{
				case "Rubicon/Engine" when projectName != "Rubicon":
					ScreenNotifier.Instance.Notify("New project name has been found. Reload project.godot for it to apply.");
					ProjectSettings.SetSetting("application/config/custom_user_dir_name", $"Rubicon/{projectName}");
					ProjectSettings.Save();
					break;
				default:
				{
					if (customUserDir != "Rubicon/Engine" && projectName == "Rubicon")
					{
						ScreenNotifier.Instance.Notify("Base engine detected. Reload project.godot for it to apply.");
						ProjectSettings.SetSetting("application/config/custom_user_dir_name", "Rubicon/Engine");
						ProjectSettings.Save();
					}
					else ScreenNotifier.Instance.Notify($"Data stored at: user://{customUserDir}");
					break;
				}
			}
		}
    }

	public override void _ExitTree()
	{
		base._ExitTree();
		Settings = null;
		DiscordRPC(false);
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
				settings = JsonConvert.DeserializeObject<SettingsData>(json);
				GD.Print($"Settings loaded from file. [{path}]");
			}
			else
			{
				GD.Print("Settings file not found. Writing default settings to file.");
				settings.GetDefaultSettings().SaveSettings();
			}
			Global.Settings = settings;
		}
		catch (Exception e)
		{
			GD.PrintErr($"Failed to load or write default settings: {e.Message}");
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
}
