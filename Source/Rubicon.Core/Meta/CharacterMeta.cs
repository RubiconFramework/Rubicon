namespace Rubicon.Core.Meta;

[GlobalClass]
public partial class CharacterMeta : Resource
{
    /// <summary>
    /// The character to get.
    /// </summary>
    [Export] public string Character = "";

    /// <summary>
    /// The index of the bar line (strum line) selected.
    /// </summary>
    [Export] public int BarLineIndex = -1;

    /// <summary>
    /// The index of the place to spawn at on the stage.
    /// </summary>
    [Export] public int SpawnPointIndex = 0;
}