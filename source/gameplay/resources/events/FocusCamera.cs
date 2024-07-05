using FNFGodot.Gameplay.Classes.Events;

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
