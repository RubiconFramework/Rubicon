using Rubicon.Backend.Autoload;
using Button = Godot.Button;

namespace Rubicon.menus.options.objects.sections;

[Icon("res://assets/miscicons/settingsbutton.png")]
public partial class SettingsSectionBase : ScrollContainer
{
    protected void SetupButton(Button button, Action<bool> updateAction, bool initialValue)
    {
        button.Pressed += () => 
        {
            updateAction.Invoke(button.ButtonPressed);
            RubiconSettings.Save();
        };
        button.MouseEntered += () => OptionsMenu.Instance.OptionDescriptionLabel.Text = Tr($"%{button.Name}%");
        button.ButtonPressed = initialValue;
    }

    protected void SetupOptionButton(OptionButton optionButton, Action<int> updateAction, int initialValue)
    {
        optionButton.ItemSelected += index => 
        {
            updateAction.Invoke((int)index);
            RubiconSettings.Save();
        };
        optionButton.MouseEntered += () => OptionsMenu.Instance.OptionDescriptionLabel.Text = Tr($"%{optionButton.Name}%");
        optionButton.Selected = initialValue;
    }

    protected void SetupSlider(Label label, string settingName, Action<float> updateAction, float initialValue, bool showPercentage = false)
    {
        var slider = label.GetNode<HSlider>("Slider");
        slider.ValueChanged += v => 
        {
            label.Text = showPercentage ? $"{settingName}: [{(int)v}%]" : $"{settingName} [{(float)v}]";
            updateAction.Invoke((float)v);
            RubiconSettings.Save();
        };
        label.MouseEntered += () => OptionsMenu.Instance.OptionDescriptionLabel.Text = Tr($"%{label.Name}%");
        slider.Value = initialValue;
        label.Text = showPercentage ? $"{settingName}: [{(int)initialValue}%]" : $"{settingName} [{initialValue}]";
    }

    protected void RegisterColorPicker(Label label, Action<Color> updateAction)
    {
        var colorPicker = label.GetNode<ColorPickerButton>("Picker");
        colorPicker.ColorChanged += color =>
        {
            updateAction.Invoke(color);
            RubiconSettings.Save();
        };
        label.MouseEntered += () => OptionsMenu.Instance.OptionDescriptionLabel.Text = Tr($"%{label.Name}%");
    }
}
