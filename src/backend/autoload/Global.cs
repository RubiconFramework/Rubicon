global using GameplayScene = Rubicon.gameplay.GameplayScene;
global using Conductor = Rubicon.backend.autoload.Conductor;
global using Godot;
global using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using DiscordRPC;
using DiscordRPC.Logging;
using Rubicon.backend.autoload.debug.ScreenNotifier;
using Rubicon.backend.scripts;
using Rubicon.gameplay.elements.resources;
using Rubicon.scenes.options.elements;

namespace Rubicon.backend.autoload;

[Icon("res://assets/miscicons/autoload.png")]
public partial class Global : Node
{
	public static Chart Song; // the fuck is this for ? -duo
							  // ^ so we can access it from anywhere without having to reference gameplay :) -lego
							  // stop talking -duo
							  // :( -lego
							  // im sorry that was rude -duo
							  
	public static SettingsData Settings { get; set; } = new();
	public static Global Instance { get; private set; } = new();
	
 	public static readonly string[] audioFormats = { "mp3", "ogg", "wav" , "flac" };
    public static readonly string SettingsFilePath = "user://settings.json";
    public static readonly string[] defaultNoteDirections = { "left", "down", "up", "right" };
    public static readonly string DiscordRpcClientID = "1218405526677749760";
	public static readonly string EngineVersion = ProjectSettings.Singleton.GetSetting("application/config/version", "1.0").ToString();
	public static readonly Vector2 windowSize = new((float)ProjectSettings.GetSetting("display/window/size/viewport_width"), (float)ProjectSettings.GetSetting("display/window/size/viewport_height"));

	public static DiscordRpcClient DiscordRpcClient = new(DiscordRpcClientID);

	public override void _EnterTree()
    {
        base._EnterTree();

        Instance = this;
        DiscordRPC(true);
        RenderingServer.SetDefaultClearColor(new(0,0,0));
        JsonSettingsManager.LoadSettingsFromFile(SettingsFilePath);
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

				string sceneName = GetTree().CurrentScene?.Name ?? "Unknown Scene";
				DiscordRpcClient.SetPresence(new()
				{
					Details = sceneName,
					Assets = new()
					{
						LargeImageKey = "image_large",
						LargeImageText = $"Version {EngineVersion}",
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
