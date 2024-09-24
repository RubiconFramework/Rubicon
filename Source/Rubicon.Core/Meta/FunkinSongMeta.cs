namespace Rubicon.Core.Meta;

/// <summary>
/// Used to hold important information for a song to load alongside a Funkin' stage.
/// </summary>
public partial class FunkinSongMeta : SongMeta
{
    /// <summary>
    /// The characters to spawn in the song.
    /// </summary>
    [Export] public CharacterMeta[] Characters = [];
    
    /// <summary>
    /// If turned on when the song loads, the game will load into a 3D stage instead of a 2D one. Only use when you need to, 3D spaces can be expensive on the computer.
    /// </summary>
    [Export] public bool Enable3D = false;

    /// <summary>
    /// The stage to spawn in for this song.
    /// </summary>
    [Export] public string Stage = "stage";
}