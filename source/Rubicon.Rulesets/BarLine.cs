using Rubicon.Core;
using Rubicon.Core.Chart;

namespace Rubicon.Modes;

public partial class BarLine : Control
{
    /// <summary>
    /// The individual chart for this bar line. Contains notes and scroll velocity changes.
    /// </summary>
    [Export] public IndividualChart Chart;
    
    /// <summary>
    /// Contains all the nodes used to manage notes.
    /// </summary>
    [Export] public NoteManager[] Managers;

    /// <summary>
    /// The distance to offset notes by position-wise.
    /// </summary>
    [Export] public float DistanceOffset = 0;
    
    /// <summary>
    /// The index of the current scroll velocity.
    /// </summary>
    [Export] public int ScrollVelocityIndex = 0;
    
    /// <summary>
    /// A signal that is emitted every time a manager in this bar line hits a note successfully.
    /// </summary>
    [Signal] public delegate void NoteHitEventHandler(BarLine barLine, int lane, NoteData noteData, double distance, bool holding);
    
    /// <summary>
    /// A signal that is emitted every time a manager in this bar line misses a note.
    /// </summary>
    [Signal] public delegate void NoteMissEventHandler(BarLine barLine, int lane, NoteData noteData, double distance);
    
    public override void _Process(double delta)
    {
        base._Process(delta);
        
        // Handle SV changes
        if (Chart?.SvChanges == null)
            return;
        
        double time = Conductor.Time * 1000d;
        if (ScrollVelocityIndex < Chart.SvChanges.Length)
            while (ScrollVelocityIndex < Chart.SvChanges.Length && Chart.SvChanges[ScrollVelocityIndex].MsTime - time <= 0)
                ScrollVelocityIndex++;
        
        SvChange currentScrollVel = Chart.SvChanges[ScrollVelocityIndex];
        DistanceOffset = -(float)(currentScrollVel.Position + (Conductor.Time - currentScrollVel.MsTime) * currentScrollVel.Multiplier);
    }
}