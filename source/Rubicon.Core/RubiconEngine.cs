namespace Rubicon.Core;

public static class RubiconEngine
{
    public static uint Version => CreateVersion(0, 1, 0, 0);
    
    public static string SubVersion => "-doki";
    
    public static string VersionString => $"{(Version & 0xFF000000) >> 24}.{(Version & 0x00FF0000) >> 16}.{(Version & 0x0000FF00) >> 8}.{Version & 0x000000FF}{SubVersion}";
    
    public static uint CreateVersion(byte major, byte minor, byte patch, byte build)
    {
        return ((uint)major << 24) | ((uint)minor << 16) | ((uint)patch << 8) | build;
    }
}