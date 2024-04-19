using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Rubicon.autoload.global.elements;
using Rubicon.autoload.managers.audiomanager.enums;
using Rubicon.scenes.options.submenus.gameplay.enums;
using Rubicon.scenes.options.submenus.misc.enums;

namespace Rubicon.scenes.options.elements;

public class RubiconSettings
{
    public readonly GameplaySettings Gameplay = new();
    public readonly AudioSettings Audio = new();
    public readonly VideoSettings Video = new();
    public readonly MiscSettings Misc = new();
    
    public class GameplaySettings
    {
        public bool Downscroll { get; set; }
        public float ScrollSpeed { get; set; } = 1.0f;
        public ScrollSpeedType ScrollSpeedType { get; set; } = ScrollSpeedType.Constant;   
    }

    public class AudioSettings
    {
        public float MasterVolume { get; set; } = 50;
        public OutputMode OutputMode { get; set; } = OutputMode.Stereo;
        public float MusicVolume { get; set; } = 100;
        public float SFXVolume { get; set; } = 100;
        public float InstVolume { get; set; } = 100;
        public float VoiceVolume { get; set; } = 100;
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
        public bool SceneTransitions { get; set; } = true;
    }
    
    public RubiconSettings GetDefaultSettings()
    {
        RubiconSettings defaultSettings = new();
        return defaultSettings;
    }
    
    public void Save()
    {
        try
        {
            if (Main.RubiconSettings == null)
            {
                Main.Instance.Alert("Settings object is null.", true, NotificationType.Error);
                return;
            }

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };
        
            string jsonData = JsonConvert.SerializeObject(Main.RubiconSettings, settings);
            using var file = FileAccess.Open(Main.Instance.SettingsFilePath, FileAccess.ModeFlags.Write);
            if (file == null)
            {
                Main.Instance.Alert("Failed to open settings file for writing.", true, NotificationType.Error);
                return;
            }

            file.StoreString(jsonData);
        }
        catch (Exception e)
        {
            Main.Instance.Alert($"Failed to save settings: {e.Message}", true, NotificationType.Error);
        }
    }
}
