
using System.Collections.Generic;

namespace Rubicon.gameplay.classes.events;
[GlobalClass]
public partial class SongEvent : Node
{
    public float Time;
    public Dictionary<string,dynamic> Values;
    public bool WasHit;
    [NodePath("../")] public EventHandler eventHandler;
    public override void _Ready() => this.OnReady();
    public void EventHit() => WasHit = true;
    public void EventFinish() => QueueFree();
}
