namespace Rubicon.Core.Meta;

[GlobalClass]
public partial class CharacterMeta : Resource
{
    /// <summary>
    /// The character to get.
    /// </summary>
    [Export] public string Character = "";

    /// <summary>
    /// The name of the bar line (strum line) to link this character to.
    /// </summary>
    [Export] public string BarLine = "";

    /// <summary>
    /// The name of the spawn point to link this character to.
    /// </summary>
    [Export] public string SpawnPoint = "";
}