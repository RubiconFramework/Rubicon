using System.Linq;
using Godot.Collections;
using Rubicon.Core;
using Rubicon.Core.Data;
using Rubicon.Core.UI;

namespace Rubicon.Extras.UI;

/// <summary>
/// A <see cref="ComboDisplay"/> class that mimics the animation style of Friday Night Funkin'.
/// </summary>
public partial class FunkinComboDisplay : ComboDisplay
{
    private bool _wasZero = false;
    private Array<TextureRect> _comboGraphics = new();
    private Dictionary<TextureRect, Vector2> _comboVelocities = new();
    private Dictionary<TextureRect, int> _comboAccelerations = new();

    /// <inheritdoc/>
    public override void Play(uint combo, HitType type, Vector2? offset)
    {
        Play(combo, type, 0.5f, 0.5f, 0.5f, 0.5f, new Vector2((Size.X * 0.507f) - 97.5f, Size.Y * 0.48f) + offset);
    }
    
    /// <inheritdoc/>
    public override void Play(uint combo, HitType type, float anchorLeft, float anchorTop, float anchorRight, float anchorBottom, Vector2? pos)
    {
        if (combo == 0 && _wasZero)
            return;

        if (type > LastRating)
            LastRating = type;
        
        string comboString = combo.ToString("D3");
        int[] splitDigits = new int[comboString.Length];
        for (int i = 0; i < splitDigits.Length; i++)
            splitDigits[i] = int.Parse(comboString.ToCharArray()[i].ToString());
        
        float generalSize = Spacing;
        TextureRect[] currentGraphics = new TextureRect[splitDigits.Length];
        for (int i = 0; i < splitDigits.Length; i++)
        {
            TextureRect comboSpr = _comboGraphics.FirstOrDefault(x => x.Modulate.A == 0 && !currentGraphics.Contains(x));
            if (comboSpr == null)
            {
                comboSpr = new TextureRect();
                comboSpr.Name = $"Instance {_comboGraphics.Count}";
                _comboGraphics.Add(comboSpr);
                AddChild(comboSpr);
            }

            comboSpr.MoveToFront();
            comboSpr.Texture = Textures[splitDigits[i]];
            comboSpr.Size = comboSpr.Texture.GetSize();
            comboSpr.Scale = GraphicScale;
            comboSpr.Position = (pos ?? Vector2.Zero) + new Vector2(i * generalSize, 0);
            comboSpr.Material = GetMaterialFromRating(LastRating);
            comboSpr.Modulate = new Color(comboSpr.Modulate.R, comboSpr.Modulate.G, comboSpr.Modulate.B);

            currentGraphics[i] = comboSpr;
            _comboVelocities[comboSpr] = new Vector2(GD.RandRange(1, 15), GD.RandRange(-160, -140));
            _comboAccelerations[comboSpr] = GD.RandRange(300, 450);

            Tween fadeTween = comboSpr.CreateTween();
            fadeTween.TweenProperty(comboSpr, "modulate", Colors.Transparent, 0.2d)
                .SetDelay(60d / Conductor.Bpm * 2d);
            fadeTween.Play();
        }

        _wasZero = combo == 0;
        if (_wasZero)
            LastRating = HitType.Perfect;
    }
    
    public override void _Process(double delta)
    {
        base._Process(delta);
        
        // Theoretically I could use Rigidbody2Ds but I think running the positioning in Process makes it look better :)
        for (int i = 0; i < _comboGraphics.Count; i++)
        {
            if (_comboGraphics[i].Modulate.A == 0)
                continue;

            TextureRect comboSpr = _comboGraphics[i];
            Vector2 velocity = _comboVelocities[comboSpr];
            int acceleration = _comboAccelerations[comboSpr];

            comboSpr.Position += velocity * (float)delta;
            _comboVelocities[comboSpr] += new Vector2(0f, acceleration * (float)delta);
        }
    }
}