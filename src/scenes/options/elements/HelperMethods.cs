using BaseRubicon.Backend.Autoload;
using BaseRubicon.Scenes.Options.Submenus.Audio.Enums;
using AudioManager = BaseRubicon.Backend.Autoload.Managers.AudioManager.AudioManager;

namespace BaseRubicon.Scenes.Options.Elements;

public class HelperMethods
{
    public void SetVolume(VolumeType volumeType, float v)
    {
        switch (volumeType)
        {
            case VolumeType.MasterVolume:
                Global.Settings.Audio.MasterVolume = v;
                AudioManager.Instance.VolumeChange(v);
                break;
            case VolumeType.MusicVolume:
                Global.Settings.Audio.MusicVolume = v;
                AudioManager.Instance.VolumeChange(v);
                break;
            case VolumeType.SFXVolume:
                Global.Settings.Audio.SFXVolume = v;
                AudioManager.Instance.VolumeChange(v);
                break;
            case VolumeType.InstVolume:
                Global.Settings.Audio.InstVolume = v;
                AudioManager.Instance.VolumeChange(v);
                break;
            case VolumeType.VoiceVolume:
                Global.Settings.Audio.VoiceVolume = v;
                AudioManager.Instance.VolumeChange(v);
                break;
        }
    }

    public void SetVSync(DisplayServer.VSyncMode v)
    {
        Global.Settings.Video.VSync = v;
        DisplayServer.Singleton.WindowSetVsyncMode(v);
    }

    public void SetWindowMode(DisplayServer.WindowMode v)
    {
        Global.Settings.Video.WindowMode = v;
        DisplayServer.Singleton.WindowSetMode(v);
    }

    public void SetMaxFPS(float v)
    {
        if ((int)v == 1500)
        {
            Global.Settings.Video.MaxFPS = 1500;
            Engine.Singleton.MaxFps = 0;
            return;
        }
        
        Global.Settings.Video.MaxFPS = (int)v;
        Engine.Singleton.MaxFps = (int)v;
    }
    
    public void SetDiscordRPC(bool v)
    {
        Global.Settings.Misc.DiscordRichPresence = v;
        Global.Instance.DiscordRPC(v);
    }
}
