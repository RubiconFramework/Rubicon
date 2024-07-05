namespace OldRubicon.scenes.options.objects.sections.video;

public partial class Video : SettingsSectionBase
{
    [NodePath("Container/MaxFPS")] private Label MaxFPS;
    [NodePath("Container/WindowMode")] private OptionButton WindowMode;
    [NodePath("Container/VSync")] private OptionButton VSync;

    public override void _Ready()
    {
        base._Ready();
        this.OnReady();
        LoadSettings();
        
        RegisterOptionButton(VSync, i => Rubicon.menus.options.OptionsMenu.Instance.HelperMethods.SetVSync((DisplayServer.VSyncMode)i));
        RegisterOptionButton(WindowMode, i => Rubicon.menus.options.OptionsMenu.Instance.HelperMethods.SetWindowMode((DisplayServer.WindowMode)i));
        RegisterSlider(MaxFPS, "Max FPS", Rubicon.menus.options.OptionsMenu.Instance.HelperMethods.SetMaxFPS, false);
    }
    
    private void LoadSettings()
    {
        LoadOptionButtonValue(VSync, (int)Main.RubiconSettings.Video.VSync);
        LoadOptionButtonValue(WindowMode, (int)Main.RubiconSettings.Video.WindowMode);
        LoadSliderValue(MaxFPS, "Max FPS", Main.RubiconSettings.Video.MaxFPS);
    }
}
