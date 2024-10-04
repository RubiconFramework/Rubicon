using Rubicon.Core.Data;

namespace Rubicon.Core.UI;

/// <summary>
/// An interface for a class that displays the current combo for the player.
/// </summary>
public interface IComboDisplay
{
    /// <summary>
    /// Displays the combo information with the hit type provided.
    /// </summary>
    /// <param name="combo">The current combo</param>
    /// <param name="type">The hit type provided</param>
    /// <param name="offset">A Vector2 that offsets the position</param>
    public void Show(uint combo, HitType type, Vector2? offset);

    /// <summary>
    /// Displays the combo information with the hit type provided, providing access to the anchors as well.
    /// </summary>
    /// <param name="combo">The current combo</param>
    /// <param name="type">The hit type provided</param>
    /// <param name="anchorLeft">The left anchor (usually from 0 to 1)</param>
    /// <param name="anchorTop">The top anchor (usually from 0 to 1)</param>
    /// <param name="anchorRight">The right anchor (usually from 0 to 1)</param>
    /// <param name="anchorBottom">The bottom anchor (usually from 0 to 1)</param>
    /// <param name="offset">Where to offset the combo from the anchors, in pixels.</param>
    public void Show(uint combo, HitType type, float anchorLeft, float anchorTop, float anchorRight, float anchorBottom, Vector2? offset);
}