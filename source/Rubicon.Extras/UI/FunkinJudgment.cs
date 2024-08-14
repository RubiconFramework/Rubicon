using Godot;
using Promise.Framework;
using Promise.Framework.UI;
using Promise.Framework.Utilities;

namespace Rubicon.Extras.UI;

/// <summary>
/// Overrides the original Promise.Framework Judgment with animations more Funkin'-like.
/// </summary>
public partial class FunkinJudgment : Judgment
{
    #region Public Variables
    /// <summary>
    /// Determines whether this FunkinJudgment instance is a child of another.
    /// </summary>
    public bool IsChildJudgment = false;

    /// <summary>
    /// The current velocity of this FunkinJudgment, in pixels.
    /// </summary>
    public Vector2 Velocity = Vector2.Zero;
    #endregion

    #region Private Variables
    private bool _missedCombo = false;
    private FunkinJudgment _baseJudgment;
    #endregion
    
    #region Public Methods
    /// <summary>
    /// Overriden Play function that applies Funkin-like animations.
    /// </summary>
    /// <param name="rating">The current rating</param>
    public override void Play(NoteHitType rating)
    {
        if (_missedCombo && rating == NoteHitType.Miss)
            return;
        
        if (_baseJudgment == null)
            _baseJudgment = (FunkinJudgment)Duplicate();
        
        FunkinJudgment newJudge = (FunkinJudgment)_baseJudgment.Duplicate();
        newJudge.AnimationPlayer = newJudge.GetChildOfType<AnimationPlayer>();
        newJudge.IsChildJudgment = true;
        newJudge.Size = Vector2.One;
        newJudge.Position = Vector2.Zero;
        newJudge.Velocity = new Vector2(GD.RandRange(0, 25), GD.RandRange(-262, -52));
        newJudge.PlayAnimation(rating);
        AddChild(newJudge);

        Tween fadeTween = newJudge.CreateTween();
        fadeTween.TweenProperty(newJudge, "self_modulate", Colors.Transparent, 0.2).SetDelay(60d / Conductor.Instance.Bpm);
        fadeTween.Finished += () => newJudge.QueueFree();
        fadeTween.Play();

        _missedCombo = rating == NoteHitType.Miss;
    }

    /// <summary>
    /// Handles all Judgment moving.
    /// </summary>
    /// <param name="delta">The time passed from the last frame, in seconds.</param>
    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!IsChildJudgment)
            return;

        Position += Velocity * (float)delta;
        Velocity = new Vector2(Velocity.X, Velocity.Y + 825f * (float)delta);
    }

    /// <summary>
    /// The default Play function, moved to a different function.
    /// </summary>
    /// <param name="rating">The current rating</param>
    public void PlayAnimation(NoteHitType rating)
    {
        string anim = Perfect;
        switch (rating)
        {
            case NoteHitType.Great: anim = Great;   break;
            case NoteHitType.Good:  anim = Good;    break;
            case NoteHitType.Bad:   anim = Bad;     break;
            case NoteHitType.Miss:  anim = Miss;    break;
        }

        if (AnimationPlayer == null)
            return;
            
        AnimationPlayer.Play(anim);
        AnimationPlayer.Seek(0, true);
    }
    #endregion
}