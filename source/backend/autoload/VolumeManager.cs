using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Rubicon.backend.autoload;
using Rubicon.backend.misc.volumemanager;

namespace Rubicon.backend.autoload;
public enum VolumeType
{
    Master,
    Music,
    SFX,
    Inst,
    Voice
}

public partial class VolumeManager : CanvasLayer
{
    [NodePath("Player")] private AnimationPlayer animPlayer;
    [NodePath("MasterVolumeLabel")] private Label masterVolumeLabel;
    [NodePath("MasterVolumeLabel/Icon")] private AnimatedSprite2D masterVolumeIcon;
    [NodePath("MasterVolumeLabel/Bar")] private ProgressBar masterVolumeBar;
    [NodePath("VolumePanel")] private Panel volumePanel;
    [NodePath("VolumePanel/Containers")] private VBoxContainer volumeContainer;

    public readonly Dictionary<VolumeType, VolumeButton> volumeButtons = new();

    private const double AnimationDuration = 2.0;
    private bool isMasterVolumeBarShown;
    private bool isVolumePanelShown;
    private double animationTimer;
    private bool muteAnimationPlayed;

    public static VolumeManager Instance { get; private set; }

    public override void _EnterTree() => Instance = this;

    public override void _Ready()
    {
        this.OnReady();

        var master = volumePanel.GetNode<VolumeButton>("Master");
        master.VolumeChanged += OnVolumeChanged;
        volumeButtons[master.VolumeType] = master;

        foreach (var node in volumeContainer.GetChildren<VolumeButton>()) 
            volumeButtons[node.VolumeType] = node;
    }

    private void OnVolumeChanged(float value, string volumeType)
    {   
        if ((VolumeType)Enum.Parse(typeof(VolumeType), volumeType) == VolumeType.Master) 
            UpdateMasterVolumeDisplay(value);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (masterVolumeLabel.Visible)
            masterVolumeBar.Value = Mathf.Lerp((float)masterVolumeBar.Value, RubiconSettings.Audio.MasterVolume, (float)delta * 10);

        if (Input.IsActionJustPressed("volume_up") && !isVolumePanelShown)
        {
            float newVolume = Mathf.Clamp(RubiconSettings.Audio.MasterVolume + 10, 0, 100);
            volumeButtons[VolumeType.Master].SetVolume(newVolume);
            PlayVolumeAnimation();
        }
        else if (Input.IsActionJustPressed("volume_down") && !isVolumePanelShown)
        {
            float newVolume = Mathf.Clamp(RubiconSettings.Audio.MasterVolume - 10, 0, 100);
            volumeButtons[VolumeType.Master].SetVolume(newVolume);
            PlayVolumeAnimation();
        }
        else if (Input.IsActionJustPressed("volume_mute") && !isVolumePanelShown)
        {
            volumeButtons[VolumeType.Master].SetVolume(0);
            PlayVolumeAnimation();
        }
    }

    private void PlayVolumeAnimation()
    {
        animationTimer = 0.0;
        if (!isMasterVolumeBarShown && !animPlayer.IsPlaying())
        {
            animPlayer.Play("MasterPanel/In");
            isMasterVolumeBarShown = true;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey { KeyLabel: Key.Escape } && isVolumePanelShown)
        {
            animPlayer.Play("AllPanels/Out");
            isVolumePanelShown = false;
            return;
        }

        if (Input.IsActionPressed("volume_open") && !isVolumePanelShown && !animPlayer.IsPlaying())
        {
            animPlayer.Play("AllPanels/In");
            isVolumePanelShown = true;
        }
    }

    private void UpdateMasterVolumeDisplay(float volume)
    {
        masterVolumeLabel.Text = volume == 0 ? "Muted" : $"{volume}%";
        masterVolumeIcon.Play(volume switch
        {
            0 => "mute",
            < 50 => "mid",
            _ => "full"
        });

        switch (volume)
        {
            case 0 when !muteAnimationPlayed:
                animPlayer.Play("AllPanels/MasterVolumeMuted");
                muteAnimationPlayed = true;
                break;
            case > 0 when muteAnimationPlayed:
                animPlayer.Play("AllPanels/MasterVolumeUnmuted");
                muteAnimationPlayed = false;
                break;
        }
    }
}
