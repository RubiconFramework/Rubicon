using System.Collections.Generic;
using System.Linq;
using BaseSubmenu = Rubicon.scenes.options.objects.BaseSubmenu;

namespace Rubicon.scenes.options.submenus.keybinds;

public partial class Keybinds : BaseSubmenu
{
    [NodePath("Container")] private VBoxContainer KeybindContainer;
    
    private Dictionary<Button, string> buttonKeybindings = new();
    private bool isPickingKeybind;
    private Button currentKeybindButton;
    
    private const float BUTTON_SPACING_MULTIPLIER = 1.05f;
    
    public override void _Ready()
    {
        this.OnReady();
        foreach (string action in InputMap.GetActions().Select(action => action.ToString()).Where(action => !action.StartsWith("ui_")).ToList())
        {
            Label actionLabel = new Label();
            actionLabel.AddThemeFontSizeOverride("font_size", 19);
            actionLabel.Text = $" {action} -> ";
            KeybindContainer.AddChild(actionLabel);

            List<Button> actionButtons = new();
            float buttonX = actionLabel.GetMinimumSize().X;
            foreach (string keybind in GetKeybinds(action))
            {
                Button button = new Button();
                button.AddThemeFontSizeOverride("font_size", 17);
                button.Text = $" {keybind} ";
                buttonKeybindings.Add(button, action);
                actionLabel.AddChild(button);
                actionButtons.Add(button);

                button.Position = new Vector2(buttonX, button.CustomMinimumSize.Y + 2f); 
                button.Pressed += () =>
                {
                    if (!isPickingKeybind)
                    {
                        OptionsMenu.Instance.OptionsMenuAnimPlayer.Play("KeybindPicking/PickingKeybind");
                        isPickingKeybind = true;
                        currentKeybindButton = button;
                    }
                };

                if (actionButtons.Count > 1 && actionButtons[0] != button) buttonX = button.Position.X + buttonX - actionLabel.GetMinimumSize().X; 
                else buttonX = button.Position.X + actionLabel.GetMinimumSize().X;
            }
        }
    }
    
    public override void _Input(InputEvent @event)
    {
        if (@event is not InputEventKey inputEventKey) return;

        if (isPickingKeybind && currentKeybindButton != null)
        {
            if (inputEventKey.Keycode == Key.Backspace) RemoveKeybind(currentKeybindButton);
            else SetKeybind(inputEventKey);
        }
    }

    private void RemoveKeybind(Button button)
    {
        if (buttonKeybindings.TryGetValue(button, out string action))
        {
            List<InputEventKey> keyEvents = InputMap.ActionGetEvents(action).OfType<InputEventKey>().ToList();
            InputEventKey eventToRemove = keyEvents.Find(k => k.Keycode.ToString() == button.Text.Trim());
            if (eventToRemove != null)
            {
                InputMap.ActionEraseEvent(action, eventToRemove);
                buttonKeybindings.Remove(button);
                button.QueueFree();
                Main.Instance.SendNotification($"Button {button.Text.Trim()} removed from keybinds (Action: {action})");
            }
            else Main.Instance.SendNotification($"Button {button.Text.Trim()} not found in keybinds for action {action}");
        }
        else Main.Instance.SendNotification($"Button {button.Text} not found in keybinds");

        currentKeybindButton = null;
        isPickingKeybind = false;
        OptionsMenu.Instance.OptionsMenuAnimPlayer.Play("KeybindPicking/RemovedKeybind");
    }

    private void SetKeybind(InputEventKey inputEventKey)
    {
        currentKeybindButton.Text = $"  {OS.GetKeycodeString(inputEventKey.Keycode)}  ";
        InputMap.AddAction(buttonKeybindings[currentKeybindButton]);
        InputEventKey keyEvent = new InputEventKey();
        keyEvent.Keycode = inputEventKey.Keycode;
        InputMap.ActionAddEvent(buttonKeybindings[currentKeybindButton], keyEvent);
        OptionsMenu.Instance.KeybindLabel.Text = $"{keyEvent.Keycode} Selected.";

        InputMap.ActionEraseEvents(buttonKeybindings[currentKeybindButton]);
        Main.Instance.SendNotification($"{OS.GetKeycodeString(inputEventKey.Keycode)} bound to {buttonKeybindings[currentKeybindButton]}");
        currentKeybindButton = null;
        isPickingKeybind = false;
        OptionsMenu.Instance.OptionsMenuAnimPlayer.Play("KeybindPicking/PickedKeybind");
    }

    public List<string> GetKeybinds(string action)
    {
        List<string> keybinds = InputMap.ActionGetEvents(action).OfType<InputEventKey>().Select(key => key.AsTextPhysicalKeycode()).ToList();
        return keybinds.Count > 0 ? keybinds : new List<string> { "N/A" };
    }
}
