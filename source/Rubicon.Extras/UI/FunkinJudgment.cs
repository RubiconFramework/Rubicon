using System.Linq;
using Godot.Collections;
using Rubicon.Core;
using Rubicon.Core.Data;
using Rubicon.Core.UI;

namespace Rubicon.Extras.UI;

/// <summary>
/// A <see cref="Judgment"/> class that mimics the animation style of Friday Night Funkin'.
/// </summary>
public partial class FunkinJudgment : Judgment
{
    private bool _missedCombo = false;
    private Array<TextureRect> _judgmentGraphics = new();
    private Dictionary<TextureRect, Vector2> _judgmentVelocities = new();

    /// <inheritdoc/>
    public override void Play(HitType type, Vector2? offset)
    {
        Play(type, 0.5f, 0.5f, 0.5f, 0.5f, new Vector2((Size.X * 0.474f) - 60f, (Size.Y * 0.45f) - 90f) + offset);
    }

    /// <inheritdoc/>
    public override void Play(HitType type, float anchorLeft, float anchorTop, float anchorRight, float anchorBottom, Vector2? pos)
    {
        if (_missedCombo && type == HitType.Miss)
            return;

        TextureRect judgment = _judgmentGraphics.FirstOrDefault(x => x.Modulate.A == 0);
        if (judgment == null)
        {
            judgment = new TextureRect();
            judgment.Name = $"Instance {_judgmentGraphics.Count}";
            _judgmentGraphics.Add(judgment);
            AddChild(judgment);
        }
        
        judgment.AnchorLeft = anchorLeft;
        judgment.AnchorTop = anchorTop;
        judgment.AnchorRight = anchorRight;
        judgment.AnchorBottom = anchorBottom;
        judgment.Modulate = new Color(judgment.Modulate.R, judgment.Modulate.G, judgment.Modulate.B);
        judgment.Texture = GetJudgmentTexture(type);
        judgment.Material = GetJudgmentMaterial(type);
        judgment.Size = judgment.Texture.GetSize();
        judgment.Scale = GraphicScale;
        judgment.Position = pos ?? Vector2.Zero;
        judgment.MoveToFront();
        
        _judgmentVelocities[judgment] = new Vector2(GD.RandRange(0, 25), GD.RandRange(-262, -52));
        Tween fadeTween = judgment.CreateTween();
        fadeTween.TweenProperty(judgment, "modulate", Colors.Transparent, 0.2).SetDelay(60d / Conductor.Bpm);
        fadeTween.Play();
        
        _missedCombo = type == HitType.Miss;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        for (int i = 0; i < _judgmentGraphics.Count; i++)
        {
            TextureRect curGraphic = _judgmentGraphics[i];
            Vector2 curVelocity = _judgmentVelocities[curGraphic];
            if (curGraphic.Modulate.A == 0)
                continue;
            
            curGraphic.Position += curVelocity * (float)delta;
            _judgmentVelocities[curGraphic] = new Vector2(curVelocity.X, curVelocity.Y + 825f * (float)delta);
        }
    }
}