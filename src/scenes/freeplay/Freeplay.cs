using Godot.Sharp.Extras;
using Array = Godot.Collections.Array;

namespace Rubicon.scenes.freeplay;

public partial class Freeplay : Node
{
    [Resource("res://src/scenes/freeplay/objects/resources/SongList.tres")] 
    [Export]
    public Resource SongList;
    
    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
    }
}
