using Rubicon.Data;

namespace Rubicon.Autoload;
public partial class Settings : Node
{
    private const string SettingsFilePath = "user://settings.tres";
    
    public static GeneralSettings General { get; private set; } = new();
    
    public override void _Ready()
    {
        base._Ready();
        ReadSettings();
    }

    public static void ReadSettings()
    {
        if (!ResourceLoader.Exists(SettingsFilePath))
            SaveSettings();
        
        General = GD.Load<GeneralSettings>(SettingsFilePath);
    }

    public static void SaveSettings()
    {
        ResourceSaver.Save(General, SettingsFilePath);
        GD.Print($"Succesfully saved settings to file: {SettingsFilePath}");
    }
}
