using Rubicon.Backend.Autoload;
using Rubicon.backend.autoload.enums;
using Rubicon.menus.options.objects.sections;

public partial class Audio : SettingsSectionBase
{
    [NodePath("Container/AudioOutputMode")]
    private OptionButton AudioOutputMode;

    [NodePath("Container/MasterVolume")]
    private Label MasterVolume;

    [NodePath("Container/MusicVolume")]
    private Label MusicVolume;

    [NodePath("Container/SFXVolume")]
    private Label SFXVolume;

    [NodePath("Container/InstVolume")]
    private Label InstVolume;

    [NodePath("Container/VoiceVolume")]
    private Label VoiceVolume;

    public override void _Ready()
    {
        base._Ready();
        this.OnReady();
        SetupControls();
    }

    private void SetupControls()
    {
        SetupSlider(MasterVolume, "Master Volume", 
            v => HelperMethods.SetVolume(VolumeType.MasterVolume, v), RubiconSettings.Audio.MasterVolume, true);

        SetupSlider(MusicVolume, "Music Volume", 
            v => HelperMethods.SetVolume(VolumeType.MusicVolume, v), RubiconSettings.Audio.MusicVolume, true);

        SetupSlider(SFXVolume, "SFX Volume", 
            v => HelperMethods.SetVolume(VolumeType.SFXVolume, v), RubiconSettings.Audio.SFXVolume, true);

        SetupSlider(InstVolume, "Inst Volume", 
            v => HelperMethods.SetVolume(VolumeType.InstVolume, v), RubiconSettings.Audio.InstVolume, true);

        SetupSlider(VoiceVolume, "Voice Volume", 
            v => HelperMethods.SetVolume(VolumeType.VoiceVolume, v), RubiconSettings.Audio.VoiceVolume, true);

        SetupOptionButton(AudioOutputMode, 
            i => RubiconSettings.Audio.AudioOutputMode = (AudioOutputMode)i, (int)RubiconSettings.Audio.AudioOutputMode);
    }
}
