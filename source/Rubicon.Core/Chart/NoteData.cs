using System.Linq;
using Godot;
using Godot.Collections;

namespace Rubicon.Core.Chart;

/// <summary>
/// Used for storing a note's data values. Is not the actual Note object.
/// </summary>
[GlobalClass]
public partial class NoteData : RefCounted
{
    /// <summary>
    /// The note's lane.
    /// </summary>
    public int Lane;
        
    /// <summary>
    /// The note's type.
    /// </summary>
    public string Type = "normal";

    /// <summary>
    /// Starting point of the note. Stored on disk in measures.
    /// </summary>
    public double Time;

    /// <summary>
    /// Ending point of the note. Stored on disk in measures.
    /// </summary>
    public double Length;
        
    /// <summary>
    /// Length of the note converted to milliseconds. Should be ignored when serialized.
    /// </summary>
    public double MsTime;
        
    /// <summary>
    /// Length of the note converted to milliseconds. Should be ignored when serialized.
    /// </summary>
    public double MsLength;

    /// <summary>
    /// Basically tells the autoplay whether to miss this note or not. Should be ignored when serialized.
    /// </summary>
    public bool ShouldMiss = false;

    /// <summary>
    /// Converts the pre-existing Time and Length variables to milliseconds and stores them in MsTime and MsLength, using the provided BpmInfo.
    /// </summary>
    /// <param name="bpmInfo">An Array of BpmInfos</param>
    public void ConvertData(BpmInfo[] bpmInfo)
    {
        BpmInfo bpm = bpmInfo.Last();
        for (int i = 0; i < bpmInfo.Length; i++)
        {
            if (bpmInfo[i].Time > Time)
            {
                bpm = bpmInfo[i - 1];
                break;
            }
        }

        MsTime = ConductorUtility.MeasureToMs(Time - bpm.Time, bpm.Bpm, bpm.TimeSignatureNumerator) + bpm.MsTime;
        MsLength = ConductorUtility.MeasureToMs(Length, bpm.Bpm, bpm.TimeSignatureNumerator);
    }

    #region JSON Methods
    /// <summary>
    /// Serializes this NoteData instance into a Godot Dictionary.
    /// </summary>
    /// <returns>Itself, as a Dictionary</returns>
    public Dictionary Serialize() => new Dictionary()
    {
        { "Lane", Lane },
        { "Type", Type },
        { "Time", Time },
        { "Length", Length }
    };

    /// <summary>
    /// Converts a Godot Dictionary into a BpmInfo instance.
    /// </summary>
    /// <param name="info">The provided Godot Dictionary</param>
    /// <returns>The newly created NoteData</returns>
    public static NoteData Deserialize(Dictionary info) => new NoteData()
    {
        Lane = info["Lane"].AsInt32(),
        Type = info["Type"].AsString(),
        Time = info["Time"].AsDouble(),
        Length = info["Length"].AsDouble()
    };
        
    /// <summary>
    /// Converts this NoteData instance into JSON format.
    /// </summary>
    /// <returns>This NoteData instance in JSON format.</returns>
    public string Stringify()
    {
        Dictionary serializedInfo = Serialize();
        string jsonString = Json.Stringify(serializedInfo);
        serializedInfo.Dispose();
            
        return jsonString;
    }

    /// <summary>
    /// Attempts to parse the string provided and returns an instance of NoteData if successful.
    /// </summary>
    /// <param name="jsonString">The JSON string</param>
    /// <returns>The NoteData if successful.</returns>
    public static NoteData ParseString(string jsonString)
    {
        Dictionary parsedInfo = Json.ParseString(jsonString).AsGodotDictionary();
        NoteData noteData = Deserialize(parsedInfo);
        parsedInfo.Dispose();

        return noteData;
    }
    #endregion
}