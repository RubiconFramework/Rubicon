using Godot.Sharp.Extras;
using Rubicon.Core.Data;
using Rubicon.Core.UI;

namespace Rubicon.Core;

[GlobalClass]
public partial class UiStyle : Resource
{
    [ExportGroup("Judgment"), Export] public PackedScene Judgment;

    [Export] public Vector2 JudgmentOffset;

    [ExportGroup("Combo"), Export] public PackedScene Combo;

    [Export] public Vector2 ComboOffset;

    [ExportGroup("Hit Distance"), Export] public PackedScene HitDistance;

    [Export] public Vector2 HitDistanceOffset;
    
    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Perfect"/>.
    /// </summary>
    [ExportGroup("Materials"), Export] public Material PerfectMaterial; // dokibird glasses

    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Great"/>.
    /// </summary>
    [Export] public Material GreatMaterial;

    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Good"/>.
    /// </summary>
    [Export] public Material GoodMaterial;

    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Bad"/>.
    /// </summary>
    [Export] public Material BadMaterial;

    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Horrible"/>.
    /// </summary>
    [Export] public Material HorribleMaterial;
    
    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Miss"/>.
    /// </summary>
    [Export] public Material MissMaterial;
}