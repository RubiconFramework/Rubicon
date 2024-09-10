using Rubicon.Data;

namespace Rubicon.Autoload;
public partial class SettingsManager : Node
{
    public static ClientSettings clientSettings = new();

    public override void _Ready()
    {
        base._Ready();
        ReadSettings();
    }

    public static void ReadSettings()
    {
        if (!ResourceLoader.Exists(Main.SettingsFilePath))
            SaveSettings();

        clientSettings = GD.Load<ClientSettings>(Main.SettingsFilePath);
    }

    public static void SaveSettings()
    {
        ResourceSaver.Save(clientSettings, Main.SettingsFilePath);
        GD.Print($"Succesfully saved settings to file: {Main.SettingsFilePath}");
    }
}
