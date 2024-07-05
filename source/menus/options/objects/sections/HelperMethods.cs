using System.IO;
using System.IO.Compression;
using System.Text;
using OldRubicon.autoload.enums;
using VolumeManager = OldRubicon.autoload.VolumeManager;

namespace OldRubicon.scenes.options.objects.sections;

public class HelperMethods
{
    public void SetVolume(VolumeType volumeType, float v)
    {
        switch (volumeType)
        {
            case VolumeType.MasterVolume:
                Main.RubiconSettings.Audio.MasterVolume = v;
                VolumeManager.Instance.ChangeVolume(v);
                break;
            case VolumeType.MusicVolume:
                Main.RubiconSettings.Audio.MusicVolume = v;
                VolumeManager.Instance.ChangeVolume(v);
                break;
            case VolumeType.SFXVolume:
                Main.RubiconSettings.Audio.SFXVolume = v;
                VolumeManager.Instance.ChangeVolume(v);
                break;
            case VolumeType.InstVolume:
                Main.RubiconSettings.Audio.InstVolume = v;
                VolumeManager.Instance.ChangeVolume(v);
                break;
            case VolumeType.VoiceVolume:
                Main.RubiconSettings.Audio.VoiceVolume = v;
                VolumeManager.Instance.ChangeVolume(v);
                break;
        }
    }

    public void SetVSync(DisplayServer.VSyncMode v)
    {
        Main.RubiconSettings.Video.VSync = v;
        DisplayServer.Singleton.WindowSetVsyncMode(v);
    }

    public void SetWindowMode(DisplayServer.WindowMode v)
    {
        Main.RubiconSettings.Video.WindowMode = v;
        DisplayServer.Singleton.WindowSetMode(v);
    }

    public void SetMaxFPS(float v)
    {
        if ((int)v == 1500)
        {
            Main.RubiconSettings.Video.MaxFPS = 1500;
            Engine.Singleton.MaxFps = 0;
            return;
        }
        
        Main.RubiconSettings.Video.MaxFPS = (int)v;
        Engine.Singleton.MaxFps = (int)v;
    }
    
    public void SetDiscordRPC(bool v)
    {
        Main.RubiconSettings.Misc.DiscordRichPresence = v;
        //DiscordRichPresence.Instance.Toggle(v);
    }
    
    public static string CompressString(string text)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        using var mso = new MemoryStream();
        using (var gzs = new GZipStream(mso, CompressionMode.Compress)) gzs.Write(bytes, 0, bytes.Length);
        return Convert.ToBase64String(mso.ToArray());
    }

    public static string DecompressString(string compressedText)
    {
        var bytes = Convert.FromBase64String(compressedText);
        using var msi = new MemoryStream(bytes);
        using var mso = new MemoryStream();
        using (var gzs = new GZipStream(msi, CompressionMode.Decompress)) gzs.CopyTo(mso);
        return Encoding.UTF8.GetString(mso.ToArray());
    }
}
