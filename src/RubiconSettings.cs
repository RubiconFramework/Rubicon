using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Rubicon.backend.ui.notification;
using Rubicon.common.autoload.managers.enums;

namespace Rubicon;

public enum StrumSides
{
    Opponent,
    Spectator,
    Player
}

public enum ScrollSpeedType
{
    Multiplier,
    Constant
}
    
public enum GameLanguages
{
    English,
    Spanish
}
    
public enum TransitionType
{
    Vanilla,
    Fade,
    SawnOff,
    DiamondShapes
}

public class RubiconSettings
{
    public readonly GameplaySettings Gameplay = new();
    public readonly GameplayModifiers Modifiers = new();
    public readonly AudioSettings Audio = new();
    public readonly VideoSettings Video = new();
    public readonly MiscSettings Misc = new();

    public class GameplaySettings
    {
        public bool Downscroll { get; set; }
        public float ScrollSpeed { get; set; } = 1.0f;
        public ScrollSpeedType ScrollSpeedType { get; set; } = ScrollSpeedType.Constant; 
    }

    public class GameplayModifiers
    {
        public bool NoMissMode { get; set; }
        public bool PFCOnly { get; set; }
        public bool CoolMechanic { get; set; }
        public float HealthGainMult { get; set; } = 1.0f;
        public float HealthLossMult { get; set; } = 1.0f;
        public StrumSides StrumSides { get; set; } = StrumSides.Player;
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

    public RubiconSettings(string path)
    {
        try
        {
            RubiconSettings rubiconSettings = new();
            if (FileAccess.FileExists(path))
            {
                var jsonData = FileAccess.Open(path, FileAccess.ModeFlags.Read);
                string json = jsonData.GetAsText();

                if (!string.IsNullOrEmpty(json))
                {
                    rubiconSettings = JsonConvert.DeserializeObject<RubiconSettings>(json);
                    if (rubiconSettings != null)
                    {
                        Main.RubiconSettings = rubiconSettings;
                        GD.Print($"Settings loaded from file. [{path}]");
                    }
                }
            }
            else
            {
                Main.Instance.SendNotification("Settings file not found. Writing default settings to file.");
                rubiconSettings.GetDefaultSettings().Save();
                Main.RubiconSettings = rubiconSettings;
            }
        }
        catch (Exception e)
        {
            Main.Instance.SendNotification($"Failed to load or write default settings: {e.Message}", true, NotificationType.Error);
            throw;
        }
    }
    
    public RubiconSettings(){}

    public RubiconSettings GetDefaultSettings()
    {
        RubiconSettings defaultSettings = new();
        return defaultSettings;
    }
    
    public void Save()
    {
        try
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };
        
            string jsonData = JsonConvert.SerializeObject(Main.RubiconSettings, settings);
            using var file = FileAccess.Open(Main.SettingsFilePath, FileAccess.ModeFlags.Write);
            file.StoreString(jsonData);
        }
        catch (Exception e)
        {
            Main.Instance.SendNotification($"Failed to save settings: {e.Message}", true, NotificationType.Error);
        }
    }
}
