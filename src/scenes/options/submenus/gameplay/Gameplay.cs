using BaseRubicon.Backend.Autoload;
using BaseRubicon.Scenes.Options.Elements;
using BaseRubicon.Scenes.Options.Submenus.Gameplay.Enums;

namespace BaseRubicon.Scenes.Options.Submenus.Gameplay;

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
        
        RegisterButton(Downscroll, _ => Global.Settings.Gameplay.Downscroll = !Global.Settings.Gameplay.Downscroll);
        RegisterOptionButton(ScrollSpeedType, i => Global.Settings.Gameplay.ScrollSpeedType = (ScrollSpeedType)i);
        RegisterSlider(ScrollSpeed, "Scroll Speed", v => Global.Settings.Gameplay.ScrollSpeed = v, false);
    }
    
    private void LoadSettings()
    {
        LoadButtonValue(Downscroll, Global.Settings.Gameplay.Downscroll);
        LoadOptionButtonValue(ScrollSpeedType, (int)Global.Settings.Gameplay.ScrollSpeedType);
        LoadSliderValue(ScrollSpeed, "Scroll Speed", Global.Settings.Gameplay.ScrollSpeed);
    }
}
