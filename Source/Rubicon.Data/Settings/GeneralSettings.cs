using Godot.Collections;
using Rubicon.Data.Settings.Attributes;

namespace Rubicon.Data.Settings;

public class GeneralSettings
{
	public Gameplay Gameplay { get; set; } = new();
	public Audio Audio { get; set; } = new();
	public Video Video { get; set; } = new();
	public Misc Misc { get; set; } = new();
	public Keybinds KeyBinds { get; set; } = new();
}

[RubiconSettingsSection("Gameplay", true, "res://Assets/UI/Menus/Settings/Gameplay.png")]
public class Gameplay
{
	public GameplayModifiers Modifiers { get; set; } = new();
	public bool DownScroll { get; set; } = false;
	public bool CenterBarLine { get; set; } = false;
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
	public double SfxVolume { get; set; } = 100f;
	public double MusicVolume { get; set; } = 100f;
}

[RubiconSettingsSection("Video", true, "res://Assets/UI/Menus/Settings/Video.png")]
public class Video
{
	public bool Fullscreen { get; set; } = false;
	public bool VSync { get; set; } = false;
	public int Fps { get; set; } = 144;
}

[RubiconSettingsSection("Misc", true, "res://Assets/UI/Menus/Settings/Misc.png")]
public class Misc
{
	public bool PrintSettingsOnConsole { get; set; } = false;
}

[RubiconSettingsSection("Keybinds", true, "res://Assets/UI/Menus/Settings/Keybinds.png")]
public class Keybinds
{
	public Dictionary UiKeyBinds { get; set; } = new();
	public Dictionary GameKeyBinds { get; set; } = new();	
}