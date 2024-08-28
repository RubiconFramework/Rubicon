using Godot;
using Godot.Sharp.Extras;
using Rubicon.Data;

namespace Rubicon.Menus.Options.Objects.Sections.video;

public partial class Video : SettingsSectionBase
{
    [NodePath("Container/MaxFPS")] private Label MaxFPS;
    [NodePath("Container/WindowMode")] private OptionButton WindowMode;
    [NodePath("Container/VSync")] private OptionButton VSync;

    public override void _Ready()
    {
        base._Ready();
        this.OnReady();
        
        SetupOptionButton(VSync, i => HelperMethods.SetVSync((DisplayServer.VSyncMode)i), 
            (int)SaveData.Video.VSync);
        
        SetupOptionButton(WindowMode, i => HelperMethods.SetWindowMode((DisplayServer.WindowMode)i), 
            (int)SaveData.Video.WindowMode);
        
        SetupSlider(MaxFPS, "Max FPS", HelperMethods.SetMaxFPS, 
            SaveData.Video.MaxFPS);
    }
}
