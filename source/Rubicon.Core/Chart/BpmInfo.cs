using System;
using Godot;
using Godot.Collections;

namespace Rubicon.Core.Chart;

/// <summary>
/// Keeps track of a BPM / time signature change.
/// </summary>
[GlobalClass]
public partial class BpmInfo : RefCounted
{
    /// <summary>
    /// The exact time this change happens in milliseconds. Should not be serialized.
    /// </summary>
    public double MsTime = 0d;

    /// <summary>
    /// The measure this change happens in.
    /// </summary>
    public double Time;
            
    /// <summary>
    /// The BPM to set with this change.
    /// </summary>
    public double Bpm;
        
    /// <summary>
    /// The number of beats in a measure with this change.
    /// </summary>
    public int TimeSignatureNumerator = 4;
        
    /// <summary>
    /// The type of note which equals one beat with this change.
    /// </summary>
    public int TimeSignatureDenominator = 4;

    #region JSON Methods
    /// <summary>
    /// Serializes this BpmInfo instance into a Godot Dictionary.
    /// </summary>
    /// <returns>This BpmInfo as a Godot Dictionary</returns>
    public Dictionary Serialize() => new Dictionary()
    {
        { "Time", Time },
        { "Bpm", Bpm },
        { "TimeSignatureNumerator", TimeSignatureNumerator },
        { "TimeSignatureDenominator", TimeSignatureDenominator }
    };

    /// <summary>
    /// Converts a Godot Dictionary into a BpmInfo instance.
    /// </summary>
    /// <param name="info">The provided Godot Dictionary</param>
    /// <returns>The newly created BpmInfo</returns>
    public static BpmInfo Deserialize(Dictionary info) => new BpmInfo()
    {
        Time = info["Time"].AsDouble(),
        Bpm = info["Bpm"].AsDouble(),
        TimeSignatureNumerator = info["TimeSignatureNumerator"].AsInt32(),
        TimeSignatureDenominator = info["TimeSignatureDenominator"].AsInt32()
    };
        
    /// <summary>
    /// Converts this BpmInfo instance into JSON format.
    /// </summary>
    /// <returns>This BpmInfo instance in JSON format.</returns>
    public string Stringify()
    {
        Dictionary serializableInfo = Serialize();
        string jsonString = Json.Stringify(serializableInfo);
        serializableInfo.Dispose();

        return jsonString;
    }

    /// <summary>
    /// Attempts to parse the string provided and returns an instance of BpmInfo if successful.
    /// </summary>
    /// <param name="jsonString">The JSON string</param>
    /// <returns>The BpmInfo if successful</returns>
    public static BpmInfo ParseString(string jsonString)
    {
        Dictionary parsedInfo = Json.ParseString(jsonString).AsGodotDictionary();
        BpmInfo bpmInfo = Deserialize(parsedInfo);
        parsedInfo.Dispose();
            
        return bpmInfo;
    }
    #endregion
}