using BaseSubmenu = Rubicon.scenes.options.objects.BaseSubmenu;

namespace Rubicon.scenes.options.submenus.video;

public partial class Video : BaseSubmenu
{
    [NodePath("Container/MaxFPS")] private Label MaxFPS;
    [NodePath("Container/WindowMode")] private OptionButton WindowMode;
    [NodePath("Container/VSync")] private OptionButton VSync;

    public override void _Ready()
    {
        base._Ready();
        this.OnReady();
        LoadSettings();
        
        RegisterOptionButton(VSync, i => OptionsMenu.Instance.HelperMethods.SetVSync((DisplayServer.VSyncMode)i));
        RegisterOptionButton(WindowMode, i => OptionsMenu.Instance.HelperMethods.SetWindowMode((DisplayServer.WindowMode)i));
        RegisterSlider(MaxFPS, "Max FPS", OptionsMenu.Instance.HelperMethods.SetMaxFPS, false);
    }
    
    private void LoadSettings()
    {
        LoadOptionButtonValue(VSync, (int)Main.RubiconSettings.Video.VSync);
        LoadOptionButtonValue(WindowMode, (int)Main.RubiconSettings.Video.WindowMode);
        LoadSliderValue(MaxFPS, "Max FPS", Main.RubiconSettings.Video.MaxFPS);
    }
}
