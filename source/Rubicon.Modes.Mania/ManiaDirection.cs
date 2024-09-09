namespace Rubicon.Modes.Mania;

/// <summary>
/// Resource used to specify directions for a <see cref="ManiaNoteSkin"/>.
/// </summary>
[GlobalClass]
public partial class ManiaDirection : Resource
{
    /// <summary>
    /// Lane count for these directions.
    /// </summary>
    [Export] public int LaneCount = 4;

    /// <summary>
    /// The direction names.
    /// </summary>
    [Export] public string[] Directions = ["left", "down", "up", "right"];
}