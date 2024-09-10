namespace Rubicon.Data;

public partial class ClientSettings : Resource
{
    // sorry duo you'll have to change this a bit for it to work
    // since this is a resource now



    /// <summary>
    /// The Autoloaded Settings instance.
    /// </summary>
    //public static ClientSettings Settings { get; } = new ClientSettings();

    /// <summary>
    /// The individual setting types, you can access them thanks to the autoloaded settings instance.
    /// So for example. you can do ClientSettings.Gameplay.Downscroll
    /// </summary>
    //public static GameplaySettings Gameplay = Settings._gameplaySettings;

    /// <summary>
    /// The instances of the setting types, you cant access those. x3
    /// </summary>
    [Export] public GameplaySettings GameplaySettings = new GameplaySettings();

}

public partial class GameplaySettings : Resource
{
    [Export] public bool Downscroll = false;
}
