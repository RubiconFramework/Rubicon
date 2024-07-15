using Rubicon.Backend.Autoload;
using Rubicon.Backend.Classes;

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
        
        SetupButton(Downscroll, _ => RubiconSettings.Gameplay.Downscroll = !RubiconSettings.Gameplay.Downscroll, 
            RubiconSettings.Gameplay.Downscroll);
        
        SetupOptionButton(ScrollSpeedType, i => RubiconSettings.Gameplay.ScrollSpeedType = (ScrollSpeedType)i, 
            (int)RubiconSettings.Gameplay.ScrollSpeedType);
        
        SetupSlider(ScrollSpeed, "Scroll Speed", v => RubiconSettings.Gameplay.ScrollSpeed = v, 
            RubiconSettings.Gameplay.ScrollSpeed);
    }
}
