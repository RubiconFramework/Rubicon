using Rubicon.Data;

namespace Rubicon.Autoload;
public partial class Settings : Node
{
    private const string SettingsFilePath = "user://settings.tres";
    
    public static ClientSettings ClientSettings { get; private set; } = new();
    
    public override void _Ready()
    {
        base._Ready();
        ReadSettings();
    }

    public static void ReadSettings()
    {
        if (!ResourceLoader.Exists(SettingsFilePath))
            SaveSettings();
        
        ClientSettings = GD.Load<ClientSettings>(SettingsFilePath);
    }

    public static void SaveSettings()
    {
        ResourceSaver.Save((Resource)ClientSettings, SettingsFilePath);
        GD.Print($"Succesfully saved settings to file: {SettingsFilePath}");
    }
}