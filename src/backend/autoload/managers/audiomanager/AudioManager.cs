using BaseRubicon.Backend.Scripts;
using BaseRubicon.Scenes.Options.Submenus.Audio.Enums;

namespace BaseRubicon.Backend.Autoload.Managers.AudioManager;

[Icon("res://assets/miscicons/autoload.png")]
public partial class AudioManager : Node
{
    [NodePath("VolumeManager/AnimationPlayer")] private AnimationPlayer AnimationPlayer;
    [NodePath("VolumeManager/dumbass panel/VolumeIcon")] private AnimatedSprite2D VolumeIcon;
    [NodePath("VolumeManager/dumbass panel/VolumeBar")] private ProgressBar VolumeBar;
    
    private float MasterVolume;
    private float preMuteVolume = 50;
    private float targetVolume;
    
    private bool isMuted;
    private VolumeBarState CurrentVolumeBarState = VolumeBarState.VolumeBarHidden;

    private double animationTimer;
    private const double AnimationDuration = 2.0;

    public static AudioManager Instance { get; private set; }
    
    public override void _EnterTree()
    {
        base._EnterTree();
        Instance = this;
    }

    public override void _Ready()
    {
        this.OnReady();
        VolumeChange(Global.Settings.Audio.MasterVolume);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        VolumeBar.Value = Mathf.Lerp(VolumeBar.Value, targetVolume, delta * 10);

        if (Input.IsActionJustPressed("volume_up"))
        {
            VolumeChange(Global.Settings.Audio.MasterVolume + 10);
            targetVolume = Global.Settings.Audio.MasterVolume;
            PlayVolumeAnimation();
        }
        else if (Input.IsActionJustPressed("volume_down"))
        {
            VolumeChange(Global.Settings.Audio.MasterVolume - 10);
            targetVolume = Global.Settings.Audio.MasterVolume;
            PlayVolumeAnimation();
        }
        else if (Input.IsActionJustPressed("volume_mute"))
        {
            VolumeMute();
            targetVolume = isMuted ? 0 : preMuteVolume;
            PlayVolumeAnimation();
        }

        void PlayVolumeAnimation()
        {
            animationTimer = 0.0;
            if (CurrentVolumeBarState == VolumeBarState.VolumeBarHidden && !AnimationPlayer.IsPlaying())
            {
                AnimationPlayer.Play("In");
                CurrentVolumeBarState = VolumeBarState.VolumeBarShown;
            }
        }

        if (CurrentVolumeBarState == VolumeBarState.VolumeBarShown)
        {
            animationTimer += delta;
            if (animationTimer >= AnimationDuration)
            {
                AnimationPlayer.Play("Out");
                CurrentVolumeBarState = VolumeBarState.VolumeBarHidden;
            }
        }
    }

    public void VolumeChange(float value)
    {
        if (isMuted) VolumeMute();
        MasterVolume = value; 
        MasterVolume = Mathf.Clamp(MasterVolume, 0, 100);
        Global.Settings.Audio.MasterVolume = MasterVolume;
        Global.Settings.SaveSettings();
        UpdateVolume();
    }

    private void VolumeMute()
    {
        if (!isMuted)
        {
            preMuteVolume = MasterVolume;
            MasterVolume = 0;
            isMuted = true;
        }
        else
        {
            MasterVolume = preMuteVolume;
            isMuted = false;
        }
        UpdateVolume();
    }

    private void UpdateVolume()
    {
        float volumeFloat = MasterVolume / 100.0f;
        AudioServer.SetBusVolumeDb(0, Global.LinearToDb(volumeFloat));
        AudioServer.SetBusMute(0, MasterVolume == 0);

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
