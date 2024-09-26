using Godot.Collections;
using Rubicon.Data.Settings.Attributes;

namespace Rubicon.Data.Settings;

public class GeneralSettings
{
	public Gameplay Gameplay { get; set; } = new();
	public Audio Audio { get; set; } = new();
	public Video Video { get; set; } = new();
	public Misc Misc { get; set; } = new();
	public Keybinds Keybinds { get; set; } = new();
}

[RubiconSettingsSection("Gameplay", true, "res://Assets/UI/Menus/Settings/Gameplay.png")]
public class Gameplay
{
	public GameplayModifiers Modifiers { get; set; } = new();
	public bool Downscroll { get; set; } = false;
	public bool Middlescroll { get; set; } = false;
	public bool GhostTapping { get; set; } = true;
	public bool FlashingLights { get; set; } = true;
	
	[RubiconSettingsSubsection("Gameplay Modifiers")]
	public class GameplayModifiers
	{
		[StepValue(0.01f)] public double PlaybackRate { get; set; } = 1f;
		[StepValue(0.1f)] public double HealthGain { get; set; } = 1f;
		[StepValue(0.1f)] public double HealthLoss { get; set; } = 1f;
		[StepValue(0.1f)] public double HealthDrain { get; set; } = 0.5f;
		public bool OpponentDrainsHealth { get; set; } = false;
	}
}

[RubiconSettingsSection("Audio", true, "res://Assets/UI/Menus/Settings/Audio.png")]
public class Audio
{
	public double AudioOffset { get; set; } = 0f;
	public double MasterVolume { get; set; } = 100f;
	public double InstVolume { get; set; } = 100f;
	public double VoicesVolume { get; set; } = 100f;
	public double SFXVolume { get; set; } = 100f;
	public double MusicVolume { get; set; } = 100f;
}

[RubiconSettingsSection("Video", true, "res://Assets/UI/Menus/Settings/Video.png")]
public class Video
{
	public bool Fullscreen { get; set; } = false;
	public bool VSync { get; set; } = false;
	public int FPS { get; set; } = 144;
}

[RubiconSettingsSection("Misc", true, "res://Assets/UI/Menus/Settings/Misc.png")]
public class Misc
{
	public bool PrintSettingsOnConsole { get; set; } = false;
}

[RubiconSettingsSection("Keybinds", true, "res://Assets/UI/Menus/Settings/Keybinds.png")]
public class Keybinds
{
	public Dictionary UIKeybinds { get; set; } = new();
	public Dictionary GameKeybinds { get; set; } = new();	
}