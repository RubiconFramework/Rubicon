using System.Collections.Generic;
using Godot.Collections;
using Rubicon.Backend.Autoload;
using Rubicon.backend.autoload.enums;
using AudioManager = Rubicon.backend.autoload.AudioManager;
using FreeplaySong = Rubicon.menus.freeplay.resources.FreeplaySong;

namespace Rubicon.menus.freeplay;

public partial class FreeplayMenu : Node
{
    [NodePath("SongData/Panel/Buttons/PlayButton")] private Button PlayButton;
    [NodePath("SongData/Panel/Buttons/DifficultiesButton")] private OptionButton DifficultyOptionButton;

    [NodePath("SongData/AnimPlayer")] private AnimationPlayer SongDataAnimPlayer;
    
    [NodePath("GameplayModifiersLabel/AnimPlayer")] private AnimationPlayer GameplayModifiersAnimPlayer;
    private FreeplaySong CurrentFreeplaySong;
    private Tween ColorTween;

    [NodePath("SongData/Panel/Stats/SongScore")] private Label SongScore;
    [NodePath("SongData/Panel/Stats/SongDescription")] private Label SongDescription;
    [NodePath("SongData/Panel/Stats/SongDisplayName")] private Label SongDisplayName;
    [NodePath("BG")] private Sprite2D BackgroundSprite;
    [NodePath("Camera")] private Camera2D Camera;
    [Export] private Array<FreeplaySong> Songs = new();
    private System.Collections.Generic.Dictionary<FreeplaySong, Node> songAlphabets = new System.Collections.Generic.Dictionary<FreeplaySong, Node>();
    private bool isCameraBopping;
    private bool isGameplayModifiers;
    
    [NodePath("SongList")] private Control SongList;
    private int selectedSongIndex;
    private string selectedDifficultyIndex;
    private int maxDisplayedSongs = 7;
    
    public override void _Ready()
    {
        this.OnReady();

        foreach (Node child in new List<Node>(SongList.GetChildren()))
        {
            child.QueueFree();
            SongList.RemoveChild(child);
        }

        songAlphabets.Clear();

        /*PackedScene alphabetScene = GD.Load<PackedScene>(Main.Instance.alphabetScene.ResourcePath);
        for (int i = 0; i < Songs.Count; i++)
        {
            FreeplaySong song = Songs[i];
            Node alphabetNode = alphabetScene.Instantiate();
            GodotObject alphabet = alphabetNode;
            alphabet.Set("text", song.SongDisplayName);
            alphabet.Set("is_menu_item", true);
            songAlphabets[song] = alphabetNode;
            SongList.AddChild(alphabetNode);
        }*/

        UpdateSelection(0);
        PlayButton.Pressed += OnPlayButtonPressed;
        DifficultyOptionButton.ItemSelected += OnDifficultyButtonSelected;
        Conductor.OnBeatHit += OnBeatHit;
    }

    private int OnBeatHit(int beat)
    {
        if (isCameraBopping) Camera.Zoom = new(1.03f, 1.03f);
        return beat;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey eventKey)
        {
            switch (eventKey.Pressed)
            {
                case true when eventKey.Keycode == Key.Down && !isGameplayModifiers:
                    UpdateSelection(1);
                    break;
                case true when eventKey.Keycode == Key.Up && !isGameplayModifiers:
                    UpdateSelection(-1);
                    break;
                case true when eventKey.Keycode == Key.Enter && !isGameplayModifiers:
                    OnPlayButtonPressed();
                    break;
                case true when eventKey.Keycode == Key.Space && !isGameplayModifiers:
                    isGameplayModifiers = true;
                    GameplayModifiersAnimPlayer.Play("Start");
                    ColorTween = GetTree().CreateTween();
                    ColorTween.TweenProperty(BackgroundSprite, "modulate", new Color(0.47f, 0.47f, 0.47f), 1);
                    break;
                case true when eventKey.Keycode == Key.Escape && isGameplayModifiers:
                    isGameplayModifiers = false;
                    GameplayModifiersAnimPlayer.Play("End");
                    ColorTween = GetTree().CreateTween();
                    ColorTween.TweenProperty(BackgroundSprite, "modulate", CurrentFreeplaySong.BackgroundColor, 1);
                    break;
            }
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Camera.Zoom = new(Mathf.Lerp(Camera.Zoom.X, 1, Mathf.Clamp(((float)delta * 3), 0f, 1f)),Mathf.Lerp(Camera.Zoom.X, 1, Mathf.Clamp(((float)delta * 3), 0f, 1f)));
    }

    private void UpdateSelection(int direction)
    {
        foreach (var entry in songAlphabets)
        {
            GodotObject alphabet = entry.Value;
            alphabet.Set("target_y", GetTargetY(entry.Key, (selectedSongIndex + direction + songAlphabets.Count) % songAlphabets.Count));
        }

        AudioManager.Play(AudioType.Sounds, "menus/scrollMenu");
        selectedSongIndex = (selectedSongIndex + direction + songAlphabets.Count) % songAlphabets.Count;
        UpdateSongData(Songs[selectedSongIndex]);
        SongDataAnimPlayer.Stop();
        SongDataAnimPlayer.Play("ChangePanelSong");
    }

    private int GetTargetY(FreeplaySong song, int selectedIndex)
    {
        int distance = Mathf.Abs(Songs.IndexOf(song) - selectedIndex);
        return distance <= maxDisplayedSongs / 2 ? distance : maxDisplayedSongs / 2;
    }

    private void UpdateSongData(FreeplaySong song)
    {
        CurrentFreeplaySong = song;
        SongDisplayName.Text = song.SongDisplayName;
        SongDescription.Text = string.IsNullOrEmpty(song.SongDescription) ? "" : song.SongDescription;
        SongScore.Text = "TBA";
        DifficultyOptionButton.Clear();
        foreach (string difficulty in song.Difficulties) DifficultyOptionButton.AddItem($" {difficulty}");
        isCameraBopping = song.IsCameraBopping;
        Conductor.BPM = song.BPM;
        ColorTween = GetTree().CreateTween();
        ColorTween.TweenProperty(BackgroundSprite, "modulate", song.BackgroundColor, 1);
    }

    private void OnPlayButtonPressed()
    {
        ChartHandler.NewChart(CurrentFreeplaySong.SongName, selectedDifficultyIndex);
        LoadingHandler.ChangeScene("res://src/scenes/gameplay/Gameplay.tscn");
    }

    private void OnDifficultyButtonSelected(long index)
    {
        if (index < 0 || index >= CurrentFreeplaySong.Difficulties.Count) return;
        string selectedDifficulty = CurrentFreeplaySong.Difficulties[(int)index];
        selectedDifficultyIndex = selectedDifficulty;
    }
}
