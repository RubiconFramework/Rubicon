using Rubicon.Core.Autoload;

namespace Rubicon.Core;

/// <summary>
/// A static link to the global instance of <see cref="UserSettingsInstance"/>.
/// </summary>
public static class UserSettings
{
    /// <summary>
    /// The global instance of the <see cref="UserSettingsInstance"/> instance.
    /// </summary>
    public static UserSettingsInstance Singleton;

    /// <inheritdoc cref="UserSettingsInstance.DownScroll" />
    public static bool DownScroll
    {
        get => Singleton.DownScroll;
        set => Singleton.DownScroll = value;
    }

    public static void SetValue(string section, string key, Variant value = default) =>
        Singleton.SetValue(section, key, value);
}
