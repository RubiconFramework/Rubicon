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
        
        RegisterOptionButton(VSync, i => OptionsMenu.HelperMethods.SetVSync((DisplayServer.VSyncMode)i));
        RegisterOptionButton(WindowMode, i => OptionsMenu.HelperMethods.SetWindowMode((DisplayServer.WindowMode)i));
        RegisterSlider(MaxFPS, "Max FPS", OptionsMenu.HelperMethods.SetMaxFPS, false);
    }
    
    private void LoadSettings()
    {
        LoadOptionButtonValue(VSync, (int)UserSettings.Video.VSync);
        LoadOptionButtonValue(WindowMode, (int)UserSettings.Video.WindowMode);
        LoadSliderValue(MaxFPS, "Max FPS", UserSettings.Video.MaxFPS);
    }
}
