using Godot;
using Godot.Sharp.Extras;
using Rubicon.Data;

namespace Rubicon.Menus.Options.Objects.Sections.Gameplay;

public partial class Gameplay : SettingsSectionBase
{
    [NodePath("Container/Downscroll")] private CheckBox Downscroll;
    [NodePath("Container/ScrollSpeedType")] private OptionButton ScrollSpeedType;
    [NodePath("Container/ScrollSpeed")] private Label ScrollSpeed;

    public override void _Ready()
    {
        base._Ready();
        this.OnReady();
        
        SetupButton(Downscroll, _ => SaveData.Gameplay.Downscroll = !SaveData.Gameplay.Downscroll, 
            SaveData.Gameplay.Downscroll);
        
        SetupOptionButton(ScrollSpeedType, i => SaveData.Gameplay.ScrollSpeedType = (ScrollSpeedType)i, 
            (int)SaveData.Gameplay.ScrollSpeedType);
        
        SetupSlider(ScrollSpeed, "Scroll Speed", v => SaveData.Gameplay.ScrollSpeed = v, 
            SaveData.Gameplay.ScrollSpeed);
    }
}
