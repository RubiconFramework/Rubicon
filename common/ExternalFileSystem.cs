using System.IO;

namespace Rubicon.common;
public static class ExternalFileSystem
{
    //haxe spotted -dunine
    //at the moment, this will only work with music in source, i'll do the support for mods laterrr -blear
    
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
        char[] invalidChars = new char[] {'~', '&', '\\', ';', ':', '<', '>', '#'};
        char[] hideChars = new char[] {'~', '/', '[', '.', '\'', '%', '?', '!', ']'};
        path = string.Join("-", path.Replace(' ', '-').Split(invalidChars));
        return string.Join("", path.Split(hideChars)).ToLower();
    }
}
