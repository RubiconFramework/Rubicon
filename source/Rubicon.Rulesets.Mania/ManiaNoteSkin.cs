namespace Rubicon.Rulesets.Mania;

public partial class ManiaNoteSkin : Resource
{
    [Export] public int LaneCount = 4;
    
    [Export] public SpriteFrames NoteTextures;

    [Export] public SpriteFrames LaneTextures;

    [Export] public CanvasItem.TextureFilterEnum Filter = CanvasItem.TextureFilterEnum.Linear;

    [Export] public string[] Directions = ["LEFT", "DOWN", "UP", "RIGHT"];
}