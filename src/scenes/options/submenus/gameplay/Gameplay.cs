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
        
        RegisterButton(Downscroll, _ => UserSettings.Gameplay.Downscroll = !UserSettings.Gameplay.Downscroll);
        RegisterOptionButton(ScrollSpeedType, i => UserSettings.Gameplay.ScrollSpeedType = (ScrollSpeedType)i);
        RegisterSlider(ScrollSpeed, "Scroll Speed", v => UserSettings.Gameplay.ScrollSpeed = v, false);
    }
    
    private void LoadSettings()
    {
        LoadButtonValue(Downscroll, UserSettings.Gameplay.Downscroll);
        LoadOptionButtonValue(ScrollSpeedType, (int)UserSettings.Gameplay.ScrollSpeedType);
        LoadSliderValue(ScrollSpeed, "Scroll Speed", UserSettings.Gameplay.ScrollSpeed);
    }
}
