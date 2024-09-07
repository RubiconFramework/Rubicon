using System.Linq;
using Rubicon.Rulesets;

namespace Rubicon.Core.Chart;

/// <summary>
/// Keeps track of a scroll velocity change.
/// </summary>
public partial class SvChange : Resource
{
    /// <summary>
    /// The time to execute this scroll velocity change, in milliseconds.
    /// </summary>
    public double MsTime = 0d;

    /// <summary>
    /// The starting position of this scroll velocity change. Is set by <see cref="RubiChart.ConvertData">RubiChart.ConvertData</see>.
    /// </summary>
    public float Position = 0f;
    
    /// <summary>
    /// The measure to execute this scroll velocity change.
    /// </summary>
    [Export] public double Time = 0;

    /// <summary>
    /// The scroll velocity value
    /// </summary>
    [Export] public float Multiplier = 1f;

    public void ConvertData(BpmInfo[] bpmInfo, SvChange previousChange = null)
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
        if (previousChange != null)
            Position = (float)(previousChange.Position + (MsTime - previousChange.MsTime) * Multiplier);
    }
}