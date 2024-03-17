using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Rubicon.backend.autoload;
using Rubicon.backend.autoload.debug.ScreenNotifier;
using Rubicon.backend.common.enums;
using Rubicon.scenes.options.elements;
using FileAccess = Godot.FileAccess;

namespace Rubicon.backend.scripts;

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
