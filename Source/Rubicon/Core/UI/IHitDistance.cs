using Rubicon.Core.Data;

namespace Rubicon.Core.UI;

/// <summary>
/// An interface to implement on classes meant to show the hit distance from note times.
/// </summary>
public interface IHitDistance
{
    /// <summary>
    /// Shows the hit distance from the actual note time.
    /// </summary>
    /// <param name="distance">The hit distance from the note time</param>
    /// <param name="type">The <see cref="HitType"/> provided</param>
    /// <param name="offset">A <see cref="Vector2"/> that offsets the position from its default anchors</param>
    public void Show(double distance, HitType type, Vector2? offset);

    /// <summary>
    /// Shows the hit distance from the actual note time, with access to the anchors.
    /// </summary>
    /// <param name="distance">The hit distance from the note time</param>
    /// <param name="type">The <see cref="HitType"/> provided</param>
    /// <param name="anchorLeft">The left anchor (usually from 0 to 1)</param>
    /// <param name="anchorTop">The top anchor (usually from 0 to 1)</param>
    /// <param name="anchorRight">The right anchor (usually from 0 to 1)</param>
    /// <param name="anchorBottom">The bottom anchor (usually from 0 to 1)</param>
    /// <param name="offset">A <see cref="Vector2"/> that offsets the position from its default anchors</param>
    public void Show(double distance, HitType type, float anchorLeft, float anchorTop, float anchorRight, float anchorBottom, Vector2? offset);
}