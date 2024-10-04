using Rubicon.Core.Data;

namespace Rubicon.Core.UI;

/// <summary>
/// An interface to implement when a UI element needs colors based on certain judgments listed in <see cref="HitType"/>.
/// </summary>
public interface IJudgmentColor
{
    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Perfect"/>.
    /// </summary>
    public Color[] PerfectColors { get; set; } // dokibird glasses

    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Great"/>.
    /// </summary>
    public Color[] GreatColors { get; set; }

    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Good"/>.
    /// </summary>
    public Color[] GoodColors { get; set; }

    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Okay"/>.
    /// </summary>
    public Color[] OkayColors { get; set; }
    
    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Bad"/>.
    /// </summary>
    public Color[] BadColors { get; set; }

    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Miss"/>.
    /// </summary>
    public Color[] MissColors { get; set; }
}