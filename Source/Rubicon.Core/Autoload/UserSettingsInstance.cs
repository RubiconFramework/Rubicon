namespace Rubicon.Core.Autoload;

/// <summary>
/// Contains info about the user's preferences.
/// </summary>
[GlobalClass]
public partial class UserSettingsInstance : Node
{
    /// <summary>
    /// A container for all the data to be stored in.
    /// </summary>
    [Export] public ConfigFile Data = new();

    private string _filePath;
    
    public UserSettingsInstance()
    {
        _filePath = "user://settings.cfg";

        if (!Load())
        {
            Reset();
            Save();
        }
    }
    
    public UserSettingsInstance(string filePath)
    {
        _filePath = filePath;

        if (!Load())
        {
            Reset();
            Save();
        }
    }

    /// <summary>
    /// If true, the notes' scroll direction will go down instead of up.
    /// </summary>
    [Export]
    public bool DownScroll
    {
        get => Data.GetValue("Gameplay", "DownScroll", false).AsBool();
        set => Data.SetValue("Gameplay", "DownScroll", value);
    }

    public bool Load()
    {
        if (FileAccess.FileExists(_filePath))
        {
            string configText = FileAccess.GetFileAsString(_filePath);
            if (Data.Parse(configText) == Error.Ok)
                return true;
        }

        return false;
    }

    public bool Save()
    {
        if (Data.Save(_filePath) == Error.Ok)
            return true;

        return false;
    }

    public void Reset()
    {
        // TODO: Add functionality to this.
    }

    public void SetValue(string section, string key, Variant value = default) => 
        Data.SetValue(section, key, value);
}
