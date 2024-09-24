namespace Rubicon.Core.Chart;

/// <summary>
/// Keeps track of a BPM / time signature change.
/// </summary>
[GlobalClass]
public partial class BpmInfo : Resource
{
    /// <summary>
    /// The exact time this change happens in milliseconds.
    /// </summary>
    public double MsTime = 0d;

    /// <summary>
    /// The measure this change happens in.
    /// </summary>
    [Export] public double Time;
            
    /// <summary>
    /// The BPM to set with this change.
    /// </summary>
    [Export] public double Bpm;
        
    /// <summary>
    /// The number of beats in a measure with this change.
    /// </summary>
    [Export] public int TimeSignatureNumerator = 4;
        
    /// <summary>
    /// The type of note which equals one beat with this change.
    /// </summary>
    [Export] public int TimeSignatureDenominator = 4;
}
