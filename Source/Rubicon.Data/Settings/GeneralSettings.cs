using Rubicon.Data.Settings.Attributes;

namespace Rubicon.Data.Settings;

public class GeneralSettings
{
	public Modifiers Modifiers { get; set; } = new();
	public Gameplay Gameplay { get; set; } = new();
	public Audio Audio { get; set; } = new();
	public Video Video { get; set; } = new();
	public Misc Misc { get; set; } = new();
	public Debug Debug { get; set; } = new();
}

[RubiconSettingsSection("Modifiers")]
public class Modifiers
{
	[StepValue(0.1f)] public float HealthGain { get; set; } = 1f;
	[StepValue(0.1f)] public float HealthLoss { get; set; } = 1f;
	[StepValue(0.1f)] public float HealthDrain { get; set; } = 0.5f;
	public bool OpponentDrainsHealth { get; set; } = false;
}

[RubiconSettingsSection("Gameplay")]
public class Gameplay
{
	public bool DownScroll { get; set; } = false;
	public bool CenterBarLine { get; set; } = false;
	public bool GhostTapping { get; set; } = true;
	public bool FlashingLights { get; set; } = true;
}

[RubiconSettingsSection("Audio")]
public class Audio
{
	public float MasterVolume { get; set; } = 100f;
	public float InstVolume { get; set; } = 100f;
	public float VoicesVolume { get; set; } = 100f;
	public float SFXVolume { get; set; } = 100f;
	public float MusicVolume { get; set; } = 100f;
}

[RubiconSettingsSection("Video")]
public class Video
{
	public bool Fullscreen { get; set; } = false;
	public bool VSync { get; set; } = false;
	public int FPS { get; set; } = 144;
}

[RubiconSettingsSection("Misc")]
public class Misc
{
	public bool KillPeople { get; set; } = false;
}

[RubiconSettingsSection("Debug")]
public class Debug
{
	public bool PrintSettingsOnConsole { get; set; } = false;
}
