using Godot;
using Godot.Collections;
using Promise.Framework.Chart;
using Promise.Framework.Utilities;
using Array = System.Array;

namespace Rubicon.Data.Events;

/// <summary>
/// Contains multiple EventData instances for use in-game.
/// </summary>
[GlobalClass]
public partial class ChartEvents : RefCounted
{
    /// <summary>
    /// The individual events stored.
    /// </summary>
    public EventData[] Events = Array.Empty<EventData>();
    
    /// <summary>
    /// Converts the events in this chart to millisecond format.
    /// </summary>
    /// <param name="bpmList">A list of bpm changes</param>
    /// <returns>Itself</returns>
    public ChartEvents ConvertData(BpmInfo[] bpmList)
    {
        for (int i = 1; i < bpmList.Length; i++)
            bpmList[i].MsTime = bpmList[i - 1].MsTime + ConductorUtil.MeasureToMs(bpmList[i].Time - bpmList[i - 1].Time, bpmList[i - 1].Bpm, bpmList[i].TimeSignatureNumerator);

        for (int i = 0; i < Events.Length; i++)
            Events[i].ConvertData(bpmList);

        return this;
    }
        
    #region JSON Methods
    /// <summary>
    /// Serializes this ChartEvents instance into a Godot Dictionary.
    /// </summary>
    /// <returns>Itself, as a Dictionary.</returns>
    public Dictionary Serialize()
    {
        Dictionary serializedInfo = new Dictionary();

        Array<Dictionary> serializedEvents = new Array<Dictionary>();
        for (int i = 0; i < Events.Length; i++)
            serializedEvents.Add(Events[i].Serialize());

        serializedInfo.Add("Events", serializedEvents);

        return serializedInfo;
    }

    /// <summary>
    /// Converts a Godot Dictionary into an ChartEvents instance.
    /// </summary>
    /// <param name="info">The provided Godot Dictionary</param>
    /// <returns>The newly created ChartEvents</returns>
    public static ChartEvents Deserialize(Dictionary info)
    {
        ChartEvents chart = new ChartEvents();

        Array<Dictionary> eventInfo = info["Events"].AsGodotArray<Dictionary>();
        chart.Events = new EventData[eventInfo.Count];
        for (int i = 0; i < eventInfo.Count; i++)
            chart.Events[i] = EventData.Deserialize(eventInfo[i]);
            
        return chart;
    }
        
    /// <summary>
    /// Converts this ChartEvents instance into JSON format.
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
    /// Attempts to parse the string provided and returns an instance of ChartEvents if successful.
    /// </summary>
    /// <param name="jsonString">The JSON string</param>
    /// <returns>The ChartEvents if successful.</returns>
    public static ChartEvents ParseString(string jsonString)
    {
        Dictionary parsedInfo = Json.ParseString(jsonString).AsGodotDictionary();
        ChartEvents events = Deserialize(parsedInfo);
        parsedInfo.Dispose();

        return events;
    }
    #endregion
}