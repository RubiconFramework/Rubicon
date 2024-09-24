using Rubicon.Core.Meta;

namespace Rubicon.Core.Chart;

/// <summary>
/// A class to store data for individual charts (referred to as "strum lines" for some).
/// </summary>
[GlobalClass]
public partial class IndividualChart : Resource
{
    /// <summary>
    /// What to name this bar-line. Used primarily for <see cref="SongMeta."/>
    /// </summary>
    [Export] public string Name = "Player";
    
    /// <summary>
    /// How many lanes this specific chart will have.
    /// </summary>
    [Export] public int Lanes = 4;

    /// <summary>
    /// Stores the data for all notes in an array. Is used to generate notes in-game.
    /// </summary>
    [Export] public NoteData[] Notes = [];

    /// <summary>
    /// Stores data about scroll velocity changes.
    /// </summary>
    [Export] public SvChange[] SvChanges = [ new SvChange() ];
}