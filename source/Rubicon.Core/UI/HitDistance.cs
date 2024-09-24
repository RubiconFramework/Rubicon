namespace Rubicon.Core.UI;

/// <summary>
/// A control node that activates upon the player hitting a note, showing the hit distance from the actual note's time.
/// </summary>
public partial class HitDistance : Control
{
    /// <summary>
    /// The label that this HitDistance instance will change the text of.
    /// </summary>
    [Export] public Label Label;

    /// <summary>
    /// Changes the label's text and also calls the Play function.
    /// </summary>
    /// <param name="distance">The hit distance from the note time</param>
    /// <param name="offset">Where the label should be offset positionally</param>
    public virtual void Show(double distance, Vector2? offset)
    {
        Label.Text = $"{distance:0.00} ms";
        if (Math.Abs(distance) > ProjectSettings.GetSetting("rubicon/judgments/bad_hit_window").AsDouble())
            Label.Text = $"Too {(distance < 0 ? "late!" : "early!")}";
        
        Play(offset);
    }

    /// <summary>
    /// Changes the label's text and also calls the Play function.
    /// </summary>
    /// <param name="distance">The hit distance from the note time</param>
    /// <param name="pos">The starting position of the label</param>
    /// <param name="offset">Where the label should be offset positionally</param>
    public virtual void Show(double distance, Vector2 pos, Vector2? offset)
    {
        Label.Text = $"{distance:0.00} ms";
        if (Math.Abs(distance) > ProjectSettings.GetSetting("rubicon/judgments/bad_hit_window").AsDouble())
            Label.Text = $"Too {(distance < 0 ? "late!" : "early!")}";
        
        Play(offset);
    }
    
    /// <summary>
    /// Does a unique animation for the hit distance label. Can be overriden.
    /// </summary>
    /// <param name="offset">Where the label should be offset positionally</param>
    public virtual void Play(Vector2? offset)
    {
        Play(0.5f, 0f, 0.5f, 0f, offset);
    }
    
    /// <summary>
    /// Does a unique animation for the hit distance label. Can be overriden.
    /// </summary>
    /// <param name="anchorLeft">The left anchor (usually from 0 to 1)</param>
    /// <param name="anchorTop">The top anchor (usually from 0 to 1)</param>
    /// <param name="anchorRight">The right anchor (usually from 0 to 1)</param>
    /// <param name="anchorBottom">The bottom anchor (usually from 0 to 1)</param>
    /// <param name="offset">Where the label should be offset positionally</param>
    public virtual void Play(float anchorLeft, float anchorTop, float anchorRight, float anchorBottom, Vector2? offset)
    {
        
    }
}