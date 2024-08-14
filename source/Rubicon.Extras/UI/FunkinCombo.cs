using System.Collections.Generic;
using Godot;
using Promise.Framework;
using Promise.Framework.UI;

namespace Rubicon.Extras.UI;

/// <summary>
/// Overrides the original Promise.Framework ComboDisplay with animations more Funkin'-like.
/// </summary>
public partial class FunkinCombo : ComboDisplay
{
    #region Private Variables
    private Dictionary<TextureRect, Vector2> _comboVelocities = new Dictionary<TextureRect, Vector2>();
    private Dictionary<TextureRect, int> _comboAccelerations = new Dictionary<TextureRect, int>();
    private bool _wasZero = false;
    #endregion
    
    #region Public Methods
    /// <summary>
    /// Handles all the combo moving.
    /// </summary>
    /// <param name="delta">The time passed from the last frame, in seconds.</param>
    public override void _Process(double delta)
    {
        base._Process(delta);

        foreach (KeyValuePair<TextureRect, Vector2> velocityPair in _comboVelocities)
            velocityPair.Key.Position += velocityPair.Value * (float)delta;

        foreach (KeyValuePair<TextureRect, int> accelerationPair in _comboAccelerations)
            _comboVelocities[accelerationPair.Key] += new Vector2(0f, accelerationPair.Value * (float)delta);
    }

    /// <summary>
    /// Overriden UpdateCombo function that applies Funkin-like animations.
    /// </summary>
    /// <param name="combo">The current combo.</param>
    public override void UpdateCombo(uint combo)
    {
        if (combo == 0 && _wasZero)
            return;
        
        int[] splitDigits = new int[combo.ToString("D3").Length];
        for (int i = 0; i < splitDigits.Length; i++)
            splitDigits[i] = int.Parse(combo.ToString("D3").ToCharArray()[i].ToString());
        
        float generalSize = Spacing;
        for (int i = 0; i < splitDigits.Length; i++)
        {
            TextureRect comboSpr = new TextureRect();

            comboSpr.Texture = Textures[splitDigits[i]];
            comboSpr.Size = comboSpr.Texture.GetSize();
            comboSpr.Scale = Vector2.One;
            comboSpr.UseParentMaterial = true;
            AddChild(comboSpr);
            
            comboSpr.Position = new Vector2(i * generalSize, 0) - comboSpr.PivotOffset;
            
            _comboVelocities.Add(comboSpr, new Vector2((float)GD.RandRange(-5d, 5d), GD.RandRange(-160, -140)));
            _comboAccelerations.Add(comboSpr, GD.RandRange(300, 450));

            Tween fadeTween = comboSpr.CreateTween();
            fadeTween.TweenProperty(comboSpr, "self_modulate", Colors.Transparent, 0.2d)
                .SetDelay(60d / Conductor.Instance.Bpm * 2d);
            fadeTween.Finished += () => DeleteComboSprite(comboSpr);
            fadeTween.Play();
        }

        _wasZero = combo == 0;
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Ensures that a combo sprite is deleted and never called by this FunkinCombo instance again.
    /// </summary>
    /// <param name="comboSpr">A combo sprite, usually a TextureRect</param>
    private void DeleteComboSprite(TextureRect comboSpr)
    {
        _comboAccelerations.Remove(comboSpr);
        _comboVelocities.Remove(comboSpr);
        comboSpr.QueueFree();
    }
    #endregion
}