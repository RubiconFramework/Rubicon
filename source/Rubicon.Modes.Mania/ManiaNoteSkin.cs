namespace Rubicon.Modes.Mania;

public partial class ManiaNoteSkin : Resource
{
    [Export] public int LaneCount = 4;
    
    [Export] public SpriteFrames NoteTextures;

    [Export] public PackedScene LaneScene;

    [Export] public CanvasItem.TextureFilterEnum Filter = CanvasItem.TextureFilterEnum.Linear;
}