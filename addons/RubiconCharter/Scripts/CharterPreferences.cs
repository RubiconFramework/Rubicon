namespace Charter.Scripts;

/// <summary>
/// This class holds chart editor preferences such as
/// chart editor settings, recent projects among others. (not planned yet)
/// The file will get saved in:
/// user://charter/
/// </summary>
[GlobalClass] public partial class CharterPreferences : Resource
{
    [Export] public bool ShowWelcomeWindow = true;
}

[Tool] public class CharterPreferenceManager
{
    public string FilePath = "res://addons/RubiconCharter/preferences.tres";
    public CharterPreferences Preferences = new();

    public void Load()
    {
        if (!ResourceLoader.Exists(FilePath))
        {
            Save();
            return;
        }

        Resource _preferences = ResourceLoader.Load<Resource>(FilePath);
        Preferences = _preferences as CharterPreferences;
    }

    public void Save()
    {
        ResourceSaver.Save(Preferences, FilePath);
    }
}
