
using System.Collections.Generic;

namespace Rubicon.Gameplay.Classes.Events;
[GlobalClass]
public partial class SongEvent : Node
{
    public float Time;
    public Dictionary<string,dynamic> Values;
    public bool WasHit = false;
    [NodePath("../")] public EventHandler eventHandler;
    public override void _Ready() => this.OnReady();
    public void EventHit() => WasHit = true;
    public void EventFinish() => QueueFree();
}
