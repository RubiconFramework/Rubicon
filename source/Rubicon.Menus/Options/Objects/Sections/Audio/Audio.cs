using Godot;
using Godot.Sharp.Extras;
using Rubicon.Autoload.API;
using Rubicon.Data;

namespace Rubicon.Menus.Options.Objects.Sections.audio;

public partial class Audio : SettingsSectionBase
{
    [NodePath("Container/AudioOutputMode")] private OptionButton AudioOutputMode;
    [NodePath("Container/MasterVolume")] private Label MasterVolume;
    [NodePath("Container/MusicVolume")] private Label MusicVolume;
    [NodePath("Container/SFXVolume")] private Label SFXVolume;
    [NodePath("Container/InstVolume")] private Label InstVolume;
    [NodePath("Container/VoiceVolume")] private Label VoiceVolume;

    public override void _Ready()
    {
        base._Ready();
        this.OnReady();
        
        SetupSlider(MasterVolume, "Master Volume", v => HelperMethods.SetVolume(VolumeType.Master, v), SaveData.Audio.MasterVolume, true);
        SetupSlider(MusicVolume, "Music Volume", v => HelperMethods.SetVolume(VolumeType.Music, v), SaveData.Audio.MusicVolume, true);
        SetupSlider(SFXVolume, "SFX Volume", v => HelperMethods.SetVolume(VolumeType.SFX, v), SaveData.Audio.SFXVolume, true);
        SetupSlider(InstVolume, "Inst Volume", v => HelperMethods.SetVolume(VolumeType.Inst, v), SaveData.Audio.InstVolume, true);
        SetupSlider(VoiceVolume, "Voice Volume", v => HelperMethods.SetVolume(VolumeType.Voice, v), SaveData.Audio.VoiceVolume, true);
        SetupOptionButton(AudioOutputMode, i => SaveData.Audio.AudioOutputMode = (AudioOutputMode)i, (int)SaveData.Audio.AudioOutputMode);
    }
}
