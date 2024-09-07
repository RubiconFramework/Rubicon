using Rubicon.Core.Chart;

namespace Rubicon.Rulesets.Mania;

public partial class ManiaNoteManager : NoteManager
{
    [Export] public int Lane = 0;
     
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

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        string actionName = $"MANIA_{ParentBarLine.Managers}K_{Lane}";
        if (!InputMap.HasAction(actionName) || !@event.IsAction(actionName) || @event.IsEcho())
            return;

        if (@event.IsPressed())
        {
            
        }
    }
}