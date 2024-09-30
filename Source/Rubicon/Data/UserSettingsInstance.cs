using Godot.Collections;
using Rubicon.Data.Settings;
using Array = Godot.Collections.Array;

namespace Rubicon.Data;

public partial class UserSettingsInstance : Node
{
    private UserSettingsData _data = new();

    public void UpdateSettings()
    {
        
    }
    
    public Error Load(string path = null)
    {
        path ??= ProjectSettings.GetSetting("rubicon/general/settings_save_path").AsString();
        ConfigFile config = new();
        Error loadError = config.Load(path);
        if (loadError != Error.Ok)
            return loadError;

        _data.Load(config);
        return Error.Ok;
    }

    public Error Save(string path = null)
    {
        path ??= ProjectSettings.GetSetting("rubicon/general/settings_save_path").AsString();
        ConfigFile configFile = _data.CreateConfigFileInstance();
        return configFile.Save(path);
    }

    public void Reset()
    {
        _data = new UserSettingsData();
    }

    public Variant GetSetting(string key) => _data.GetSetting(key);

    public void SetSetting(string key, Variant value) => _data.SetSetting(key, value);
}