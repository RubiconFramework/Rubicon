using System.IO;

namespace Rubicon.backend.scripts;

/// <summary>
/// haxe looking headass paths -dunine
/// at the moment, this will only work with music in source, i'll do the support for mods laterrr -blear
/// </summary>
public static class ExternalPaths
{
    private static char[] invalidChars = new char[] {'~', '&', '\\', ';', ':', '<', '>', '#'};
    private static char[] hideChars = new char[] {'~', '/', '[', '.', '\'', '%', '?', '!', ']'};
    
    public static string Inst(string song)
    {
        string path = $"{Directory.GetCurrentDirectory().Split('\\')[0]}//common/songs/{FormatToSongPath(song)}/Inst";
        GD.Print("Instrumental loaded at Path: "+path);
        return path;
    }
    
    public static string Voices(string song)
    {
        string path = $"{Directory.GetCurrentDirectory().Split('\\')[0]}//common/songs/{FormatToSongPath(song)}/Voices";
        GD.Print("Voices loaded at Path: "+path);
        return path;
    }

    public static string FormatToSongPath(string path)
    {
        path = string.Join("-", path.Replace(' ', '-').Split(invalidChars));
        return string.Join("", path.Split(hideChars)).ToLower();
    }
}
