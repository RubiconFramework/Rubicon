using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Rubicon.Backend.Autoload;

public enum AudioOutputMode
{
    Stereo,
    Mono
}

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

public partial class RubiconSettings : Node
{
    public static RubiconSettings Instance;
    private GameplaySettings _Gameplay { get; set; } = new();
    private AudioSettings _Audio { get; set; } = new();
    private VideoSettings _Video { get; set; } = new();
    private MiscSettings _Misc { get; set; } = new();
    
    public static GameplaySettings Gameplay => Instance._Gameplay;
    public static AudioSettings Audio => Instance._Audio;
    public static VideoSettings Video => Instance._Video;
    public static MiscSettings Misc => Instance._Misc;

    public class GameplaySettings
    {
        public readonly GameplayModifiers Modifiers = new();
        public readonly GameplayOffsets Offsets = new();

        public bool Downscroll { get; set; }
        public bool Middlescroll { get; set; }
        public bool DisableOpponentStrums { get; set; }
        public float ScrollSpeed { get; set; } = 1.0f;
        public ScrollSpeedType ScrollSpeedType { get; set; } = ScrollSpeedType.Constant;

        public class GameplayModifiers
        {
            public bool NoMissMode { get; set; }
            public bool PFCOnly { get; set; }
            public bool CoolMechanic { get; set; }
            public float HealthGainMult { get; set; } = 1.0f;
            public float HealthLossMult { get; set; } = 1.0f;
            public float SongRate { get; set; } = 1.0f;
            public StrumSides StrumSides { get; set; } = StrumSides.Player;
        }

        public class GameplayOffsets
        {
            public float SoundOffset { get; set; } = 1.0f;
            public Vector2 RatingsPosition { get; set; } = new(0, 0);
            public Vector2 ComboPosition { get; set; } = new(0, 0);
        }
    }

    public class AudioSettings
    {
        public float MasterVolume { get; set; } = 50;
        public AudioOutputMode AudioOutputMode { get; set; } = AudioOutputMode.Stereo;
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

    public static RubiconSettings GetDefaultSettings()
    {
        return new RubiconSettings();
    }

    public void Load(string path)
    {
        try
        {
            if (FileAccess.FileExists(path))
            {
                var jsonData = FileAccess.Open(path, FileAccess.ModeFlags.Read);
                string json = jsonData.GetAsText();

                if (!string.IsNullOrEmpty(json))
                {
                    var loadedSettings = JsonConvert.DeserializeObject<RubiconSettings>(json);
                    if (loadedSettings != null)
                    {
                        Instance = loadedSettings;
                        GD.Print($"Settings loaded from file. [{path}]");
                    }
                }
            }
            else
            {
                GetDefaultSettings().Save();
                GD.Print($"Settings file not found. Writing default settings to file.");
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"Failed to load or write default settings: {e.Message}");
            throw;
        }
    }

    public void Save()
    {
        try
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented,
            };

            string jsonData = JsonConvert.SerializeObject(Instance, settings);
            using var file = FileAccess.Open("user://settings.json", FileAccess.ModeFlags.Write);
            file.StoreString(jsonData);
        }
        catch (Exception e)
        {
            GD.PrintErr($"Failed to save settings: {e.Message}");
        }
    }

    public override void _EnterTree()
    {
        Instance ??= this;
    }

    public override void _ExitTree()
    {
        if (Instance == this) Instance = null;
    }
}
