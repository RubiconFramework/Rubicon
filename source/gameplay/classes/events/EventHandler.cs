using Rubicon.Backend.Autoload;

namespace Rubicon.Gameplay.Classes.Events;
[GlobalClass]
public partial class EventHandler : Node
{
    [NodePath("../")] public dynamic Gameplay;
    public override void _Ready() => this.OnReady();
    public override void _Process(double delta) {
        foreach(SongEvent songEvent in GetChildren())
        {
            if(songEvent.Time >= Conductor.SongPosition && !songEvent.WasHit)
            {
                songEvent.EventHit();
            }
        }
    }
}
