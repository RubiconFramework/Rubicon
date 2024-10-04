using Rubicon.Core.Data;

namespace Rubicon.Core.UI;

/// <summary>
/// An interface to implement when a UI element needs materials based on certain judgments listed in <see cref="HitType"/>.
/// </summary>
public interface IJudgmentMaterial
{
    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Perfect"/>.
    /// </summary>
    public Material PerfectMaterial { get; set; } // dokibird glasses

    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Great"/>.
    /// </summary>
    public Material GreatMaterial { get; set; }

    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Good"/>.
    /// </summary>
    public Material GoodMaterial { get; set; }

    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Okay"/>.
    /// </summary>
    public Material OkayMaterial { get; set; }
    
    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Bad"/>.
    /// </summary>
    public Material BadMaterial { get; set; }

    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Miss"/>.
    /// </summary>
    public Material MissMaterial { get; set; }
}