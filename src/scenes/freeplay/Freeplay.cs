using Godot.Collections;
using Rubicon.scenes.freeplay.objects.resources;

namespace Rubicon.scenes.freeplay;

public partial class Freeplay : Node
{
    [Export(PropertyHint.ArrayType, "res://src/scenes/freeplay/objects/resources/FreeplaySong.tres")]
    private Array<FreeplaySong> Songs = new();
    
    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
    }
}
