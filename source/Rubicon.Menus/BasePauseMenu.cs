using Godot;
using Rubicon.Game;

namespace Rubicon.Menus;

/// <summary>
/// A base for any pause menu.
/// </summary>
public partial class BasePauseMenu : BaseMenu
{
    /// <summary>
    /// Is invoked when the user presses the pause button.
    /// </summary>
    [Signal] public delegate void PauseOpenedEventHandler();
        
    /// <summary>
    /// Overridden so that pausing takes place BEFORE the actual menu inputs.
    /// </summary>
    /// <param name="event">The input event</param>
    public override void _Input(InputEvent @event)
    {
        // Do it before so that the base _Input event doesn't capture the event and invoke ConfirmPressed after pausing the game
        if (!RubiconGame.Instance.Paused && @event.IsActionPressed("GAME_PAUSE"))
        {
            RubiconGame.Instance.Pause();
            EmitSignal(SignalName.PauseOpened);
            return;
        }
            
        base._Input(@event);
    }
}