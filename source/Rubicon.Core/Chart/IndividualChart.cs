using Godot;
using Godot.Collections;
using Array = System.Array;

namespace Rubicon.Core.Chart;

/// <summary>
/// A class to store data for individual charts (referred to as "strum lines" for some).
/// </summary>
[GlobalClass]
public partial class IndividualChart : RefCounted
{
    /// <summary>
    /// If enabled, the chart will visibly appear on the screen.
    /// </summary>
    public bool Visible = true;

    /// <summary>
    /// How many lanes this specific chart will have.
    /// </summary>
    public int Lanes = 4;

    /// <summary>
    /// Stores the data for all notes in an array. Is used to generate notes in-game.
    /// </summary>
    public NoteData[] Notes = Array.Empty<NoteData>();

    #region JSON Methods
    /// <summary>
    /// Serializes this IndividualChart instance into a Godot Dictionary.
    /// </summary>
    /// <returns>Itself, as a Dictionary.</returns>
    public Dictionary Serialize()
    {
        Dictionary serializedInfo = new Dictionary()
        {
            { "Visible", Visible },
            { "Lanes", Lanes }
        };

        Array<Dictionary> serializedNotes = new Array<Dictionary>();
        for (int i = 0; i < Notes.Length; i++)
            serializedNotes.Add(Notes[i].Serialize());

        serializedInfo.Add("Notes", serializedNotes);

        return serializedInfo;
    }

    /// <summary>
    /// Converts a Godot Dictionary into an IndividualChart instance.
    /// </summary>
    /// <param name="info">The provided Godot Dictionary</param>
    /// <returns>The newly created IndividualChart</returns>
    public static IndividualChart Deserialize(Dictionary info)
    {
        IndividualChart chart = new IndividualChart()
        {
            Visible = info["Visible"].AsBool(),
            Lanes = info["Lanes"].AsInt32()
        };

        Array<Dictionary> noteInfo = info["Notes"].AsGodotArray<Dictionary>();
        chart.Notes = new NoteData[noteInfo.Count];
        for (int i = 0; i < noteInfo.Count; i++)
            chart.Notes[i] = NoteData.Deserialize(noteInfo[i]);
            
        return chart;
    }
        
    /// <summary>
    /// Converts this CharacterChart instance into JSON format.
    /// </summary>
    /// <returns>Itself, in JSON format</returns>
    public string Stringify()
    {
        Dictionary serializedInfo = Serialize();
        string jsonString = Json.Stringify(serializedInfo);
        serializedInfo.Dispose();

        return jsonString;
    }
        
    /// <summary>
    /// Attempts to parse the string provided and returns an instance of IndividualChart if successful.
    /// </summary>
    /// <param name="jsonString">The JSON string</param>
    /// <returns>The IndividualChart if successful.</returns>
    public static IndividualChart ParseString(string jsonString)
    {
        Dictionary parsedInfo = Json.ParseString(jsonString).AsGodotDictionary();
        IndividualChart chart = Deserialize(parsedInfo);
        parsedInfo.Dispose();

        return chart;
    }
    #endregion
}