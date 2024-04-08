using System.Collections.Generic;
using System.IO;

namespace Rubicon.Backend.Scripts;
public static class Paths
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
    
    public static IEnumerable<string> FilesInDirectory(string path)
    {
        List<string> files = new();
        using var directory = DirAccess.Open(path);
        if (directory != null)
        {
            try
            {
                directory.ListDirBegin();
                while (true)
                {
                    string file = directory.GetNext();
                    if (file == "") break;
                    if (!file.StartsWith(".")) files.Add(file);
                }
            }
            finally
            {
                directory.ListDirEnd();
            }
        }
        return files;
    }
}
