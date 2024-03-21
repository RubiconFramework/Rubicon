using System.Collections.Generic;
using BaseRubicon.Backend.Autoload;
using BaseRubicon.Backend.Autoload.Debug.ScreenNotifier;
using BaseRubicon.Backend.Scripts;
using BaseRubicon.Scenes.Options.Submenus.Audio.Enums;
using BaseRubicon.Scenes.Options.Submenus.Gameplay.Enums;
using BaseRubicon.Scenes.Options.Submenus.Misc.Enums;

namespace BaseRubicon.Scenes.Options.Elements;

public class SettingsData
{
    public readonly GameplaySettings Gameplay = new();
    public readonly AudioSettings Audio = new();
    public readonly VideoSettings Video = new();
    public readonly MiscSettings Misc = new();
    private Dictionary<string, string> Keybinds { get; set; } = new();
    
    public class GameplaySettings
    {
        public bool Downscroll { get; set; }
        public float ScrollSpeed { get; set; } = 1.0f;
        public ScrollSpeedType ScrollSpeedType { get; set; } = ScrollSpeedType.Constant;   
    }

    public class AudioSettings
    {
        public float MasterVolume { get; set; } = 50;
        public float MusicVolume { get; set; } = 100;
        public float SFXVolume { get; set; } = 100;
        public float InstVolume { get; set; } = 100;
        public float VoiceVolume { get; set; } = 100;
        public OutputMode OutputMode { get; set; } = OutputMode.Stereo;
    }

    public class VideoSettings
    {
        public int MaxFPS { get; set; } = 144;
        public DisplayServer.WindowMode WindowMode { get; set; } = DisplayServer.WindowMode.Windowed;
        public DisplayServer.VSyncMode VSync { get; set; } = DisplayServer.VSyncMode.Disabled;
    }

    public class MiscSettings
    {
        public GameLanguages Languages { get; set; } = GameLanguages.English;
        public TransitionType Transitions { get; set; } = TransitionType.Vanilla;
        public bool DiscordRichPresence { get; set; } = true;
        public bool OptionsMenuAnimations { get; set; } = true;
        public bool SceneTransitions { get; set; }
    }
    
    public static SettingsData GetDefaultSettings()
    {
        SettingsData defaultSettings = new();
        return defaultSettings;
    }
    
    public void SetKeybind(string action, string button)
    {
        Keybinds[action] = button;
        ScreenNotifier.Instance.Notify($"{action} bound to {button}");
        JsonSettingsManager.SaveSettingsToFile(Global.SettingsFilePath);
    }
    
    public void RemoveKeybind(string action)
    {
        if (Keybinds.ContainsKey(action)) Keybinds.Remove(action);
        JsonSettingsManager.SaveSettingsToFile(Global.SettingsFilePath);
    }
    
    public List<string> GetKeybind(string action)
    {
        List<string> keybinds = new();
        foreach (var kvp in Keybinds) if (kvp.Value == action) keybinds.Add(kvp.Key);
        if (keybinds.Count > 0) return keybinds;
        else return new() { "N/A" };
    }
}
