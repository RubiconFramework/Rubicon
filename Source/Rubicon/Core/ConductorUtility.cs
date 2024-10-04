using System.Linq;
using Rubicon.Core.Chart;

namespace Rubicon.Core;

/// <summary>
/// Helper functions for Conductor and time-based stuff.
/// </summary>
public static class ConductorUtility
{
    /// <summary>
    /// Converts a measure to milliseconds based on the number of beats in a measure.
    /// </summary>
    /// <param name="measure">The measure</param>
    /// <param name="bpm">The BPM</param>
    /// <param name="timeSignatureNumerator">The number of beats in a measure.</param>
    /// <returns>The measure, in milliseconds.</returns>
    public static double MeasureToMs(double measure, double bpm, double timeSignatureNumerator = 4d)
    {
        return measure * (60000d / (bpm / timeSignatureNumerator));
    }

    /// <summary>
    /// Converts milliseconds to measures based on a list of BPM changes.
    /// </summary>
    /// <param name="msTime">The time in milliseconds</param>
    /// <param name="bpmList">The bpm list (<see cref="RubiChart.ConvertData"/> needs to be invoked beforehand!)</param>
    /// <returns>The milliseconds, in measures</returns>
    public static double MsToMeasures(double msTime, BpmInfo[] bpmList)
    {
        BpmInfo bpm = bpmList.Last();
        for (int i = 0; i < bpmList.Length; i++)
        {
            if (bpmList[i].MsTime > msTime)
            {
                bpm = bpmList[i - 1];
                break;
            }
        }

        double measureValue = MeasureToMs(1, bpm.Bpm, bpm.TimeSignatureNumerator);
        double offset = msTime - bpm.MsTime;
        return offset / measureValue;
    }

    /// <summary>
    /// Converts measures to beats.
    /// </summary>
    /// <param name="measure">The measure.</param>
    /// <param name="timeSignatureNumerator">The number of beats in a measure.</param>
    /// <returns>The measure, in beats.</returns>
    public static double MeasureToBeats(double measure, double timeSignatureNumerator = 4d)
    {
        return measure * timeSignatureNumerator;
    }

    /// <summary>
    /// Converts measures to steps.
    /// </summary>
    /// <param name="measure">The measure</param>
    /// <param name="timeSignatureNumerator">The number of beats in a measure.</param>
    /// <param name="timeSignatureDenominator">The type of note which equals one beat.</param>
    /// <returns>The measure, in steps.</returns>
    public static double MeasureToSteps(double measure, double timeSignatureNumerator = 4d, double timeSignatureDenominator = 4d)
    {
        return measure * timeSignatureNumerator * timeSignatureDenominator;
    }

    /// <summary>
    /// Converts beats to steps.
    /// </summary>
    /// <param name="beats">The beats</param>
    /// <param name="timeSignatureDenominator">The type of note which equals one beat.</param>
    /// <returns>The beats, in steps.</returns>
    public static double BeatsToSteps(double beats, double timeSignatureDenominator = 4f)
    {
        return beats * timeSignatureDenominator;
    }

    /// <summary>
    /// Converts steps to measures.
    /// </summary>
    /// <param name="steps">The steps.</param>
    /// <param name="timeSignatureNumerator">The number of beats in a measure.</param>
    /// <param name="timeSignatureDenominator">The type of note which equals one beat.</param>
    /// <returns>The steps, in measures.</returns>
    public static double StepsToMeasures(double steps, double timeSignatureNumerator = 4d, double timeSignatureDenominator = 4d)
    {
        return steps / (timeSignatureNumerator * timeSignatureDenominator);
    }
}