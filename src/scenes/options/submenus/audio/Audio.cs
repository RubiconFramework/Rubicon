using BaseRubicon.Backend.Autoload;
using BaseRubicon.Scenes.Options.Elements;
using BaseRubicon.Scenes.Options.Submenus.Audio.Enums;

namespace BaseRubicon.Scenes.Options.Submenus.Audio;

public partial class Audio : BaseSubmenu
{
    [NodePath("Container/SoundOutputMode")] private OptionButton SoundOutputMode;
    [NodePath("Container/MasterVolume")] private Label MasterVolume;
    [NodePath("Container/MusicVolume")] private Label MusicVolume;
    [NodePath("Container/SFXVolume")] private Label SFXVolume;
    [NodePath("Container/InstVolume")] private Label InstVolume;
    [NodePath("Container/VoiceVolume")] private Label VoiceVolume;
    
    public override void _Ready()
    {
        base._Ready();
        this.OnReady();
        LoadSettings();
        
        RegisterSlider(MasterVolume, "Master Volume", v => OptionsMenu.Instance.HelperMethods.SetVolume(VolumeType.MasterVolume, v), true);
        RegisterSlider(MusicVolume, "Music Volume", v => OptionsMenu.Instance.HelperMethods.SetVolume(VolumeType.MusicVolume, v), true);
        RegisterSlider(SFXVolume, "SFX Volume", v => OptionsMenu.Instance.HelperMethods.SetVolume(VolumeType.SFXVolume, v), true);
        RegisterSlider(InstVolume, "Inst Volume", v => OptionsMenu.Instance.HelperMethods.SetVolume(VolumeType.InstVolume, v), true);
        RegisterSlider(VoiceVolume, "Voice Volume", v => OptionsMenu.Instance.HelperMethods.SetVolume(VolumeType.VoiceVolume, v), true);
        RegisterOptionButton(SoundOutputMode, i => Global.Settings.Audio.OutputMode = (OutputMode)i);
    }
    
    private void LoadSettings()
    {
        LoadSliderValue(MasterVolume, "Master Volume", Global.Settings.Audio.MasterVolume, true);
        LoadSliderValue(MusicVolume, "Music Volume", Global.Settings.Audio.MusicVolume, true);
        LoadSliderValue(SFXVolume, "SFX Volume", Global.Settings.Audio.SFXVolume, true);
        LoadSliderValue(InstVolume, "Inst Volume", Global.Settings.Audio.InstVolume, true);
        LoadSliderValue(VoiceVolume, "Voice Volume", Global.Settings.Audio.VoiceVolume, true);
        LoadOptionButtonValue(SoundOutputMode, (int)Global.Settings.Audio.OutputMode);
    }
}
