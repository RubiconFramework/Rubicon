using Rubicon.Autoload;
using Rubicon.Core.UI;
using Rubicon.Game;
using Rubicon.Rulesets;

namespace Rubicon.Extras.UI;

/// <summary>
/// A HitDistance class that mimics the animations from Dance Dance Revolution.
/// </summary>
public partial class DdrHitDistance : HitDistance
{
    /// <summary>
    /// The base scale for this hit distance.
    /// </summary>
    [Export] public Vector2 GraphicScale = Vector2.One;
    
    private Tween _labelTween;
    private Vector2 _offset = Vector2.Zero;
    
    /// <inheritdoc/>
    public override void Play(Vector2? offset)
    {
        if (RubiconGame.Instance != null && RubiconGame.Instance.PlayField != null)
        {
            PlayField playField = RubiconGame.Instance.PlayField;
            BarLine barLine = playField.BarLines[playField.TargetBarLine];
            _offset = offset ?? Vector2.Zero;

            Vector2 pos = barLine.GlobalPosition + (_offset * (Settings.General.Downscroll ? -1f : 1f));
            Play(barLine.AnchorLeft, barLine.AnchorTop, barLine.AnchorRight, barLine.AnchorBottom, pos);
            return;
        }
        
        Play(0f, 0f, 0f, 0f, offset);
    }
    
    /// <inheritdoc/>
    public override void Play(float anchorLeft, float anchorTop, float anchorRight, float anchorBottom, Vector2? pos)
    {
        _labelTween?.Kill();

        Label.AnchorLeft = anchorLeft;
        Label.AnchorTop = anchorTop;
        Label.AnchorRight = anchorRight;
        Label.AnchorBottom = anchorBottom;
        Label.PivotOffset = Label.Size / 2f;
        Label.Position = (pos ?? Vector2.Zero) - Label.PivotOffset;
        Label.Modulate = new Color(Label.Modulate.R, Label.Modulate.G, Label.Modulate.B);
        Label.Scale = GraphicScale * 1.1f;

        _labelTween = Label.CreateTween();
        _labelTween.TweenProperty(Label, "scale", GraphicScale, 0.1d);
        _labelTween.TweenProperty(Label, "modulate", Colors.Transparent, 0.5d).SetDelay(1d);
        _labelTween.Play();
    }

    public override void _Process(double delta)
    {
        if (RubiconGame.Instance == null || RubiconGame.Instance.PlayField == null || Label == null)
            return;
        
        PlayField playField = RubiconGame.Instance.PlayField;
        BarLine barLine = playField.BarLines[playField.TargetBarLine];
        Label.Position = barLine.GlobalPosition + (_offset * (Settings.General.Downscroll ? -1f : 1f)) - Label.PivotOffset;
    }
}