namespace Rubicon.Core.Data;

[GlobalClass]
public partial class ClientSettings : Node
{
    /// <summary>
    /// The Autoloaded Settings instance.
    /// </summary>
    public static ClientSettings Settings { get; } = new ClientSettings();
    
    /// <summary>
    /// The individual setting types, you can access them thanks to the autoloaded settings instance.
    /// So for example. you can do ClientSettings.Gameplay.Downscroll
    /// </summary>
    public static GameplaySettings Gameplay = Settings._gameplaySettings;
    
    /// <summary>
    /// The instances of the setting types, you cant access those. x3
    /// </summary>
    private GameplaySettings _gameplaySettings = new GameplaySettings();
}

public class GameplaySettings
{
    public bool Downscroll = false;
}
