using Button = Godot.Button;

namespace Rubicon.Scenes.Options.Elements;

[Icon("res://assets/miscicons/settingsbutton.png")]
public partial class BaseSubmenu : ScrollContainer
{
    protected void RegisterButton(Button button, Action<bool> updateAction)
    {
        button.Pressed += () =>
        {
            updateAction.Invoke(button.ButtonPressed);
            Main.GameSettings.Save();
        };
        button.MouseEntered += () => OptionsMenu.Instance.OptionDescriptionLabel.Text = Tr($"%{button.Name}%");
    }

    protected void RegisterOptionButton(OptionButton optionButton, Action<int> updateAction)
    {
        optionButton.ItemSelected += index =>
        {
            updateAction.Invoke((int)index);
            Main.GameSettings.Save();
        };
        optionButton.MouseEntered += () => OptionsMenu.Instance.OptionDescriptionLabel.Text = Tr($"%{optionButton.Name}%");
    }

    protected void RegisterSlider(Label label, string settingName, Action<float> updateAction, bool showPercentage)
    {
        label.GetNode<HSlider>("Slider").ValueChanged += v =>
        {
            label.Text = showPercentage ? $" {settingName}: [{(int)v}%]" : $" {settingName} [{(float)v}]";
            updateAction.Invoke((float)v);
            Main.GameSettings.Save();
        };
        label.MouseEntered += () => OptionsMenu.Instance.OptionDescriptionLabel.Text = Tr($"%{label.Name}%");
    }

    protected void RegisterColorPicker(Label label, Action<Color> updateAction)
    {
        label.GetNode<ColorPickerButton>("Picker").ColorChanged += color =>
        {
            updateAction.Invoke(color);
            Main.GameSettings.Save();
        };
        label.MouseEntered += () => OptionsMenu.Instance.OptionDescriptionLabel.Text = Tr($"%{label.Name}%");
    }

    protected void LoadButtonValue(Button button, bool v) => button.ButtonPressed = v;
    protected void LoadOptionButtonValue(OptionButton optionButton, int v) => optionButton.Selected = v;
    protected void LoadSliderValue(Label parent, string settingName, float v, bool showPercentage = false)
    {
        parent.GetNode<Slider>("Slider").Value = v;
        parent.Text = showPercentage ? $" {settingName}: [{(int)v}%]" : $" {settingName} [{v}]";
    }
}
