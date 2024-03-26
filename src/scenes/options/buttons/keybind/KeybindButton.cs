using System.Collections.Generic;
using System.Linq;
using Rubicon.Scenes.Options.Elements.Enums;

namespace Rubicon.Scenes.Options.Buttons.Keybind;

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
            Button button = new();
            button.AddThemeFontSizeOverride("font_size", 15);
            AppendKey.AddChild(button);

            float centerY = (AppendKey.GetRect().Size.Y - button.GetRect().Size.Y) / 2;

            Vector2 appendKeyGlobalPos = AppendKey.GlobalPosition;
            Vector2 buttonGlobalPos = new(appendKeyGlobalPos.X, appendKeyGlobalPos.Y + centerY);

            if (buttonPositions.Count > 0)
            {
                var lastButton = buttonPositions.Keys.Last();
                buttonGlobalPos.X = buttonPositions[lastButton].X + lastButton.GetRect().Size.X + ButtonSpacing;
            }
            else buttonGlobalPos.X = appendKeyGlobalPos.X + AppendKey.GetRect().Size.X + ButtonSpacing;
            
            button.Text = $"  {keybind}  ";
            buttonPositions.Add(button, buttonGlobalPos);
            button.GlobalPosition = buttonGlobalPos;
            button.Pressed += () => StartKeybindingSequence(button);
        }
    }

    private void AppendBindableButton()
    {
        Button button = new();
        button.Text = "N/A";
        button.AddThemeFontSizeOverride("font_size", 15);
        AppendKey.AddChild(button);

        float centerY = (AppendKey.GetRect().Size.Y - button.GetRect().Size.Y) / 2;

        Vector2 appendKeyGlobalPos = AppendKey.GlobalPosition;
        Vector2 buttonGlobalPos = new(appendKeyGlobalPos.X, appendKeyGlobalPos.Y + centerY);

        if (buttonPositions.Count > 0)
        {
            var lastButton = buttonPositions.Keys.Last();
            buttonGlobalPos.X = buttonPositions[lastButton].X + lastButton.GetRect().Size.X + ButtonSpacing;
        }
        else buttonGlobalPos.X = appendKeyGlobalPos.X + AppendKey.GetRect().Size.X + ButtonSpacing;
        
        buttonPositions.Add(button, buttonGlobalPos);
        button.GlobalPosition = buttonGlobalPos;
        StartKeybindingSequence(button);
        button.Pressed += () => StartKeybindingSequence(button);
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

        if (foundDeletedButton) UpdateButtonPositions(deletedButtonPos);
        
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

    private void UpdateButtonPositions(Vector2 deletedButtonPos)
    {
        List<Button> buttonsToUpdate = new();
        foreach (var kvp in buttonPositions) if (kvp.Value.X > deletedButtonPos.X) buttonsToUpdate.Add(kvp.Key);
        foreach (var button in buttonsToUpdate)
        {
            buttonPositions[button] = new(buttonPositions[button].X - CurrentButton.GetRect().Size.X - ButtonSpacing, buttonPositions[button].Y);
            button.GlobalPosition = buttonPositions[button];
        }
    }
}

