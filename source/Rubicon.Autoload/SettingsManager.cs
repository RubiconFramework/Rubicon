using Rubicon.Data;

namespace Rubicon.Autoload;
public partial class SettingsManager : Node
{
    public const string filePath = "user://settings.tres";
    public static ClientSettings clientSettings = new();

    public override void _Ready()
    {
        base._Ready();

        //test
        //clientSettings.GameplaySettings.Downscroll = true;
        //SaveSettings();
        //

        ReadSettings();
    }

    public static void ReadSettings()
    {
        if (!ResourceLoader.Exists(filePath))
            SaveSettings();

        clientSettings = GD.Load<ClientSettings>(filePath);
    }

    public static void SaveSettings()
    {
        ResourceSaver.Save(clientSettings, filePath);
        GD.Print($"Succesfully saved settings to file: {filePath}");
    }
}
