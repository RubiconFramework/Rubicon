using Rubicon.Core.Chart;

namespace Rubicon.Rulesets.Mania;

public partial class ManiaNoteManager : NoteManager
{
    [Export] public NoteData NoteHeld;
    
    [Export] public float DirectionAngle = 90f;
    
    [Export] public ManiaNoteSkin NoteSkin;

    public void Setup()
    {
        
    }
    
    protected override Note SpawnNote(NoteData data, SvChange svChange)
    {
        
        
        return base.SpawnNote(data, svChange);
    }
}