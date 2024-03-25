using BaseRubicon.Backend.Autoload.Managers.AudioManager.Enums;

namespace BaseRubicon.Backend.Autoload.Managers.AudioManager;

[Icon("res://assets/miscicons/autoload.png")]
public partial class AudioManager : Node
{
    public static AudioManager Instance { get; private set; }

    [NodePath("MasterVolume/Player")] private AnimationPlayer MasterVolumeAnimPlayer;
    [NodePath("MasterVolume/Panel/Icon")] private AnimatedSprite2D VolumeIcon;
    [NodePath("MasterVolume/Panel/Bar")] private ProgressBar VolumeBar;
    
    private float _musicVolume;
    private float _sfxVolume;
    private float _instVolume;
    private float _voiceVolume;
    private float _masterVolume;
    
    private float preMuteVolume = 50;
    private float targetVolume;
    
    private bool isMuted;
    private bool isMasterVolumeBarShown;

    private double animationTimer;
    private const double AnimationDuration = 2.0;

    [NodePath("VolumeManagerPanel/Player")] private AnimationPlayer VolumePanelAnimPlayer;
    [NodePath("VolumeManagerPanel/Container/Master/HSlider")] private HSlider MasterVolumeSlider;
    [NodePath("VolumeManagerPanel/Container/Music/HSlider")] private HSlider MusicVolumeSlider; 
    [NodePath("VolumeManagerPanel/Container/SFX/HSlider")] private HSlider SFXVolumeSlider;
    [NodePath("VolumeManagerPanel/Container/Inst/HSlider")] private HSlider InstVolumeSlider;
    [NodePath("VolumeManagerPanel/Container/Voices/HSlider")] private HSlider VoicesVolumeSlider;

    private bool isVolumePanelShown;

    public float MasterVolume
    {
        get => _masterVolume;
        set
        {
            _masterVolume = Mathf.Clamp(value, 0, 100);
            VolumeChange(_masterVolume, 0);
        }
    }

    public float MusicVolume
    {
        get => _musicVolume;
        set
        {
            _musicVolume = Mathf.Clamp(value, 0, 100);
            VolumeChange(_musicVolume, 1);
        }
    }

    public float SFXVolume
    {
        get => _sfxVolume;
        set
        {
            _sfxVolume = Mathf.Clamp(value, 0, 100);
            VolumeChange(_sfxVolume, 2);
        }
    }

    public float InstVolume
    {
        get => _instVolume;
        set
        {
            _instVolume = Mathf.Clamp(value, 0, 100);
            VolumeChange(_instVolume, 3);
        }
    }

    public float VoiceVolume
    {
        get => _voiceVolume;
        set
        {
            _voiceVolume = Mathf.Clamp(value, 0, 100);
            VolumeChange(_voiceVolume, 4);
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

        //Global.Settings.Audio.MasterVolume = value;
        //Global.Settings.SaveSettings();
        //Global.Settings.Audio.MusicVolume = value;
        //Global.Settings.SaveSettings();
        //Global.Settings.Audio.SFXVolume = value;
        //Global.Settings.SaveSettings();
        //Global.Settings.Audio.InstVolume = value;
        //Global.Settings.SaveSettings();
        //Global.Settings.Audio.VoiceVolume = value;
        //Global.Settings.SaveSettings();
        
        MasterVolumeSlider.Value = Global.Settings.Audio.MasterVolume;
        MusicVolumeSlider.Value = Global.Settings.Audio.MusicVolume;
        SFXVolumeSlider.Value = Global.Settings.Audio.SFXVolume;
        InstVolumeSlider.Value = Global.Settings.Audio.InstVolume;
        VoicesVolumeSlider.Value = Global.Settings.Audio.VoiceVolume;
        
        MasterVolumeSlider.ValueChanged += value => MasterVolume = (float)value;
        MusicVolumeSlider.ValueChanged += value => MusicVolume = (float)value;
        SFXVolumeSlider.ValueChanged += value => SFXVolume = (float)value;
        InstVolumeSlider.ValueChanged += value => InstVolume = (float)value;
        VoicesVolumeSlider.ValueChanged += value => VoiceVolume = (float)value;

        var Audio = Global.Settings.Audio;
        foreach (var volumeSetting in new[] { Audio.MasterVolume, Audio.SFXVolume, Audio.InstVolume, Audio.MusicVolume, Audio.VoiceVolume}) 
            VolumeChange(volumeSetting);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        VolumeBar.Value = Mathf.Lerp(VolumeBar.Value, targetVolume, delta * 10);
        
        if (Input.IsActionJustPressed("master_volume_up") && !isVolumePanelShown)
        {
            VolumeChange(Global.Settings.Audio.MasterVolume + 10);
            targetVolume = Global.Settings.Audio.MasterVolume;
            PlayVolumeAnimation();
        }
        else if (Input.IsActionJustPressed("master_volume_down") && !isVolumePanelShown)
        {
            VolumeChange(Global.Settings.Audio.MasterVolume - 10);
            targetVolume = Global.Settings.Audio.MasterVolume;
            PlayVolumeAnimation();
        }
        else if (Input.IsActionJustPressed("master_volume_mute") && !isVolumePanelShown)
        {
            bool mute = !isMuted;
            VolumeChange(mute ? 0 : preMuteVolume, 0, mute);
            targetVolume = mute ? 0 : preMuteVolume;
            PlayVolumeAnimation();
        }

        void PlayVolumeAnimation()
        {
            animationTimer = 0.0;
            if (isMasterVolumeBarShown && !MasterVolumeAnimPlayer.IsPlaying())
            {
                MasterVolumeAnimPlayer.Play("In");
                isMasterVolumeBarShown = false;
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
            if (isMasterVolumeBarShown && MasterVolumeAnimPlayer.CurrentAnimation == "In")
            {
                MasterVolumeAnimPlayer.Stop();
                MasterVolumeAnimPlayer.Play("Out");
                isMasterVolumeBarShown = false;
            }
        
            VolumePanelAnimPlayer.Play("In");
            isVolumePanelShown = true;
        }
    }


    public void VolumeChange(float value, int busIndex = 0, bool mute = false)
    {
        float volumeToSet = 0;
    
        switch (mute)
        {
            case true when !isMuted:
                preMuteVolume = BusVolumeOf(busIndex);
                volumeToSet = 0;
                isMuted = true;
                break;
            case true:
                volumeToSet = preMuteVolume;
                isMuted = false;
                break;
            default:
                volumeToSet = value; 
                volumeToSet = Mathf.Clamp(volumeToSet, 0, 100);
                BusVolumeOf(busIndex, volumeToSet);
                break;
        }

        float BusVolumeOf(int i, float? newVolume = null)
        {
            float volume = i switch
            {
                0 => newVolume.HasValue ? (MasterVolume = newVolume.Value) : MasterVolume,
                1 => newVolume.HasValue ? (MusicVolume = newVolume.Value) : MusicVolume,
                2 => newVolume.HasValue ? (SFXVolume = newVolume.Value) : SFXVolume,
                3 => newVolume.HasValue ? (InstVolume = newVolume.Value) : InstVolume,
                4 => newVolume.HasValue ? (VoiceVolume = newVolume.Value) : VoiceVolume,
                _ => 0
            };
        
            return volume;
        }
        
        UpdateVolume(busIndex, volumeToSet);
    }

    private void UpdateVolume(int busIndex, float volume)
    {
        float volumeFloat = volume / 100.0f;
        AudioServer.SetBusVolumeDb(busIndex, Global.LinearToDb(volumeFloat));
        AudioServer.SetBusMute(busIndex, volume == 0);

        VolumeIcon.Animation = MasterVolume switch
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

            player = new AudioStreamPlayer()
            {
                Stream = audiostream,
                VolumeDb = Global.LinearToDb(volume),
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
        foreach (var format in Global.AudioFormats)
        {
            string formattedPath = $"res://assets/{type}/{path}.{format}";
            if (ResourceLoader.Exists(formattedPath)) return formattedPath;
        }
        return string.Empty;
    }
}
