using Rubicon.Core.Chart;

namespace Rubicon.Rulesets.Mania;

public partial class ManiaNote : Note
{
    [Export] public SvChange SvChange;

    [Export] public ManiaNoteManager ParentManager;
    
    [Export] public SpriteFrames NoteTextures;

    [Export] public SpriteFrames TailTextures;
    
    public void Setup(NoteData noteData, SvChange svChange, ManiaNoteManager parentManager, ManiaNoteSkin noteSkin)
    {
        Info = noteData;
        SvChange = svChange;
        ParentManager = parentManager;
    }
    
    public override void UpdatePosition()
    {
        float startingPos = ParentManager.DistanceOffset;
        float distance = (float)(Info.MsTime - SvChange.MsTime) * ParentManager.ScrollSpeed;
        Vector2 posMult = new Vector2(Mathf.Cos(ParentManager.DirectionAngle), Mathf.Sin(ParentManager.DirectionAngle));
        Position = Vector2.One * (startingPos + distance) * posMult; // TODO: Do holding.
    }
}