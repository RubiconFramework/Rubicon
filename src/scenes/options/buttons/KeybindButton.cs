using BaseRubicon.Backend.Autoload;
using BaseRubicon.Scenes.Options.Elements.Enums;

namespace BaseRubicon.Scenes.Options.Buttons;

public partial class KeybindButton : Button
{
    [Export] private string Action;
    [NodePath("AppendKey")] private Button AppendKey;
    private Button CurrentButton;

    public override void _Ready()
    {
        this.OnReady();
        AppendKey.Pressed += CreateKeybindButton;
    }
    
    private void CreateKeybindButton()
    {
        Button button = new();
        button.Text = "N/A";
        AppendKey.AddChild(button);

        Rect2 appendKeyRect = AppendKey.GetRect();
        Rect2 rect2 = button.GetRect();
        Vector2 newPosition = new(appendKeyRect.Position.X + appendKeyRect.Size.X + 15, appendKeyRect.Position.Y);
        rect2.Position = newPosition;
        button.Pressed += () => StartKeybinding(button);
    }
        
    private void StartKeybinding(Button button)
    {
        if (OptionsMenu.Instance.OptionsMenuCurrentState == OptionsMenuState.Idle)
        {
            OptionsMenu.Instance.SubmenuIndicatorAnimationPlayer.Play("KeybindPicking/PickingKeybind");
            OptionsMenu.Instance.OptionsMenuCurrentState = OptionsMenuState.ChoosingKeybind;
            CurrentButton = button;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is not InputEventKey inputEventKey) return;
        
        if (OptionsMenu.Instance.OptionsMenuCurrentState == OptionsMenuState.ChoosingKeybind)
        {
            OptionsMenu.Instance.SubmenuIndicatorAnimationPlayer.Play("KeybindPicking/PickedKeybind");
            Global.Settings.SetKeybind(OS.GetKeycodeString(inputEventKey.Keycode), Action);
            OptionsMenu.Instance.OptionsMenuCurrentState = OptionsMenuState.Idle;
        }
    }
}
