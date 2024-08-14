using Godot;
using Rubicon.Space2D.Objects;

namespace Rubicon.Space2D;

public partial class Stage2D : Node2D
{
    [Export] public CharacterGroup2D[] CharacterGroups;
    
    [Export] public CameraFocusPoint2D[] FocusPoints;
}