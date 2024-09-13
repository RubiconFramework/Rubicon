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

    /// <summary>
    /// If false, this note is ready to be recycled.
    /// </summary>
    [Export] public bool Active = true;

    /// <summary>
    /// Whether this note has missed.
    /// </summary>
    [Export] public bool Missed = false;

    public override void _Process(double delta)
    {
        if (!Active)
            return;
        
        UpdatePosition();
    }
    
    /// <summary>
    /// Triggers when the note needs to update its position.
    /// </summary>
    public virtual void UpdatePosition()
    {
        
    }

    /// <summary>
    /// Triggers upon this note being recycled.
    /// </summary>
    public virtual void Reset()
    {
        Info = null;
        Active = true;
        Visible = true;
        Missed = false;
    }

    /// <summary>
    /// Triggers when the note needs to be prepared for recycling. (Ex: hitting a note)
    /// </summary>
    public virtual void PrepareRecycle()
    {
        Active = Visible = false;
        Info.HitObject = null;
    }
}