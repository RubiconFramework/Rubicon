using Rubicon.gameplay.classes.events;

namespace Rubicon.gameplay.resources.events;

public partial class FocusCamera : SongEvent
{
    public new void EventHit()
    {
        base.EventHit();
        string charToFocus = Values["char"] == 0 ? "Opponent" : "Player";
        if(Values["char"] == 2) charToFocus = "Spectator";
        eventHandler.Gameplay.FocusCharacterCamera(charToFocus);
        GD.Print("camera change");
    }
}