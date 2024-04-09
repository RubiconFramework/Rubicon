using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Sharp.Extras;

namespace Rubicon.scenes.options.buttons.keybind;

public partial class KeybindButton : Button
{
    [Export] private string Action;
    [Export] private int ButtonSpacing = 15;

    [NodePath("AppendKey")] private Button AppendKey;
    private Button CurrentButton;
    private readonly Dictionary<Button, Vector2> buttonPositions = new();

    public override void _Ready()
    {
        this.OnReady();
        AppendKey.Pressed += AppendBindableButton;
        InitializeKeybindButtons();
    }

    private void InitializeKeybindButtons()
    {
        List<string> keybinds = Main.GameSettings.GetKeybind(Action);
        foreach (string keybind in keybinds)
        {
            Button button = NewKeybindButton(keybind);
            button.GlobalPosition = new Vector2(button.GlobalPosition.X, AppendKey.GlobalPosition.Y + AppendKey.GetRect().Size.Y / 2 - button.GetRect().Size.Y / 2);
            button.Pressed += () => StartKeybindingSequence(button);
        }
    }

    private void AppendBindableButton()
    {
        Button button = NewKeybindButton("N/A");
        button.GlobalPosition = new Vector2(button.GlobalPosition.X, AppendKey.GlobalPosition.Y + AppendKey.GetRect().Size.Y / 2 - button.GetRect().Size.Y / 2);
        StartKeybindingSequence(button);
        button.Pressed += () => StartKeybindingSequence(button);
    }

    private Button NewKeybindButton(string keybind)
    {
        Button button = new();
        button.AddThemeFontSizeOverride("font_size", 15);
        AppendKey.AddChild(button);

        button.Text = $" {keybind} ";

        Vector2 buttonGlobalPos = GetCenteredButtonPositionX();
        button.GlobalPosition = buttonGlobalPos;
        buttonPositions.Add(button, buttonGlobalPos);

        return button;
    }
    
    private Vector2 GetCenteredButtonPositionX()
    {
        Vector2 appendKeyGlobalPos = AppendKey.GlobalPosition;
        Vector2 buttonGlobalPos = new(appendKeyGlobalPos.X, 0);

        if (buttonPositions.Count > 0)
        {
            var lastButton = buttonPositions.Keys.Last();
            buttonGlobalPos.X = buttonPositions[lastButton].X + lastButton.GetRect().Size.X + ButtonSpacing;
        }
        else buttonGlobalPos.X = appendKeyGlobalPos.X + AppendKey.GetRect().Size.X + ButtonSpacing;
        return buttonGlobalPos;
    }
    
    private void StartKeybindingSequence(Button button)
    {
        if (!OptionsMenu.Instance.IsPickingKeybind)
        {
            OptionsMenu.Instance.SubmenuIndicatorAnimationPlayer.Play("KeybindPicking/PickingKeybind");
            OptionsMenu.Instance.IsPickingKeybind = true;
            CurrentButton = button;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is not InputEventKey inputEventKey) return;

        if (OptionsMenu.Instance.IsPickingKeybind && CurrentButton != null)
        {
            if (inputEventKey.Keycode == Key.Backspace) RemoveKeybind();
            else SetKeybind(inputEventKey);
        }
    }

    private void RemoveKeybind()
    {
        Main.GameSettings.RemoveKeybind(Action);

        Vector2 deletedButtonPos = Vector2.Zero;
        bool foundDeletedButton = false;
        foreach (var kvp in buttonPositions)
        {
            if (kvp.Key == CurrentButton)
            {
                deletedButtonPos = kvp.Value;
                CurrentButton.QueueFree();
                buttonPositions.Remove(CurrentButton);
                foundDeletedButton = true;
                break;
            }
        }

        if (foundDeletedButton)
        {
            List<Button> buttonsToUpdate = buttonPositions.Keys.OrderBy(b => buttonPositions[b].X).ToList();

            foreach (var button in buttonsToUpdate)
            {
                if (buttonPositions[button].X > deletedButtonPos.X)
                {
                    Vector2 newPosition = new Vector2(buttonPositions[button].X - CurrentButton.GetRect().Size.X - ButtonSpacing, AppendKey.GlobalPosition.Y + AppendKey.GetRect().Size.Y / 2 - button.GetRect().Size.Y / 2);
                    buttonPositions[button] = newPosition;
                    button.GlobalPosition = newPosition;
                }
            }
        }

        CurrentButton = null;
        OptionsMenu.Instance.IsPickingKeybind = false;
        OptionsMenu.Instance.SubmenuIndicatorAnimationPlayer.Play("KeybindPicking/PickedKeybind");
    }
    
    private void SetKeybind(InputEventKey inputEventKey)
    {
        CurrentButton.Text = $"  {OS.GetKeycodeString(inputEventKey.Keycode)}  ";
        Main.GameSettings.SetKeybind(OS.GetKeycodeString(inputEventKey.Keycode), Action);
        CurrentButton = null;
        OptionsMenu.Instance.IsPickingKeybind = false;
        OptionsMenu.Instance.SubmenuIndicatorAnimationPlayer.Play("KeybindPicking/PickedKeybind");
    }
}

