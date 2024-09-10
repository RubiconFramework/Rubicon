using Rubicon.Data;

namespace Rubicon.Autoload;
public partial class Settings : Node
{
    private static ClientSettings ClientSettings = new();
    public static GameplaySettings Gameplay = ClientSettings.GameplaySettings;
    public static VideoSettings Video = ClientSettings.VideoSettings;
    public static AudioSettings Audio = ClientSettings.AudioSettings;
    public static MiscSettings Misc = ClientSettings.MiscSettings;
    
    public override void _Ready()
    {
        base._Ready();
        ReadSettings();
    }

    public static void ReadSettings()
    {
        if (!ResourceLoader.Exists(Main.SettingsFilePath))
            SaveSettings();

        ClientSettings = GD.Load<ClientSettings>(Main.SettingsFilePath);
    }

    public static void SaveSettings()
    {
        ResourceSaver.Save(ClientSettings, Main.SettingsFilePath);
        GD.Print($"Succesfully saved settings to file: {Main.SettingsFilePath}");
    }
}
