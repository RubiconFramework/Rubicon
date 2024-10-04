using System.Linq;
using Godot.Collections;
using Rubicon.Core;
using Rubicon.Core.Data;
using Rubicon.Core.UI;

namespace Rubicon.Extras.UI;

/// <summary>
/// A <see cref="Judgment"/> class that mimics the animation style of Friday Night Funkin'.
/// </summary>
public partial class FunkinJudgment : Control, IJudgment, IJudgmentMaterial
{
    /// <summary>
    /// Textures to fetch from when displaying judgments.
    /// </summary>
    [Export] public SpriteFrames Atlas;

    /// <summary>
    /// How much to scale the judgment graphics by.
    /// </summary>
    [Export] public Vector2 GraphicScale = Vector2.One;

    /// <inheritdoc/>
    public Material PerfectMaterial { get; set; } // dokibird glasses

    /// <inheritdoc/>
    public Material GreatMaterial { get; set; }

    /// <inheritdoc/>
    public Material GoodMaterial { get; set; }

    /// <inheritdoc/>
    public Material OkayMaterial { get; set; }
    
    /// <inheritdoc/>
    public Material BadMaterial { get; set; }

    /// <inheritdoc/>
    public Material MissMaterial { get; set; }
    
    private bool _missedCombo = false;
    private Array<Control> _judgmentGraphics = new();
    private Dictionary<Control, Vector2> _judgmentVelocities = new();

    /// <inheritdoc/>
    public void Play(HitType type, Vector2? offset)
    {
        Play(type, 0.5f, 0.5f, 0.5f, 0.5f, new Vector2((Size.X * 0.474f) - 60f, (Size.Y * 0.45f) - 90f) + offset);
    }

    /// <inheritdoc/>
    public void Play(HitType type, float anchorLeft, float anchorTop, float anchorRight, float anchorBottom, Vector2? pos)
    {
        if (_missedCombo && type == HitType.Miss)
            return;

        Control judgment = _judgmentGraphics.FirstOrDefault(x => x.Modulate.A == 0);
        if (judgment == null)
        {
            judgment = new Control();
            judgment.Name = $"Instance {_judgmentGraphics.Count}";
            AnimatedSprite2D judgmentGraphic = new AnimatedSprite2D();
            judgmentGraphic.Name = "Graphic";
            judgmentGraphic.Centered = false;
            
            judgment.AddChild(judgmentGraphic);
            _judgmentGraphics.Add(judgment);
            AddChild(judgment);
        }

        AnimatedSprite2D graphic = judgment.GetChild<AnimatedSprite2D>(0);
        judgment.AnchorLeft = anchorLeft;
        judgment.AnchorTop = anchorTop;
        judgment.AnchorRight = anchorRight;
        judgment.AnchorBottom = anchorBottom;
        judgment.Modulate = new Color(judgment.Modulate.R, judgment.Modulate.G, judgment.Modulate.B);
        judgment.Scale = GraphicScale;
        graphic.SpriteFrames = Atlas;
        graphic.Animation = type.ToString();
        graphic.Frame = 0;
        graphic.Play();
        graphic.Material = this.GetHitMaterial(type);
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
            Control curGraphic = _judgmentGraphics[i];
            Vector2 curVelocity = _judgmentVelocities[curGraphic];
            if (curGraphic.Modulate.A == 0)
                continue;
            
            curGraphic.Position += curVelocity * (float)delta;
            _judgmentVelocities[curGraphic] = new Vector2(curVelocity.X, curVelocity.Y + 825f * (float)delta);
        }
    }
}