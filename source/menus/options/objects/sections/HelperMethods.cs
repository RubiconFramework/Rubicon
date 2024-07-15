using System.IO;
using System.IO.Compression;
using System.Text;
using Rubicon.backend.autoload;
using Rubicon.Backend.Autoload;
using Rubicon.backend.autoload.enums;

namespace Rubicon.menus.options.objects.sections;

public class HelperMethods
{
    public static void SetVolume(VolumeType volumeType, float v)
    {
        switch (volumeType)
        {
            case VolumeType.MasterVolume:
                RubiconSettings.Audio.MasterVolume = v;
                VolumeManager.SetVolume(v);
                break;
            case VolumeType.MusicVolume:
                RubiconSettings.Audio.MusicVolume = v;
                VolumeManager.SetVolume(v);
                break;
            case VolumeType.SFXVolume:
                RubiconSettings.Audio.SFXVolume = v;
                VolumeManager.SetVolume(v);
                break;
            case VolumeType.InstVolume:
                RubiconSettings.Audio.InstVolume = v;
                VolumeManager.SetVolume(v);
                break;
            case VolumeType.VoiceVolume:
                RubiconSettings.Audio.VoiceVolume = v;
                VolumeManager.SetVolume(v);
                break;
        }
    }

    public static void SetVSync(DisplayServer.VSyncMode v)
    {
        RubiconSettings.Video.VSync = v;
        DisplayServer.Singleton.WindowSetVsyncMode(v);
    }

    public static void SetWindowMode(DisplayServer.WindowMode v)
    {
        RubiconSettings.Video.WindowMode = v;
        DisplayServer.Singleton.WindowSetMode(v);
    }

    public static void SetMaxFPS(float v)
    {
        if ((int)v == 1500)
        {
            RubiconSettings.Video.MaxFPS = 1500;
            Engine.Singleton.MaxFps = 0;
            return;
        }
        
        RubiconSettings.Video.MaxFPS = (int)v;
        Engine.Singleton.MaxFps = (int)v;
    }
    
    public static void SetDiscordRPC(bool v)
    {
        RubiconSettings.Misc.DiscordRichPresence = v;
        DiscordRichPresence.Instance.Toggle(v);
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
