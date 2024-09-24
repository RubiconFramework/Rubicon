using Rubicon.Autoload;
using Rubicon.Core.Data;
using Rubicon.Core.UI;
using Rubicon.Game;
using Rubicon.Rulesets;

namespace Rubicon.Extras.UI;

/// <summary>
/// A Judgment class that mimics the animations from Dance Dance Revolution.
/// </summary>
public partial class DdrJudgment : Judgment
{
    [Export] public float Opacity = 0.5f;
    
    private Control _judgmentControl;
    private AnimatedSprite2D _judgmentGraphic;
    private Tween _judgeTween;
    private Vector2 _offset = Vector2.Zero;
    
    /// <inheritdoc/>
    public override void Play(HitType type, Vector2? offset)
    {
        if (RubiconGame.Instance != null && RubiconGame.Instance.PlayField != null)
        {
            PlayField playField = RubiconGame.Instance.PlayField;
            BarLine barLine = playField.BarLines[playField.TargetBarLineIndex];
            _offset = offset ?? Vector2.Zero;

            Vector2 pos = barLine.GlobalPosition + (_offset * (Settings.General.Downscroll ? -1f : 1f));
            Play(type, barLine.AnchorLeft, barLine.AnchorTop, barLine.AnchorRight, barLine.AnchorBottom, pos);
            return;
        }
        
        Play(type, 0, 0f, 0f, 0f, offset);
    }
    
    /// <inheritdoc/>
    public override void Play(HitType type, float anchorLeft, float anchorTop, float anchorRight, float anchorBottom, Vector2? pos)
    {
        if (_judgmentGraphic == null)
        {
            _judgmentControl = new Control();
            _judgmentControl.Name = "Judgment Container";
            _judgmentGraphic = new AnimatedSprite2D();
            _judgmentGraphic.Name = "Graphic";
            
            _judgmentControl.AddChild(_judgmentGraphic);
            AddChild(_judgmentControl);
        }
        _judgeTween?.Kill();

        _judgmentControl.AnchorLeft = anchorLeft;
        _judgmentControl.AnchorTop = anchorTop;
        _judgmentControl.AnchorRight = anchorRight;
        _judgmentControl.AnchorBottom = anchorBottom;
        _judgmentGraphic.SpriteFrames = Atlas;
        _judgmentGraphic.Animation = GetJudgmentAnimation(type);
        _judgmentGraphic.Frame = 0;
        _judgmentGraphic.Play();
        _judgmentGraphic.Material = GetJudgmentMaterial(type);
        _judgmentControl.Scale = GraphicScale * 1.1f;
        _judgmentControl.Position = pos ?? Vector2.Zero;
        _judgmentControl.Modulate = new Color(_judgmentControl.Modulate.R, _judgmentControl.Modulate.G,
            _judgmentControl.Modulate.B, Opacity);

        _judgeTween = _judgmentControl.CreateTween();
        _judgeTween.TweenProperty(_judgmentControl, "scale", GraphicScale, 0.1d);
        _judgeTween.TweenProperty(_judgmentControl, "modulate", Colors.Transparent, 0.5d).SetDelay(0.4d);
        _judgeTween.Play();
    }
    
    public override void _Process(double delta)
    {
        if (RubiconGame.Instance == null || RubiconGame.Instance.PlayField == null || _judgmentControl == null || _judgmentGraphic == null)
            return;
        
        PlayField playField = RubiconGame.Instance.PlayField;
        BarLine barLine = playField.BarLines[playField.TargetBarLineIndex];
        _judgmentControl.Position = barLine.GlobalPosition + (_offset * (Settings.General.Downscroll ? -1f : 1f));
    }
}