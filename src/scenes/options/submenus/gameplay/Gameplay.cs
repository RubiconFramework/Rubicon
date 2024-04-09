using Godot;
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
        
        RegisterButton(Downscroll, _ => Main.GameSettings.Gameplay.Downscroll = !Main.GameSettings.Gameplay.Downscroll);
        RegisterOptionButton(ScrollSpeedType, i => Main.GameSettings.Gameplay.ScrollSpeedType = (ScrollSpeedType)i);
        RegisterSlider(ScrollSpeed, "Scroll Speed", v => Main.GameSettings.Gameplay.ScrollSpeed = v, false);
    }
    
    private void LoadSettings()
    {
        LoadButtonValue(Downscroll, Main.GameSettings.Gameplay.Downscroll);
        LoadOptionButtonValue(ScrollSpeedType, (int)Main.GameSettings.Gameplay.ScrollSpeedType);
        LoadSliderValue(ScrollSpeed, "Scroll Speed", Main.GameSettings.Gameplay.ScrollSpeed);
    }
}
