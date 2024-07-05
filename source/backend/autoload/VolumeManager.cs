/*using System.Collections.Generic;

namespace Rubicon.backend.autoload;

[Icon("res://assets/miscicons/autoload.png")]
public partial class VolumeManager : CanvasLayer
{
	[NodePath("Player")] private AnimationPlayer AnimPlayer;

	[NodePath("MasterVolumeLabel")] private Label MasterVolumeLabel;
	[NodePath("MasterVolumeLabel/Icon")] private AnimatedSprite2D MasterVolumeIcon;
    [NodePath("MasterVolumeLabel/Bar")] private ProgressBar MasterVolumeBar;
	
    [NodePath("Containers/Master")] private Panel MasterVolumePanel;
    [NodePath("Containers/Music")] private Panel MusicVolumePanel; 
    [NodePath("Containers/SFX")] private Panel SFXVolumePanel;
    [NodePath("Containers/Inst")] private Panel InstVolumePanel;
    [NodePath("Containers/Voices")] private Panel VoiceVolumePanel;

    private float _masterVolume;
    public float MasterVolume
    {
	    get => _masterVolume;
	    set => ChangeVolume(value);
    }

    private float _musicVolume;
    public float MusicVolume
    {
	    get => _musicVolume;
	    set => ChangeVolume(value, 1);
    }

    private float _sfxVolume;
    public float SFXVolume
    {
	    get => _sfxVolume;
	    set => ChangeVolume(value, 2);
    }

    private float _instVolume;
    public float InstVolume
    {
	    get => _instVolume;
	    set => ChangeVolume(value, 3);
    }

    private float _voiceVolume;
    public float VoiceVolume
    {
	    get => _voiceVolume;
	    set => ChangeVolume(value, 4);
    }
    
    private const double AnimationDuration = 2.0;
    private readonly Dictionary<HSlider, float> previousVolumes = new();
    
    private bool isMuted;
    private bool isMasterVolumeBarShown;
    private bool isVolumePanelShown;
    private float preMuteVolume = 50;
    private double animationTimer;

    public static VolumeManager Instance { get; private set; }

    public override void _EnterTree() => Instance = this;

    public override void _Ready()
    {
	    this.OnReady();
	    
	    var valueTuples = new (Panel panel, Func<float> getVolume, Action<float> setVolume, string name)[]
        {
            (MasterVolumePanel, () => OldRubicon.Main.RubiconSettings.Audio.MasterVolume, volume => { MasterVolume = volume; }, "Master"),
            (MusicVolumePanel, () => OldRubicon.Main.RubiconSettings.Audio.MusicVolume, volume => { MusicVolume = volume; }, "Music"),
            (SFXVolumePanel, () => OldRubicon.Main.RubiconSettings.Audio.SFXVolume, volume => { SFXVolume = volume; }, "SFX"),
            (InstVolumePanel, () => OldRubicon.Main.RubiconSettings.Audio.InstVolume, volume => { InstVolume = volume; }, "Inst"),
            (VoiceVolumePanel, () => OldRubicon.Main.RubiconSettings.Audio.VoiceVolume, volume => { VoiceVolume = volume; }, "Voices")
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

    public override void _PhysicsProcess(double delta)
    {
	    base._PhysicsProcess(delta);
	    if (MasterVolumeLabel.Visible)
		    MasterVolumeBar.Value = Mathf.Lerp(MasterVolumeBar.Value, OldRubicon.Main.RubiconSettings.Audio.MasterVolume, delta * 5);

	    if (Input.IsActionJustPressed("master_volume_up") && !isVolumePanelShown)
	    {
		    float newVolume = Mathf.Clamp(OldRubicon.Main.RubiconSettings.Audio.MasterVolume + 10, 0, 100);
		    MasterVolumeLabel.Text = $"Master Volume {newVolume}%";
		    ChangeVolume(newVolume);
		    PlayVolumeAnimation();
	    }
	    else if (Input.IsActionJustPressed("master_volume_down") && !isVolumePanelShown)
	    {
		    float newVolume = Mathf.Clamp(OldRubicon.Main.RubiconSettings.Audio.MasterVolume - 10, 0, 100);
		    MasterVolumeLabel.Text = $"Master Volume {newVolume}%";
		    ChangeVolume(newVolume);
		    PlayVolumeAnimation();
	    }
	    else if (Input.IsActionJustPressed("master_volume_mute") && !isVolumePanelShown)
	    {
		    bool mute = !isMuted;
		    float newVolume = mute ? 0 : preMuteVolume;
		    ChangeVolume(newVolume, 0, mute);
		    isMuted = mute;
		    MasterVolumeLabel.Text = $"Master Volume Muted";
		    PlayVolumeAnimation();
	    }
	    
	    GetNode<HSlider>("Containers/Master/HSlider").Value = OldRubicon.Main.RubiconSettings.Audio.MasterVolume;
	    UpdateLabel(GetNode<Label>("Containers/Master/HSlider/Label"), "Master", OldRubicon.Main.RubiconSettings.Audio.MasterVolume);
	    UpdateButtonSprite(GetNode<AnimatedSprite2D>("Containers/Master/Button/Icon"), OldRubicon.Main.RubiconSettings.Audio.MasterVolume);

	    void PlayVolumeAnimation()
	    {
		    animationTimer = 0.0;
		    if (!isMasterVolumeBarShown && !AnimPlayer.IsPlaying())
		    {
			    AnimPlayer.Play("MasterPanel/In");
			    isMasterVolumeBarShown = true;
		    }
	    }

	    if (isMasterVolumeBarShown)
	    {
		    animationTimer += delta;
		    if (animationTimer >= AnimationDuration)
		    {
			    AnimPlayer.Play("MasterPanel/Out");
			    isMasterVolumeBarShown = false;
		    }
	    }
    }

    public override void _Input(InputEvent @event)
    {
	    if (@event is InputEventKey { KeyLabel: Key.Escape } && isVolumePanelShown)
	    {
		    AnimPlayer.Play("AllPanels/Out");
		    isVolumePanelShown = false;
		    return;
	    }

	    if (Input.IsActionPressed("open_volume_manager") && !isVolumePanelShown && !AnimPlayer.IsPlaying())
	    {
		    AnimPlayer.Play("AllPanels/In");
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
				    OldRubicon.Main.RubiconSettings.Audio.MasterVolume = volume;
				    break;
			    case 1:
				    _musicVolume = volume;
				    OldRubicon.Main.RubiconSettings.Audio.MusicVolume = volume;
				    break;
			    case 2:
				    _sfxVolume = volume;
				    OldRubicon.Main.RubiconSettings.Audio.SFXVolume = volume;
				    break;
			    case 3:
				    _instVolume = volume;
				    OldRubicon.Main.RubiconSettings.Audio.InstVolume = volume;
				    break;
			    case 4:
				    _voiceVolume = volume;
				    OldRubicon.Main.RubiconSettings.Audio.VoiceVolume = volume;
				    break;
		    }
	    }

	    OldRubicon.Main.RubiconSettings.Save();
	    UpdateVolume(busIndex, volumeToSet);
    }

    private void UpdateVolume(int busIndex, float volume)
    {
	    float volumeFloat = volume / 100.0f;
	    AudioServer.SetBusVolumeDb(busIndex, AudioManager.LinearToDB(volumeFloat));
	    AudioServer.SetBusMute(busIndex, volume == 0);
	    
	    UpdateButtonSprite(MasterVolumeIcon, volume);
    }
}
*/
