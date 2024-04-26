using Godot.Collections;

namespace Rubicon.scenes.freeplay.objects.resources;

public partial class FreeplaySong : Resource
{
    [ExportGroup("Song Data")]
    [Export] public string SongDisplayName { get; set; }
    [Export] public string SongName { get; set; }
    [Export] public SpriteFrames SongIcon { get; set; }
    [Export] public Array<string> Difficulties { get; set; } = new(){ "Hard", "Normal", "Easy" };

    [ExportGroup("Freeplay Data")]
    [Export] public Color BackgroundColor { get; set; }
    [Export] public string SongDescription { get; set; }
    [Export] public bool IsCameraBopping { get; set; }
}