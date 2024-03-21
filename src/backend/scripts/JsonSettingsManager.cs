using BaseRubicon.Backend.Autoload;
using BaseRubicon.Backend.Autoload.Debug.ScreenNotifier;
using BaseRubicon.Scenes.Options.Elements;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using FileAccess = Godot.FileAccess;

namespace BaseRubicon.Backend.Scripts;

public static class JsonSettingsManager
{
    public static SettingsData LoadSettingsFromFile(string path)
    {
        try
        {
            SettingsData settings;
            if (FileAccess.FileExists(path))
            {
                var jsonData = FileAccess.Open(path, FileAccess.ModeFlags.Read);
                string json = jsonData.GetAsText();
                settings = JsonConvert.DeserializeObject<SettingsData>(json);
                GD.Print($"Settings loaded from file. [{path}]");
            }
            else
            {
                GD.Print("Settings file not found. Writing default settings to file.");
                settings = SettingsData.GetDefaultSettings();
                var jsonSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Formatting = Formatting.Indented
                };
                string jsonData = JsonConvert.SerializeObject(settings, jsonSettings);
                using (var file = FileAccess.Open(path, FileAccess.ModeFlags.WriteRead)) file.StoreString(jsonData);
                GD.Print("Default settings written to file.");
            }

            Global.Settings = settings;
            return settings;
        }
        catch (Exception e)
        {
            GD.PrintErr($"Failed to load or write default settings: {e.Message}");
            return null;
        }
    }

    public static void SaveSettingsToFile(string path)
    {
        try
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };
            
            string jsonData = JsonConvert.SerializeObject(Global.Settings, settings);
            using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
            file.StoreString(jsonData);
        }
        catch (Exception e)
        {
            ScreenNotifier.Instance.Notify($"Failed to save settings: {e.Message}", true, NotificationType.Error);
        }
    }
}
