using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace Rubicon.Core.Chart;

/// <summary>
/// The general chart format for this engine.
/// </summary>
[GlobalClass]
public partial class HoloChart : RefCounted
{
    #region Public Variables

    /// <summary>
    /// A list of BPM changes.
    /// </summary>
    public BpmInfo[] BpmInfo = System.Array.Empty<BpmInfo>();

    /// <summary>
    /// The default scroll speed for this chart.
    /// </summary>
    public float ScrollSpeed = 1.6f;

    /// <summary>
    /// The chart offset.
    /// </summary>
    public double Offset;

    /// <summary>
    /// The individual charts (or "strum lines") that each contain its own notes.
    /// </summary>
    public IndividualChart[] Charts = System.Array.Empty<IndividualChart>();

    #endregion

    #region Public Methods

    #region Parsing and Converting

    /// <summary>
    /// Serializes this HoloChart instance into a Godot Dictionary.
    /// </summary>
    /// <returns>Itself, as a Godot Dictionary.</returns>
    public Dictionary Serialize()
    {
        Dictionary serializedInfo = new Dictionary()
        {
            { "ScrollSpeed", ScrollSpeed },
            { "Offset", Offset }
        };

        Array<Dictionary> serializedBpmInfo = new Array<Dictionary>();
        for (int i = 0; i < BpmInfo.Length; i++)
            serializedBpmInfo.Add(BpmInfo[i].Serialize());

        Array<Dictionary> serializedCharts = new Array<Dictionary>();
        for (int i = 0; i < Charts.Length; i++)
            serializedCharts.Add(Charts[i].Serialize());

        serializedInfo.Add("BpmInfo", serializedBpmInfo);
        serializedInfo.Add("Charts", serializedCharts);

        return serializedInfo;
    }

    /// <summary>
    /// Converts a Godot Dictionary into a HoloChart instance.
    /// </summary>
    /// <param name="info">The provided Godot Dictionary</param>
    /// <returns>The newly created HoloChart</returns>
    public static HoloChart Deserialize(Dictionary info)
    {
        HoloChart chart = new HoloChart()
        {
            ScrollSpeed = info["ScrollSpeed"].AsSingle(),
            Offset = info["Offset"].AsDouble()
        };

        Array<Dictionary> bpmInfo = info["BpmInfo"].AsGodotArray<Dictionary>();
        chart.BpmInfo = new BpmInfo[bpmInfo.Count];
        for (int i = 0; i < bpmInfo.Count; i++)
            chart.BpmInfo[i] = Chart.BpmInfo.Deserialize(bpmInfo[i]);
            
        Array<Dictionary> chartInfo = info["Charts"].AsGodotArray<Dictionary>();
        chart.Charts = new IndividualChart[chartInfo.Count];
        for (int i = 0; i < chartInfo.Count; i++)
            chart.Charts[i] = IndividualChart.Deserialize(chartInfo[i]);

        return chart.ConvertData();
    }

    /// <summary>
    /// Converts this HoloChart instance into JSON format.
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
    /// Attempts to parse the string provided and returns an instance of HoloChart if successful.
    /// </summary>
    /// <param name="jsonString">The JSON string</param>
    /// <returns>The HoloChart if successful.</returns>
    public static HoloChart ParseString(string jsonString)
    {
        Dictionary parsedInfo = Json.ParseString(jsonString).AsGodotDictionary();
        HoloChart chart = Deserialize(parsedInfo);
        parsedInfo.Dispose();

        return chart;
    }
    #endregion

    /// <summary>
    /// Converts the notes in this chart to millisecond format.
    /// </summary>
    /// <returns>Itself</returns>
    public HoloChart ConvertData()
    {
        for (int i = 1; i < BpmInfo.Length; i++)
            BpmInfo[i].MsTime = BpmInfo[i - 1].MsTime + ConductorUtility.MeasureToMs(BpmInfo[i].Time - BpmInfo[i - 1].Time, BpmInfo[i - 1].Bpm, BpmInfo[i].TimeSignatureNumerator);

        for (int i = 0; i < Charts.Length; i++)
        for (int n = 0; n < Charts[i].Notes.Length; n++)
            Charts[i].Notes[n].ConvertData(BpmInfo);

        return this;
    }

    /// <summary>
    /// Sorts the notes properly and attempts to get rid of any duplicate notes and notes inside holds.
    /// </summary>
    public void Format()
    {
        for (int c = 0; c < Charts.Length; c++)
        {
            List<NoteData> notes = new List<NoteData>();

            for (int l = 0; l < Charts[c].Lanes; l++)
            {
                List<NoteData> lane = Charts[c].Notes.Where(x => x.Lane == l).ToList();
                lane.Sort((x, y) =>
                {
                    if (x.Time < y.Time)
                        return -1;
                    if (x.Time > y.Time)
                        return 1;

                    return 0;
                });

                for (int i = 0; i < lane.Count - 1; i++)
                {
                    if (lane[i].Length > 0)
                    {
                        double start = lane[i].Time;
                        double end = lane[i].Time + lane[i].Length;
                        while (i < lane.Count - 1 && lane[i + 1].Time >= start && lane[i + 1].Time < end)
                        {
                            GD.Print(
                                $"Removed note inside hold note area at {lane[i + 1].Time} in lane {l} ({start}-{end})");
                            lane.RemoveAt(i + 1);
                        }
                    }

                    while (i < lane.Count - 1 && lane[i + 1].Time == lane[i].Time)
                    {
                        GD.Print($"Removed duplicate note at {lane[i + 1].Time} in lane {l}");
                        lane.RemoveAt(i + 1);
                    }
                }

                notes.AddRange(lane);
            }

            notes.Sort((x, y) =>
            {
                if (x.Time < y.Time)
                    return -1;
                if (x.Time > y.Time)
                    return 1;

                if (x.Lane < y.Lane)
                    return -1;
                if (x.Lane > y.Lane)
                    return 1;

                return 0;
            });

            Charts[c].Notes = notes.ToArray();
        }
    }
    #endregion
}