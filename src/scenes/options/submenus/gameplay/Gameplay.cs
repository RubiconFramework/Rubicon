using Godot.Sharp.Extras;
using Rubicon.scenes.options.elements;
using Rubicon.scenes.options.submenus.gameplay.enums;

namespace Rubicon.scenes.options.submenus.gameplay;

public partial class Gameplay : BaseSubmenu
{
    [NodePath("Container/Downscroll")] private CheckBox Downscroll;
    [NodePath("Container/ScrollSpeedType")] private OptionButton ScrollSpeedType;
    [NodePath("Container/ScrollSpeed")] private Label ScrollSpeed;

    public override void _Ready()
    {
        base._Ready();
        this.OnReady();
        LoadSettings();
        
        RegisterButton(Downscroll, _ => Main.RubiconSettings.Gameplay.Downscroll = !Main.RubiconSettings.Gameplay.Downscroll);
        RegisterOptionButton(ScrollSpeedType, i => Main.RubiconSettings.Gameplay.ScrollSpeedType = (ScrollSpeedType)i);
        RegisterSlider(ScrollSpeed, "Scroll Speed", v => Main.RubiconSettings.Gameplay.ScrollSpeed = v, false);
    }
    
    private void LoadSettings()
    {
        LoadButtonValue(Downscroll, Main.RubiconSettings.Gameplay.Downscroll);
        LoadOptionButtonValue(ScrollSpeedType, (int)Main.RubiconSettings.Gameplay.ScrollSpeedType);
        LoadSliderValue(ScrollSpeed, "Scroll Speed", Main.RubiconSettings.Gameplay.ScrollSpeed);
    }
}
