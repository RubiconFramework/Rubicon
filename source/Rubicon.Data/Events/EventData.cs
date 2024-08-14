using System.Linq;
using Godot;
using Godot.Collections;
using Promise.Framework.Chart;
using Promise.Framework.Utilities;
using Array = System.Array;

namespace Rubicon.Data.Events;

/// <summary>
/// Used for storing a note's data values. Is not the actual Note object.
/// </summary>
[GlobalClass]
public partial class EventData : RefCounted
{
    /// <summary>
    /// The time to trigger this event. Stored on disk in measures.
    /// </summary>
    public double Time = 0d;
        
    /// <summary>
    /// The event's name.
    /// </summary>
    public string Name = "normal";

    /// <summary>
    /// The event's arguments.
    /// </summary>
    public string[] Arguments = Array.Empty<string>();
        
    /// <summary>
    /// The time converted from measures to milliseconds. Should be ignored when serialized.
    /// </summary>
    public double MsTime = 0d;

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

        MsTime = ConductorUtil.MeasureToMs(Time - bpm.Time, bpm.Bpm, bpm.TimeSignatureNumerator) + bpm.MsTime;
    }

    #region JSON Methods
    /// <summary>
    /// Serializes this EventData instance into a Godot Dictionary.
    /// </summary>
    /// <returns>Itself, as a Dictionary</returns>
    public Dictionary Serialize() => new Dictionary()
    {
        { "Time", Time },
        { "Name", Name }
    };

    /// <summary>
    /// Converts a Godot Dictionary into a EventData instance.
    /// </summary>
    /// <param name="info">The provided Godot Dictionary</param>
    /// <returns>The newly created EventData</returns>
    public static EventData Deserialize(Dictionary info) => new EventData()
    {
        Time = info["Time"].AsDouble(),
        Name = info["Name"].AsString()
    };
        
    /// <summary>
    /// Converts this EventData instance into JSON format.
    /// </summary>
    /// <returns>This EventData instance in JSON format.</returns>
    public string Stringify()
    {
        Dictionary serializedInfo = Serialize();
        string jsonString = Json.Stringify(serializedInfo);
        serializedInfo.Dispose();
            
        return jsonString;
    }

    /// <summary>
    /// Attempts to parse the string provided and returns an instance of EventData if successful.
    /// </summary>
    /// <param name="jsonString">The JSON string</param>
    /// <returns>The EventData if successful.</returns>
    public static EventData ParseString(string jsonString)
    {
        Dictionary parsedInfo = Json.ParseString(jsonString).AsGodotDictionary();
        EventData eventData = Deserialize(parsedInfo);
        parsedInfo.Dispose();

        return eventData;
    }
    #endregion
}