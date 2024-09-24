namespace Rubicon.Core;

public partial class RubiconEngine : Node
{
    public static RubiconEngine Instance;
    
    public static readonly uint Version = RubiconUtility.CreateVersion(0, 1, 0, 0);
    
    public static readonly string SubVersion = "-alpha";

    public static string VersionString => RubiconUtility.VersionToString(Version) + SubVersion;

    public override void _Ready()
    {
        if (Instance != null)
        {
            QueueFree();
            return;
        }

        Instance = this;
        
        // Override content scale size with the one in rubicon's
        GetWindow().ContentScaleSize = ProjectSettings.GetSetting("rubicon/general/content_minimum_size").AsVector2I();
    }
}