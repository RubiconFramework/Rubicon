using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Rubicon.Core.Meta;

/// <summary>
/// Used to hold important information about a song.
/// </summary>
[GlobalClass]
public partial class SongMeta : Resource
{
    /// <summary>
    /// The name of the song.
    /// </summary>
    [Export] public string Name = "";

    /// <summary>
    /// The artist who made the song.
    /// </summary>
    [Export] public string Artist = "";

    /// <summary>
    /// The icon that's associated with this song.
    /// </summary>
    [Export] public Texture2D Icon;

    /// <summary>
    /// Tells the game whether to look for vocals or not.
    /// </summary>
    [Export] public bool UseVocals = false;

    /// <summary>
    /// The characters to spawn in the song.
    /// </summary>
    [Export] public CharacterMeta[] Characters = [];

    /// <summary>
    /// The events to play in a song.
    /// </summary>
    [Export] public EventData[] Events = [];

    /// <summary>
    /// If turned on when the song loads, the game will load into a 3D stage instead of a 2D one. Only use when you need to, 3D spaces can be expensive on the computer.
    /// </summary>
    [Export] public bool Enable3D = false;
        
    /// <summary>
    /// The stage to spawn in for this song.
    /// </summary>
    [Export] public string Stage = "stage";

    /// <summary>
    /// The UI style to use for this song.
    /// </summary>
    [Export] public string UiStyle = ProjectSettings.GetSetting("rubicon/general/default_ui_style").AsString();

    /// <summary>
    /// The Note Skin to use for this song.
    /// </summary>
    [Export] public string NoteSkin = ProjectSettings.GetSetting("rubicon/rulesets/mania/default_note_skin").AsString();
        
    /// <summary>
    /// Locks Opponent and Speaker mode out.
    /// </summary>
    [ExportSubgroup("Focused Indexes"), Export] public bool OnlyPlayerMode = false;
    
    /// <summary>
    /// The index for which chart to select to be the opponent (sometimes referred as the Dad).
    /// </summary>
    [Export] public int OpponentChartIndex = 0;
        
    /// <summary>
    /// The index for which chart to select to be playable (sometimes referred as the BF).
    /// </summary>
    [Export] public int PlayerChartIndex = 1;
        
    /// <summary>
    /// The index for which chart acts as the speaker character (sometimes referred as the GF).
    /// </summary>
    [Export] public int SpeakerChartIndex = 2;
}