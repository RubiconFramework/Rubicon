using Rubicon.Data.Settings.Attributes;

namespace Rubicon.Data.Settings;

public class GeneralSettings
{
	public Modifiers Modifiers { get; set; } = new();
	public Gameplay Gameplay { get; set; } = new();
	public Audio Audio { get; set; } = new();
	public Video Video { get; set; } = new();
	public Misc Misc { get; set; } = new();
}

[RubiconSettingsSection("Modifiers", false)]
public class Modifiers
{
	[StepValue(0.1f)] public double HealthGain { get; set; } = 1f;
	[StepValue(0.1f)] public double HealthLoss { get; set; } = 1f;
	[StepValue(0.1f)] public double HealthDrain { get; set; } = 0.5f;
	public bool OpponentDrainsHealth { get; set; } = false;
}

[RubiconSettingsSection("Gameplay", true, "res://Assets/UI/Menus/Settings/Gameplay.png")]
public class Gameplay
{
	public bool Downscroll { get; set; } = false;
	public bool Middlescroll { get; set; } = false;
	public bool GhostTapping { get; set; } = true;
	public bool FlashingLights { get; set; } = true;
}

[RubiconSettingsSection("Audio", true, "res://Assets/UI/Menus/Settings/Audio.png")]
public class Audio
{
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
	public bool KillPeople { get; set; } = false;
	public bool PrintSettingsOnConsole { get; set; } = false;
}
