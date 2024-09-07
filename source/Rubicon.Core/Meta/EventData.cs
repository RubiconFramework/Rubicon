using System.Linq;
using Godot.Collections;
using Rubicon.Core.Chart;

namespace Rubicon.Core.Meta;

[GlobalClass]
public partial class EventData : Resource
{
    /// <summary>
    /// The time to trigger this event. Stored on disk in measures.
    /// </summary>
    [Export] public double Time = 0d;
        
    /// <summary>
    /// The event's name.
    /// </summary>
    [Export] public string Name = "normal";

    /// <summary>
    /// The event's arguments.
    /// </summary>
    [Export] public Array Arguments = new();
        
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

        MsTime = ConductorUtility.MeasureToMs(Time - bpm.Time, bpm.Bpm, bpm.TimeSignatureNumerator) + bpm.MsTime;
    }
}