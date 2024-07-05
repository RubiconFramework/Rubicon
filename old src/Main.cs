
global using Godot; 
global using System;
global using Godot.Sharp.Extras;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OldRubicon;

//[Icon("res://assets/misc/autoload.png")]
public partial class Main : CanvasLayer
{

	public const string SettingsFilePath = "user://settings.json";
    public static readonly string RubiconVersion = ProjectSettings.Singleton.GetSetting("application/config/version", "1").ToString();
    public static RubiconSettings RubiconSettings { get; set; } = new();
    

	public override void _Ready()
	{
		this.OnReady();
		RubiconSettings.Load(SettingsFilePath);
		
		RenderingServer.SetDefaultClearColor(new(0,0,0));
		
		TranslationServer.SetLocale(RubiconSettings.Misc.Languages.ToString().ToLower());
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		RubiconSettings = null;
	}
}
