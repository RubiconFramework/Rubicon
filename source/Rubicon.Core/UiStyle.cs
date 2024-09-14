using Godot.Sharp.Extras;
using Rubicon.Core.UI;

namespace Rubicon.Core;

[GlobalClass]
public partial class UiStyle : Resource
{
    [Export] public PackedScene Judgment;

    [Export] public Vector2 JudgmentOffset;
}