using Rubicon.Core.Chart;

namespace Rubicon.Rulesets;

/// <summary>
/// A control node used to visualize a note meant to be hit on-screen.
/// </summary>
public partial class Note : Control
{
    /// <summary>
    /// Contains info about this note.
    /// </summary>
    [Export] public NoteData Info;

    public override void _Process(double delta)
    {
        base._Process(delta);
        
        UpdatePosition();
    }
    
    public virtual void UpdatePosition()
    {
        
    }
}