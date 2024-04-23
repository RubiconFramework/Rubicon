using Godot.Sharp.Extras;
using Rubicon.autoload.managers.audiomanager.enums;
using BaseSubmenu = Rubicon.scenes.options.objects.BaseSubmenu;

namespace Rubicon.scenes.options.submenus.audio;

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
        RegisterOptionButton(SoundOutputMode, i => Main.RubiconSettings.Audio.OutputMode = (OutputMode)i);
    }
    
    private void LoadSettings()
    {
        LoadSliderValue(MasterVolume, "Master Volume", Main.RubiconSettings.Audio.MasterVolume, true);
        LoadSliderValue(MusicVolume, "Music Volume", Main.RubiconSettings.Audio.MusicVolume, true);
        LoadSliderValue(SFXVolume, "SFX Volume", Main.RubiconSettings.Audio.SFXVolume, true);
        LoadSliderValue(InstVolume, "Inst Volume", Main.RubiconSettings.Audio.InstVolume, true);
        LoadSliderValue(VoiceVolume, "Voice Volume", Main.RubiconSettings.Audio.VoiceVolume, true);
        LoadOptionButtonValue(SoundOutputMode, (int)Main.RubiconSettings.Audio.OutputMode);
    }
}
