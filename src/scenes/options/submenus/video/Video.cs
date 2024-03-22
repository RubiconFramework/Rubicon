using BaseRubicon.Backend.Autoload;
using BaseRubicon.Scenes.Options.Elements;

namespace BaseRubicon.Scenes.Options.Submenus.Video;

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
        LoadOptionButtonValue(VSync, (int)Global.Settings.Video.VSync);
        LoadOptionButtonValue(WindowMode, (int)Global.Settings.Video.WindowMode);
        LoadSliderValue(MaxFPS, "Max FPS", Global.Settings.Video.MaxFPS);
    }
}
