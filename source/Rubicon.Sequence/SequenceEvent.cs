namespace Rubicon.Sequence;

/// <summary>
/// A base sequence event for other Rubicon SequenceEvents to inherit from
/// </summary>
[GlobalClass]
public partial class SequenceEvent : Resource
{
    /// <summary>
    /// Lets the sequence controller know when to move on to the next event.
    /// </summary>
    public bool Advance = true;

    /// <summary>
    /// Activates when the sequence controller first switches to this.
    /// </summary>
    /// <param name="controller">The controller</param>
    public virtual void Begin(SequenceController controller)
    {
        
    }
    
    /// <summary>
    /// An overridable function that updates every tick when the sequence controller reaches it.
    /// </summary>
    /// <param name="ticks">Ticks passed</param>
    /// <param name="controller">The controller</param>
    public virtual void Process(int ticks, SequenceController controller)
    {
        
    }
}