using System.Collections.Generic;
using Rubicon.Backend.Autoload.Managers.AudioManager.Enums;

namespace Rubicon.Backend.Autoload.Managers.AudioManager;

[Icon("res://assets/miscicons/autoload.png")]
public partial class AudioManager : Node
{
    public static AudioManager Instance { get; private set; }

    [NodePath("MasterVolume/Player")] private AnimationPlayer MasterVolumeAnimPlayer;
    [NodePath("MasterVolume/Panel/Icon")] private AnimatedSprite2D VolumeIcon;
    [NodePath("MasterVolume/Panel/Bar")] private ProgressBar VolumeBar;
    
    [NodePath("VolumeManagerPanel/Player")] private AnimationPlayer VolumePanelAnimPlayer;
    [NodePath("VolumeManagerPanel/Container/Master")] private Panel MasterVolumePanel;
    [NodePath("VolumeManagerPanel/Container/Music")] private Panel MusicVolumePanel; 
    [NodePath("VolumeManagerPanel/Container/SFX")] private Panel SFXVolumePanel;
    [NodePath("VolumeManagerPanel/Container/Inst")] private Panel InstVolumePanel;
    [NodePath("VolumeManagerPanel/Container/Voices")] private Panel VoicesVolumePanel;
    
    private bool isMuted;
    private bool isMasterVolumeBarShown;
    private bool isVolumePanelShown;
    
    private float targetVolume = 50; 
    private float preMuteVolume = 50;

    private double animationTimer;
    
    private const double AnimationDuration = 2.0;
    private readonly Dictionary<HSlider, float> previousVolumes = new();

    private float _masterVolume;
    public float MasterVolume
    {
        get => _masterVolume;
        set
        {
            _masterVolume = Mathf.Clamp(value, 0, 100);
            ChangeVolume(_masterVolume);
        }
    }

    private float _musicVolume;
    public float MusicVolume
    {
        get => _musicVolume;
        set
        {
            _musicVolume = Mathf.Clamp(value, 0, 100);
            ChangeVolume(_musicVolume, 1);
        }
    }

    private float _sfxVolume;
    public float SFXVolume
    {
        get => _sfxVolume;
        set
        {
            _sfxVolume = Mathf.Clamp(value, 0, 100);
            ChangeVolume(_sfxVolume, 2);
        }
    }

    private float _instVolume;
    public float InstVolume
    {
        get => _instVolume;
        set
        {
            _instVolume = Mathf.Clamp(value, 0, 100);
            ChangeVolume(_instVolume, 3);
        }
    }

    private float _voiceVolume;
    public float VoiceVolume
    {
        get => _voiceVolume;
        set
        {
            _voiceVolume = Mathf.Clamp(value, 0, 100);
            ChangeVolume(_voiceVolume, 4);
        }
    }
    
    public override void _EnterTree()
    {
        base._EnterTree();
        Instance = this;
    }

    public override void _Ready()
    {
        this.OnReady();
        
        var valueTuples = new (Panel panel, Func<float> getVolume, Action<float> setVolume, string name)[]
        {
            (MasterVolumePanel, () => Main.GameSettings.Audio.MasterVolume, volume => { MasterVolume = volume; }, "Master"),
            (MusicVolumePanel, () => Main.GameSettings.Audio.MusicVolume, volume => { MusicVolume = volume; }, "Music"),
            (SFXVolumePanel, () => Main.GameSettings.Audio.SFXVolume, volume => { SFXVolume = volume; }, "SFX"),
            (InstVolumePanel, () => Main.GameSettings.Audio.InstVolume, volume => { InstVolume = volume; }, "Inst"),
            (VoicesVolumePanel, () => Main.GameSettings.Audio.VoiceVolume, volume => { VoiceVolume = volume; }, "Voices")
        };

        foreach (var (panel, getVolume, setVolume, name) in valueTuples)
        {
            var slider = panel.GetNode<HSlider>("HSlider");
            var label = slider.GetNode<Label>("Label");
            var button = panel.GetNode<Button>("Button");
            var buttonSprite = button.GetNode<AnimatedSprite2D>("Icon");
            
            button.Pressed += () => OnVolumeButtonPressed(slider, label, name);
            UpdateButtonSprite(buttonSprite, getVolume());
            UpdateLabel(label, name, getVolume());

            slider.Value = getVolume();
            slider.ValueChanged += value =>
            {
                setVolume((float)value);
                UpdateLabel(label, name, (float)value);
                UpdateButtonSprite(buttonSprite, (float)value);
            };
        }

        void OnVolumeButtonPressed(HSlider slider, Label label, string sliderName) 
        {
            var newValue = slider.Value == 0 ? previousVolumes.TryGetValue(slider, out var volume) ? volume : 50 : 0;
            previousVolumes[slider] = (float)slider.Value;
            slider.Value = newValue;
            label.Text = newValue == 0 ? $"{sliderName} [Muted]" : $"{sliderName} [{(int)newValue}%]";
        }

        var Audio = Main.GameSettings.Audio;
        foreach (var volumeSetting in new[] { Audio.MasterVolume, Audio.SFXVolume, Audio.InstVolume, Audio.MusicVolume, Audio.VoiceVolume}) ChangeVolume(volumeSetting);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        VolumeBar.Value = Mathf.Lerp(VolumeBar.Value, targetVolume, delta * 10);

        if (Input.IsActionJustPressed("master_volume_up") && !isVolumePanelShown)
        {
            float newVolume = Mathf.Clamp(Main.GameSettings.Audio.MasterVolume + 5, 0, 100);
            ChangeVolume(newVolume);
            targetVolume = newVolume;
            PlayVolumeAnimation();
        }
        else if (Input.IsActionJustPressed("master_volume_down") && !isVolumePanelShown)
        {
            float newVolume = Mathf.Clamp(Main.GameSettings.Audio.MasterVolume - 5, 0, 100);
            ChangeVolume(newVolume);
            targetVolume = newVolume;
            PlayVolumeAnimation();
        }
        else if (Input.IsActionJustPressed("master_volume_mute") && !isVolumePanelShown)
        {
            bool mute = !isMuted;
            float newVolume = mute ? 0 : preMuteVolume;
            ChangeVolume(newVolume, 0, mute);
            targetVolume = newVolume;
            isMuted = mute;
            PlayVolumeAnimation();
        }

        MasterVolumePanel.GetNode<HSlider>("HSlider").Value = Main.GameSettings.Audio.MasterVolume;
        UpdateLabel(GetNode<Label>("VolumeManagerPanel/Container/Master/HSlider/Label"), "Master", Main.GameSettings.Audio.MasterVolume);
        UpdateButtonSprite(GetNode<AnimatedSprite2D>("VolumeManagerPanel/Container/Master/Button/Icon"), Main.GameSettings.Audio.MasterVolume);

        void PlayVolumeAnimation()
        {
            animationTimer = 0.0;
            if (!isMasterVolumeBarShown && !MasterVolumeAnimPlayer.IsPlaying())
            {
                MasterVolumeAnimPlayer.Play("In");
                isMasterVolumeBarShown = true;
            }
        }

        if (isMasterVolumeBarShown)
        {
            animationTimer += delta;
            if (animationTimer >= AnimationDuration)
            {
                MasterVolumeAnimPlayer.Play("Out");
                isMasterVolumeBarShown = false;
            }
        }
    }
    
    private void UpdateLabel(Label label, string sliderName, float volume) => label.Text = volume == 0 ? $"{sliderName} [Muted]" : $"{sliderName} [{(int)volume}%]";
    
    private void UpdateButtonSprite(AnimatedSprite2D buttonSprite, float volume)
    {
        buttonSprite.Play(volume switch
        {
            0 => "mute",
            < 50 => "mid",
            _ => "full"
        });
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey { KeyLabel: Key.Escape } && isVolumePanelShown)
        {
            VolumePanelAnimPlayer.Play("Out");
            isVolumePanelShown = false;
            return;
        }

        if (Input.IsActionPressed("open_volume_manager") && !isVolumePanelShown && !VolumePanelAnimPlayer.IsPlaying())
        {
            VolumePanelAnimPlayer.Play("In");
            isVolumePanelShown = true;
        }
    }
    
    public void ChangeVolume(float value, int busIndex = 0, bool mute = false)
    {
        float volumeToSet;
        
        switch (mute)
        {
            case true when !isMuted:
                preMuteVolume = GetBusVolume(busIndex);
                volumeToSet = 0;
                isMuted = true;
                break;
            case true:
                volumeToSet = 0;
                isMuted = false;
                break;
            default:
                volumeToSet = Mathf.Clamp(value, 0, 100);
                SetBusVolume(busIndex, volumeToSet);
                break;
        }

        float GetBusVolume(int i)
        {
            return i switch
            {
                0 => _masterVolume,
                1 => _musicVolume,
                2 => _sfxVolume,
                3 => _instVolume,
                4 => _voiceVolume,
                _ => 0
            };
        }
        
        void SetBusVolume(int i, float volume)
        {
            switch (i)
            {
                case 0:
                    _masterVolume = volume;
                    Main.GameSettings.Audio.MasterVolume = volume;
                    break;
                case 1:
                    _musicVolume = volume;
                    Main.GameSettings.Audio.MusicVolume = volume;
                    break;
                case 2:
                    _sfxVolume = volume;
                    Main.GameSettings.Audio.SFXVolume = volume;
                    break;
                case 3:
                    _instVolume = volume;
                    Main.GameSettings.Audio.InstVolume = volume;
                    break;
                case 4:
                    _voiceVolume = volume;
                    Main.GameSettings.Audio.VoiceVolume = volume;
                    break;
            }
        }

        Main.GameSettings.Save();
        UpdateVolume(busIndex, volumeToSet);
    }

    private void UpdateVolume(int busIndex, float volume)
    {
        float volumeFloat = volume / 100.0f;
        AudioServer.SetBusVolumeDb(busIndex, Main.LinearToDb(volumeFloat));
        AudioServer.SetBusMute(busIndex, volume == 0);

        VolumeIcon.Animation = volume switch
        {
            > 50 => "full",
            > 0 => "mid",
            _ => "mute"
        };
    }

    public AudioStreamPlayer PlayAudio(AudioType type, string path, float volume = 1, bool loop = false)
    {
        if (type == AudioType.Music)
        {
            foreach(var node in GetNode(type.ToString().ToLower()).GetChildren())
            {
                var playerBullshit = (AudioStreamPlayer)node;
                playerBullshit.Stop();
            }   
        }

        var player = GetNodeOrNull<AudioStreamPlayer>($"{type.ToString().ToLower()}/{path}");
        if (player != null && type == AudioType.Music && player.Playing && player.Stream == GD.Load<AudioStream>(ConstructAudioPath(path, type.ToString().ToLower()))) return player;

        if (player is null)
        {
            string finalPath = ConstructAudioPath(path, type.ToString().ToLower());
            if (string.IsNullOrEmpty(finalPath)) return null;
            var audiostream = GD.Load<AudioStream>(finalPath);

            player = new()
            {
                Stream = audiostream,
                VolumeDb = Main.LinearToDb(volume),
                Autoplay = true,
            };
        
            GetNode<Node>($"{type.ToString().ToLower()}/{path}").AddChild(player);
            
            player.Finished += () => player.QueueFree();
        }

        player.Finished += () =>
        {
            if (loop && type == AudioType.Music) player.Play();
        };

        player.Play();
        return player;
    }

    public void StopAudio(AudioType type, string path)
    {
        var player = GetNodeOrNull<AudioStreamPlayer>($"{type.ToString().ToLower()}/{path}");
        player?.Stop();
        player?.QueueFree();
    }

    private static string ConstructAudioPath(string path, string type)
    {
        foreach (var format in Main.AudioFormats)
        {
            string formattedPath = $"res://assets/{type}/{path}.{format}";
            if (ResourceLoader.Exists(formattedPath)) return formattedPath;
        }
        return string.Empty;
    }
}
