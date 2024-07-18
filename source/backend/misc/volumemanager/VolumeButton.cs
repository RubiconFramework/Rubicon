using Godot;
using System;
using Rubicon.backend.autoload;

namespace Rubicon.backend.misc.volumemanager;
[GlobalClass]
public partial class VolumeButton : Panel
{
    [Export] public VolumeType VolumeType { get; set; }

    [NodePath("Button")] private Button button;
    [NodePath("Button/Icon")] private AnimatedSprite2D buttonIcon;
    [NodePath("HSlider")] private HSlider slider;
    [NodePath("HSlider/Label")] private Label label;

    private float previousVolume = 50f;
    private bool isMuted;

    [Signal]
    public delegate void VolumeChangedEventHandler(float value, string volumeType);

    public override void _Ready()
    {
        this.OnReady();
        button.Pressed += OnButtonPressed;
        slider.ValueChanged += OnSliderValueChanged;

        SetVolume(GetCurrentVolume());
    }

    public void SetVolume(float value)
    {
        slider.Value = value;
        UpdateLabel();
        UpdateButtonSprite();
        ApplyVolume(value);
    }

    private void OnButtonPressed() => ToggleMute();

    private void OnSliderValueChanged(double value)
    {
        UpdateLabel();
        UpdateButtonSprite();
        ApplyVolume((float)value);
        EmitSignal(SignalName.VolumeChanged, value, VolumeType.ToString());
    }

    private void UpdateLabel() => label.Text = $"{VolumeType.ToString()} [{(slider.Value == 0 ? "Muted" : $"{(int)slider.Value}%")}]";

    private void UpdateButtonSprite()
    {
        buttonIcon.Play(slider.Value switch
        {
            0 => "mute",
            < 50 => "mid",
            _ => "full"
        });
    }

    private void ToggleMute()
    {
        if (isMuted)
        {
            isMuted = false;
            SetVolume(previousVolume);
        }
        else
        {
            previousVolume = (float)slider.Value;
            isMuted = true;
            SetVolume(0);
        }
    }

    private void ApplyVolume(float volume)
    {
        int busIndex = (int)VolumeType;
        AudioServer.SetBusVolumeDb(busIndex, Mathf.LinearToDb(volume / 100.0f));
        AudioServer.SetBusMute(busIndex, volume == 0);

        switch (VolumeType)
        {
            case VolumeType.Master:
                RubiconSettings.Audio.MasterVolume = volume;
                break;
            case VolumeType.Music:
                RubiconSettings.Audio.MusicVolume = volume;
                break;
            case VolumeType.SFX:
                RubiconSettings.Audio.SFXVolume = volume;
                break;
            case VolumeType.Inst:
                RubiconSettings.Audio.InstVolume = volume;
                break;
            case VolumeType.Voice:
                RubiconSettings.Audio.VoiceVolume = volume;
                break;
        }
        RubiconSettings.Save();
    }

    private float GetCurrentVolume()
    {
        return VolumeType switch
        {
            VolumeType.Master => RubiconSettings.Audio.MasterVolume,
            VolumeType.Music => RubiconSettings.Audio.MusicVolume,
            VolumeType.SFX => RubiconSettings.Audio.SFXVolume,
            VolumeType.Inst => RubiconSettings.Audio.InstVolume,
            VolumeType.Voice => RubiconSettings.Audio.VoiceVolume,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
