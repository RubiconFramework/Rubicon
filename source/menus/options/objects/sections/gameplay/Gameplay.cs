using Rubicon.Backend.Autoload;

namespace Rubicon.menus.options.objects.sections.gameplay;

public partial class Gameplay : SettingsSectionBase
{
    [NodePath("Container/Downscroll")] private CheckBox Downscroll;
    [NodePath("Container/ScrollSpeedType")] private OptionButton ScrollSpeedType;
    [NodePath("Container/ScrollSpeed")] private Label ScrollSpeed;

    public override void _Ready()
    {
        base._Ready();
        this.OnReady();
        LoadSettings();
        
        RegisterButton(Downscroll, _ => RubiconSettings.Gameplay.Downscroll = !RubiconSettings.Gameplay.Downscroll);
        RegisterOptionButton(ScrollSpeedType, i => RubiconSettings.Gameplay.ScrollSpeedType = (ScrollSpeedType)i);
        RegisterSlider(ScrollSpeed, "Scroll Speed", v => RubiconSettings.Gameplay.ScrollSpeed = v, false);
    }
    
    private void LoadSettings()
    {
        LoadButtonValue(Downscroll, RubiconSettings.Gameplay.Downscroll);
        LoadOptionButtonValue(ScrollSpeedType, (int)RubiconSettings.Gameplay.ScrollSpeedType);
        LoadSliderValue(ScrollSpeed, "Scroll Speed", RubiconSettings.Gameplay.ScrollSpeed);
    }
}
