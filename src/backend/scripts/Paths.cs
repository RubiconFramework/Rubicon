namespace Rubicon.Backend.Scripts;
public static class Paths
{
    //haxe spotted -dunine
    //at the moment, this will only work with music in source, i'll do the support for mods laterrr -blear
    public static string Inst(string song){
        string path = $"C://common/songs/{FormatToSongPath(song)}/Inst";
        GD.Print("inst path: "+path);
        return path;
    }
    
    public static string Voices(string song){
        string path = $"C://common/songs/{FormatToSongPath(song)}/Voices";
        GD.Print("voices path: "+path);
        return path;
    }

    public static string FormatToSongPath(string path){
        char[] invalidChars = new char[] {'~', '&', '\\', ';', ':', '<', '>', '#'};
        char[] hideChars = new char[] {'~', '/', '[', '.', '\'', '%', '?', '!', ']'};
        path = string.Join("-", path.Replace(' ', '-').Split(invalidChars));
        return string.Join("", path.Split(hideChars)).ToLower();
    }
}
