using Rubicon.Core.Data;

namespace Rubicon.Core.UI;

/// <summary>
/// A control node that activates upon the player hitting a note, showing their combo.
/// </summary>
public partial class ComboDisplay : Control
{
    /// <summary>
    /// The textures to display. Should go from 0 to 9.
    /// </summary>
    [Export] public SpriteFrames Atlas;
    
    /// <summary>
    /// The spacing of the textures.
    /// </summary>
    [Export] public float Spacing = 100f;

    /// <summary>
    /// The scale of the textures.
    /// </summary>
    [Export] public Vector2 GraphicScale = Vector2.One;
    
    /// <summary>
    /// The last rating that was hit.
    /// </summary>
    protected HitType LastRating = HitType.Perfect;
    
    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Perfect"/>.
    /// </summary>
    public Material PerfectMaterial; // dokibird glasses

    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Great"/>.
    /// </summary>
    public Material GreatMaterial;

    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Good"/>.
    /// </summary>
    public Material GoodMaterial;

    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Bad"/>.
    /// </summary>
    public Material BadMaterial;
    
    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Horrible"/>.
    /// </summary>
    public Material HorribleMaterial;

    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Miss"/>.
    /// </summary>
    public Material MissMaterial;

    /// <summary>
    /// Displays the combo in a unique animation from the combo and hit type provided.
    /// </summary>
    /// <param name="combo">The current combo</param>
    /// <param name="type">The hit type provided</param>
    public virtual void Play(uint combo, HitType type)
    {
        Play(combo, type, Vector2.Zero);
    }

    /// <summary>
    /// Plays an animation depending on the hit type provided.
    /// </summary>
    /// <param name="combo">The current combo</param>
    /// <param name="type">The hit type provided</param>
    /// <param name="offset">A Vector2 that offsets the position</param>
    public virtual void Play(uint combo, HitType type, Vector2? offset)
    {
        // Makes the judgment anchor at the center probably
        Play(combo, type, 0.5f, 0.5f, 0.5f, 0.5f, offset);
    }
    
    /// <summary>
    /// Plays an animation depending on the hit type provided.
    /// </summary>
    /// <param name="combo">The current combo</param>
    /// <param name="type">The hit type provided</param>
    /// <param name="anchorLeft">The left anchor (usually from 0 to 1)</param>
    /// <param name="anchorTop">The top anchor (usually from 0 to 1)</param>
    /// <param name="anchorRight">The right anchor (usually from 0 to 1)</param>
    /// <param name="anchorBottom">The bottom anchor (usually from 0 to 1)</param>
    /// <param name="pos">Where to place the judgment, in pixels.</param>
    public virtual void Play(uint combo, HitType type, float anchorLeft, float anchorTop, float anchorRight, float anchorBottom, Vector2? pos)
    {
        AnchorLeft = anchorLeft;
        AnchorTop = anchorTop;
        AnchorRight = anchorRight;
        AnchorBottom = anchorBottom;
    }
    
    /// <summary>
    /// Get a material based on the rating.
    /// </summary>
    /// <param name="type">The rating</param>
    /// <returns>The Material associated with the Judgment</returns>
    protected Material GetMaterialFromRating(HitType type)
    {
        switch (type)
        {
            default:
                return PerfectMaterial;
            case HitType.Great:
                return GreatMaterial;
            case HitType.Good:
                return GoodMaterial;
            case HitType.Bad:
                return BadMaterial;
            case HitType.Horrible:
                return HorribleMaterial;
            case HitType.Miss:
                return MissMaterial;
        }
    }
}