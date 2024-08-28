using Godot;

namespace Rubicon.Menus.API;

/// <summary>
/// A base for any menu.
/// </summary>
public partial class BaseMenu : Control
{
    #region Exported Variables
    /// <summary>
    /// An array of options for the BaseMenu.
    /// </summary>
    [Export] public MenuOption[] Options;
    
    /// <summary>
    /// The current selection.
    /// </summary>
    [ExportGroup("Status"), Export] public int Selection = 0;
    
    /// <summary>
    /// Whether to allow the menu to be controlled by the player.
    /// </summary>
    [ExportGroup("Settings"), Export] public bool AllowControl = true;

    /// <summary>
    /// Whether to allow echoing when navigating the menu. (like when you hold down a key on the keyboard and it starts repeating that input)
    /// </summary>
    [Export] public bool AllowEcho = true;
    
    /// <summary>
    /// The sound effect to play when the cursor is moved (aka changing selections). Does not play by itself.
    /// </summary>
    [ExportGroup("References"), Export] public AudioStream MoveCursor;
    
    /// <summary>
    /// The sound effect to play when something is confirmed. Does not play by itself.
    /// </summary>
    [Export] public AudioStream ConfirmAudio;
    
    /// <summary>
    /// The sound effect to play when backing out. Does not play by itself.
    /// </summary>
    [Export] public AudioStream BackAudio;
    #endregion

    #region Signals
    /// <summary>
    /// Is invoked when the user presses down.
    /// </summary>
    [Signal] public delegate void DownPressedEventHandler(bool isPressed);
    
    /// <summary>
    /// Is invoked when the user presses up.
    /// </summary>
    [Signal] public delegate void UpPressedEventHandler(bool isPressed);
    
    /// <summary>
    /// Is invoked when the user presses left.
    /// </summary>
    [Signal] public delegate void LeftPressedEventHandler(bool isPressed);
    
    /// <summary>
    /// Is invoked when the user presses right.
    /// </summary>
    [Signal] public delegate void RightPressedEventHandler(bool isPressed);
    
    /// <summary>
    /// Is invoked when the user presses the confirm button.
    /// </summary>
    [Signal] public delegate void ConfirmPressedEventHandler(bool isPressed);
    
    /// <summary>
    /// Is invoked when the user presses the back button.
    /// </summary>
    [Signal] public delegate void BackPressedEventHandler(bool isPressed);
    
    /// <summary>
    /// Is invoked when the user scrolls with their mouse.
    /// </summary>
    [Signal] public delegate void ScrollEventHandler(byte direction);
    #endregion

    /// <summary>
    /// Runs every time a valid input goes through. Can be overridden.
    /// </summary>
    public virtual void UpdateSelection()
    {
        for (int i = 0; i < Options.Length; i++)
        {
            MenuOption curOption = Options[i];
                
            bool needsUpdate = curOption.IsSelected != (i == Selection);
            curOption.IsSelected = i == Selection;
            if (needsUpdate)
            {
                AnimationPlayer animator = GetNode<AnimationPlayer>(curOption.PathToAnimationPlayer);
                animator.Play(curOption.IsSelected ? curOption.SelectedAnimation : curOption.DeselectedAnimation);
                animator.Seek(0, true);
            }
        }
    }
    
    /// <summary>
    /// Redirects to UpdateSelection();
    /// </summary>
    public override void _Ready() => UpdateSelection();
    
    /// <summary>
    /// Handles all the menu input.
    /// </summary>
    /// <param name="event">The input event.</param>
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (!AllowControl)
            return;

        if (@event is InputEventMouseButton mouseEvent)
        {
            switch (mouseEvent.ButtonIndex)
            {
                case MouseButton.WheelDown:
                    EmitSignal(SignalName.Scroll, -1);
                    break;
                case MouseButton.WheelUp:
                    EmitSignal(SignalName.Scroll, 1);
                    break;
            }

            return;
        }
       
        // heh, yanderedev moment
        if (@event.IsAction("MENU_DOWN", AllowEcho))
        {
            EmitSignal(SignalName.DownPressed, @event.IsPressed());
            UpdateSelection();
        }
        else if (@event.IsAction("MENU_UP", AllowEcho))
        {
            EmitSignal(SignalName.UpPressed, @event.IsPressed());
            UpdateSelection();
        }
        else if (@event.IsAction("MENU_LEFT", AllowEcho))
        {
            EmitSignal(SignalName.LeftPressed, @event.IsPressed());
            UpdateSelection();
        }
        else if (@event.IsAction("MENU_RIGHT", AllowEcho))
        {
            EmitSignal(SignalName.RightPressed, @event.IsPressed());
            UpdateSelection();
        }
        else if (@event.IsAction("MENU_CONFIRM"))
        {
            EmitSignal(SignalName.ConfirmPressed, @event.IsPressed());
            UpdateSelection();
        }
        else if (@event.IsAction("MENU_BACK", AllowEcho))
        {
            EmitSignal(SignalName.BackPressed, @event.IsPressed());
            UpdateSelection();
        }
    }
}