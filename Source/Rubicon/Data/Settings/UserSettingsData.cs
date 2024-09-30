using Godot.Collections;
using Rubicon.Data.Settings.Attributes;

namespace Rubicon.Data.Settings;

public partial class UserSettingsData
{
    public VideoSection Video = new();

    public GameplaySection Gameplay = new();

    public AudioSection Audio = new();

    public partial void Load(ConfigFile config);

    public partial ConfigFile CreateConfigFileInstance();
    
    public partial Variant GetSetting(string key);

    public partial void SetSetting(string key, Variant val);
}

public class VideoSection
{
    public DisplayServer.WindowMode Fullscreen = (DisplayServer.WindowMode)ProjectSettings.GetSetting("display/window/size/mode").AsInt64();
    
    public Vector2I Resolution = new(ProjectSettings.GetSetting("display/window/size/window_width_override").AsInt32(), ProjectSettings.GetSetting("display/window/size/window_height_override").AsInt32());
    
    public DisplayServer.VSyncMode VSync = (DisplayServer.VSyncMode)ProjectSettings.GetSetting("display/window/vsync/vsync_mode").AsInt64();
    
    public int MaxFps = ProjectSettings.GetSetting("application/run/max_fps").AsInt32();

    public Settings3DSection Settings3D = new();
    
    public class Settings3DSection
    {
        public Viewport.Scaling3DModeEnum Scaling3DMode = (Viewport.Scaling3DModeEnum)ProjectSettings.GetSetting("rendering/scaling_3d/scale").AsInt64();
        
        public float RenderScale = ProjectSettings.GetSetting("rendering/scaling_3d/scale").AsSingle();
        
        public float FsrSharpness = ProjectSettings.GetSetting("rendering/scaling_3d/fsr_sharpness").AsSingle();
    }
}

public class GameplaySection
{
    public double Offset = 0.0d;
    public double VisualOffset = 0.0d;
    
    public bool DownScroll = false;
    public bool CenterBarLine = false;
    public bool GhostTapping = false;
    public bool FlashingLights = true;
    public bool Autoplay = false;

    public float NoteAmplitude = 0f; // 0 means set to the default chart note amplitude (scroll speed)

    // I'll do modifiers later LOL
}

public class AudioSection
{
    public float MasterVolume = 1.0f;
    public float MusicVolume = 1.0f;
    public float VocalsVolume = 1.0f;
    public float SfxVolume = 1.0f;
}