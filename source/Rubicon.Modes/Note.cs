using Rubicon.Core.Chart;

namespace Rubicon.Modes;

/// <summary>
/// A control node used to visualize a note meant to be hit on-screen.
/// </summary>
public partial class Note : Control
{
    /// <summary>
    /// Contains info about this note.
    /// </summary>
    [Export] public NoteData Info;

    /// <summary>
    /// If false, this note is ready to be recycled.
    /// </summary>
    [Export] public bool Active = true;

    /// <summary>
    /// 
    /// </summary>
    [Export] public bool Missed = false;

    public override void _Process(double delta)
    {
        if (!Active)
            return;
        
        UpdatePosition();
    }
    
    public virtual void UpdatePosition()
    {
        
    }
}