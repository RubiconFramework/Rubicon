using System.Collections.Generic;
using System.Linq;
using Rubicon.backend.common.enums;
using Rubicon.gameplay.elements.classes.song;
using Rubicon.gameplay.elements.resources;
using Rubicon.gameplay.elements.scripts;
using Rubicon.gameplay.elements.strumlines;
using AudioManager = Rubicon.backend.autoload.managers.AudioManager;
using Global = Rubicon.backend.autoload.Global;
using Stage = Rubicon.backend.scripts.Stage;

namespace Rubicon.gameplay;

public partial class GameplayScene : Conductor
{
    private Dictionary<string, Note> CachedNotes { get; set; } = new();
    private List<SectionNote> NoteData { get; set; } = new();
    private string[] noteTypeIgnore = Array.Empty<string>();

    //Song Vars
    private bool startingSong = true, endingSong, introSkipped, skipCountdown;
    private float scrollSpeed = 1f;

    //Health
    private float health = 1f, maxHealth = 2f;

    //Judgement
    private int score, misses, combo, accPressedNotes;
    private float accTotalPressed;

    //Beat Bump / Updating
    private int camBumpInterval = 4, iconBumpInterval = 1;
    private bool camBump = true, smoothZoom, camUpdate = true, iconBump = true, iconUpdate = true;

    //Elements
    private Stage stage;
    private Character2D spectator, opponent, player;

    //2D
    private Camera2D camera;
    private float camSpeed = 3f, camZoom = 1f;

    //3D
    private Vector3 camPos, camRot;
    private bool camSmoothing3D = true;

    //HUD
    private CanvasLayer HUD;
    private CanvasModulate HUDModulate;
    private Node2D ratingGroup, strumGroup;
    private NoteGroup noteGroup;
    private ScriptGroup scriptGroup;
    private ColorRect healthBarBG;
    private ProgressBar healthBar;
    private Sprite2D oppIcon, playerIcon;
    private Label scoreText;
    private StrumLine oppStrums, playerStrums;
    private readonly bool[] pressed = Array.Empty<bool>();
    private UIStyle uiStyle;

    //Countdown
    private Sprite2D countdownSprite;
    private AudioStreamPlayer countdownSound;
    private int countdownTicks;

    private const float IconDeltaMultiplier = 60f * 0.25f;
    private const float ZoomDeltaMultiplier = 60f * 0.05f;

    public override void _Ready()
    {
        base._Ready();

        AudioManager.Instance.StopAudio(AudioType.Music, "elseVI");
        GetTree().Paused = false;

        InitializeNodes();
        LoadGameSettings();
        LoadSong();
        InitializeStrumGroups();
        InitializeCountdown();
        LoadStage();
        StartSong();
    }

    private void InitializeNodes()
    {
        camera = GetNode<Camera2D>("Camera2D");
        HUD = GetNode<CanvasLayer>("HUD");
        HUDModulate = GetNode<CanvasModulate>("HUD/HUDModulate");
        ratingGroup = GetNode<Node2D>("HUD/RatingGroup");
        noteGroup = GetNode<NoteGroup>("HUD/NoteGroup");
        strumGroup = GetNode<Node2D>("HUD/StrumGroup");
        scriptGroup = GetNode<ScriptGroup>("Scripts");
        healthBarBG = GetNode<ColorRect>("HUD/HealthBarBG");
        healthBar = GetNode<ProgressBar>("HUD/HealthBarBG/HealthBar");
        scoreText = GetNode<Label>("HUD/HealthBarBG/ScoreText");
        playerIcon = GetNode<Sprite2D>("HUD/HealthBarBG/HealthBar/PlayerIcon");
        oppIcon = GetNode<Sprite2D>("HUD/HealthBarBG/HealthBar/OppIcon");
        countdownSprite = GetNode<Sprite2D>("HUD/CountdownSprite");
        countdownSound = GetNode<AudioStreamPlayer>("HUD/CountdownSprite/CountdownSound");
        inst = GetNode<AudioStreamPlayer>("Song/Inst");
        vocals = GetNode<AudioStreamPlayer>("Song/Voices");
    }

    private void LoadGameSettings()
    {
        scrollSpeed = Song.ScrollSpeed;
        var settingSpeed = Global.Settings.Gameplay.ScrollSpeed;
        switch (settingSpeed)
        {
            case > 0:
                scrollSpeed *= settingSpeed;
                break;
        }
    }

    private void InitializeCountdown()
    {
        // Initialize countdown sprite and sound
    }

    private void LoadStage()
    {
        string path3d = Song.Is3D ? "3D/" : "";
        string stagePath = $"res://assets/gameplay/stages/{path3d + Song.Stage}/";

        IEnumerable<string> stageDir = Global.FilesInDirectory(stagePath);
        stagePath = stageDir.Where(file => file.EndsWith(".tscn") || file.EndsWith(".remap")).Aggregate(stagePath, (current, file) => current + file.Replace(".remap", ""));

        stage = ResourceLoader.Exists(stagePath) 
            ? GD.Load<PackedScene>(stagePath).Instantiate<Stage>() 
            : GD.Load<PackedScene>("res://assets/gameplay/stages/stage/Stage.tscn".Replace(".remap", "")).Instantiate<Stage>();
        AddChild(stage);
        camZoom = stage.defaultCamZoom;
        camera.Zoom = new(camZoom, camZoom);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        UpdateSongProgress(delta);
        HandleHoldAnimation();
        UpdateHealthBar();
        HandleSmoothZoom(delta);

        if (Input.IsActionJustPressed("space_bar") && (introSkipped || startingSong)) SkipIntro();
    }

    private void UpdateHealthBar()
    {
        if (health <= 0) GameOver();
        healthBar.Value = health;
    }

    private void HandleSmoothZoom(double delta)
    {
        if (!smoothZoom) return;
        float cameraSpeed = Mathf.Clamp((float)delta * ZoomDeltaMultiplier * Conductor.Instance.rate, 0f, 1f);
        if (!Song.Is3D) camera.Zoom = new(Mathf.Lerp(camera.Zoom.X, camZoom, cameraSpeed), Mathf.Lerp(camera.Zoom.Y, camZoom, cameraSpeed));
        HUD.Scale = new(Mathf.Lerp(HUD.Scale.X, 1f, cameraSpeed), Mathf.Lerp(HUD.Scale.Y, 1f, cameraSpeed));
        HUD.Offset = new((HUD.Scale.X - 1f) * -(Global.windowSize.X * 0.5f), (HUD.Scale.Y - 1f) * -(Global.windowSize.Y * 0.5f));
    }

    protected override void OnBeatHit(int beat)
    {
        GD.Print("beat");
        CharacterDance(opponent);
        CharacterDance(player);
        CharacterDance(spectator);
    }

    protected override void OnStepHit(int step)
    {
        GD.Print("step");
    }

    protected override void OnSectionHit(int section)
    {
        GD.Print("section");
    }
}
