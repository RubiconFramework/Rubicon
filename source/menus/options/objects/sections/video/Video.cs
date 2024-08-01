using RubiconSettings = Rubicon.backend.autoload.RubiconSettings;

namespace Rubicon.menus.options.objects.sections.video;

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
            (int)RubiconSettings.Video.VSync);
        
        SetupOptionButton(WindowMode, i => HelperMethods.SetWindowMode((DisplayServer.WindowMode)i), 
            (int)RubiconSettings.Video.WindowMode);
        
        SetupSlider(MaxFPS, "Max FPS", HelperMethods.SetMaxFPS, 
            RubiconSettings.Video.MaxFPS);
    }
}
