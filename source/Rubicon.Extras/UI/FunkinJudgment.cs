using System.Linq;
using Godot.Collections;
using Rubicon.Core;
using Rubicon.Core.Data;
using Rubicon.Core.UI;

namespace Rubicon.Extras.UI;

[GlobalClass]
public partial class FunkinJudgment : Judgment
{
    private bool _missedCombo = false;
    private Array<TextureRect> _judgmentGraphics = new();
    private Dictionary<TextureRect, Vector2> _judgmentVelocities = new();
    
    public override void Play(HitType type)
    {
        if (_missedCombo && type == HitType.Miss)
            return;

        TextureRect judgment = _judgmentGraphics.FirstOrDefault(x => x.Modulate.A == 0) ?? new TextureRect();
        judgment.Modulate = new Color(judgment.Modulate.R, judgment.Modulate.G, judgment.Modulate.B);
        judgment.Position = Vector2.Zero;
        switch (type)
        {
            default:
                judgment.Texture = PerfectTexture;
                judgment.Material = PerfectMaterial;
                break;
            case HitType.Great:
                judgment.Texture = GreatTexture;
                judgment.Material = GreatMaterial;
                break;
            case HitType.Good:
                judgment.Texture = GoodTexture;
                judgment.Material = GoodMaterial;
                break;
            case HitType.Bad:
                judgment.Texture = BadTexture;
                judgment.Material = BadMaterial;
                break;
            case HitType.Miss:
                judgment.Texture = MissTexture;
                judgment.Material = MissMaterial;
                break;
        }
        
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