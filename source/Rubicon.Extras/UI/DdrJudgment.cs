using Rubicon.Autoload;
using Rubicon.Core.Data;
using Rubicon.Core.UI;
using Rubicon.Game;
using Rubicon.Rulesets;
using Rubicon.Rulesets.Mania;

namespace Rubicon.Extras.UI;

public partial class DdrJudgment : Judgment
{
    [Export] public float Opacity = 0.5f;
    
    private TextureRect _judgmentGraphic;
    private Tween _judgeTween;
    
    public override void Play(HitType type, Vector2 offset)
    {
        if (RubiconGame.Instance != null && RubiconGame.Instance.PlayField != null)
        {
            PlayField playField = RubiconGame.Instance.PlayField;
            BarLine barLine = playField.BarLines[playField.TargetBarLine];

            Vector2 pos = barLine.GlobalPosition + (offset * (Settings.General.Downscroll ? -1f : 1f));
            Play(type, barLine.AnchorLeft, barLine.AnchorTop, barLine.AnchorRight, barLine.AnchorBottom, pos);
            return;
        }
        
        Play(type, 0, 0f, 0f, 0f, offset);
    }
    
    public override void Play(HitType type, float anchorLeft, float anchorTop, float anchorRight, float anchorBottom, Vector2? pos)
    {
        if (_judgmentGraphic == null)
        {
            _judgmentGraphic = new TextureRect();
            _judgmentGraphic.Name = "Judgment Graphic";
            AddChild(_judgmentGraphic);
        }
        _judgeTween?.Kill();

        _judgmentGraphic.AnchorLeft = anchorLeft;
        _judgmentGraphic.AnchorTop = anchorTop;
        _judgmentGraphic.AnchorRight = anchorRight;
        _judgmentGraphic.AnchorBottom = anchorBottom;
        _judgmentGraphic.Texture = GetJudgmentTexture(type);
        _judgmentGraphic.Material = GetJudgmentMaterial(type);
        _judgmentGraphic.Scale = GraphicScale * 1.1f;
        _judgmentGraphic.Size = _judgmentGraphic.Texture.GetSize();
        _judgmentGraphic.PivotOffset = _judgmentGraphic.Size / 2f;
        _judgmentGraphic.Position = (pos ?? Vector2.Zero) - _judgmentGraphic.PivotOffset;
        _judgmentGraphic.Modulate = new Color(_judgmentGraphic.Modulate.R, _judgmentGraphic.Modulate.G,
            _judgmentGraphic.Modulate.B, Opacity);

        _judgeTween = _judgmentGraphic.CreateTween();
        _judgeTween.TweenProperty(_judgmentGraphic, "scale", GraphicScale, 0.1d);
        _judgeTween.TweenProperty(_judgmentGraphic, "modulate", Colors.Transparent, 0.5d).SetDelay(0.4d);
        _judgeTween.Play();
    }
}