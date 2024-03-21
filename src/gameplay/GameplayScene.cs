using System.Collections.Generic;
using System.Linq;
using BaseRubicon.Backend.Autoload.Debug.ScreenNotifier;
using BaseRubicon.Backend.Autoload.Managers.AudioManager;
using BaseRubicon.Gameplay.Elements.Classes.Song;
using BaseRubicon.Gameplay.Elements.Resources;
using BaseRubicon.Gameplay.Elements.Scripts;
using BaseRubicon.Gameplay.Elements.StrumLines;
using AudioManager = BaseRubicon.Backend.Autoload.Managers.AudioManager.AudioManager;
using Global = BaseRubicon.Backend.Autoload.Global;
using Stage = BaseRubicon.Backend.Scripts.Stage;

namespace BaseRubicon.Gameplay;

public partial class GameplayScene : Conductor
{
    private Dictionary<string, Note> CachedNotes { get; set; } = new();
    private List<SectionNote> NoteData { get; set; } = new();
    private string[] noteTypeIgnore = Array.Empty<string>();
    private Chart Song;
    private readonly List<AudioStreamPlayer> tracks = new();
    [NodePath("Song/Inst")] private AudioStreamPlayer inst;
    [NodePath("Song/Voices")] private AudioStreamPlayer vocals;
    
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
    [NodePath("Camera2D")] private Camera2D camera;
    private float camSpeed = 3f, camZoom = 1f;

    //3D
    private Vector3 camPos, camRot;
    private bool camSmoothing3D = true;

    //HUD
    [NodePath("HUD")] private CanvasLayer HUD;
    [NodePath("HUD/HUDModulate")] private CanvasModulate HUDModulate;
    [NodePath("HUD/RatingGroup")] private Node2D ratingGroup;
    [NodePath("HUD/NoteGroup")] private NoteGroup noteGroup;
    [NodePath("HUD/StrumGroup")] private Node2D strumGroup;
    [NodePath("Scripts")] private ScriptGroup scriptGroup;
    [NodePath("HUD/HealthBarBG")] private ColorRect healthBarBG;
    [NodePath("HUD/HealthBarBG/HealthBar")] private ProgressBar healthBar;
    [NodePath("HUD/HealthBarBG/ScoreText")] private Label scoreText;
    [NodePath("HUD/HealthBarBG/HealthBar/PlayerIcon")] private Sprite2D playerIcon;
    [NodePath("HUD/HealthBarBG/HealthBar/OppIcon")] private Sprite2D oppIcon;

    private StrumLine oppStrums, playerStrums;
    private readonly bool[] pressed = Array.Empty<bool>();
    private UIStyle uiStyle;

    
    //Countdown
    [NodePath("HUD/CountdownSprite")] private Sprite2D countdownSprite;
    [NodePath("HUD/CountdownSprite/CountdownSound")] private AudioStreamPlayer countdownSound;
    private int countdownTicks;

    private const float IconDeltaMultiplier = 60f * 0.25f;
    private const float ZoomDeltaMultiplier = 60f * 0.05f;

    public override void _Ready()
    {
        base._Ready();

        AudioManager.Instance.StopAudio(AudioType.Music, "elseVI");
        GetTree().Paused = false;

        scrollSpeed = Song.ScrollSpeed;
        var settingSpeed = Global.Settings.Gameplay.ScrollSpeed;
        switch (settingSpeed)
        {
            case > 0:
                scrollSpeed *= settingSpeed;
                break;
        }
        

        if (Global.Song == null)
        {
            Global.Song = Chart.LoadChart("imscared", "normal");
            Song = Global.Song;
        }

        Instance.MapBPMChanges(Song);
        Instance.bpm = Song.Bpm;
        Instance.position = Instance.stepCrochet * 5;

        string songPath = $"res://assets/songs/{Song.SongName.ToLower()}/song/";

        foreach (var f in Global.audioFormats)
        {
            if (ResourceLoader.Exists($"{songPath}inst.{f}"))
            {
                inst.Stream = GD.Load<AudioStream>($"{songPath}inst.{f}");
                if (vocals.Stream == null && ResourceLoader.Exists($"{songPath}voices.{f}"))
                {
                    vocals.Stream = GD.Load<AudioStream>($"{songPath}voices.{f}");
                    tracks.Add(vocals);
                }
            }
        }

        if (inst.Stream == null)
        {
            ScreenNotifier.Instance.Notify($"Inst not found on path: '{songPath}inst' with every format, ending song", true, NotificationType.Error);
            endingSong = true;
            EndSong();
            return;
        }

        foreach (AudioStreamPlayer track in tracks) track.PitchScale = Instance.rate;
        
        InitializeStrumGroups();
        InitializeCountdown();
        LoadStage();
        StartSong();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        UpdateSongProgress(delta);
        HandleHoldAnimation();
        HandleSmoothZoom(delta);

        if (health <= 0) GameOver();
        healthBar.Value = health;
        
        if (Input.IsActionJustPressed("space_bar") && (introSkipped || startingSong)) SkipIntro();
    }

    public void StartSong()
    {
        Instance.position = 0f;
        foreach (AudioStreamPlayer track in tracks) track.Play((float)Instance.position / 1000);
        startingSong = false;
    }

    public void EndSong()
    {
        // Handle end of the song
    }

    private void UpdateSongProgress(double delta)
    {
        if (endingSong || tracks == null || tracks.Count == 0 || tracks[0].Stream == null)
        {
            if (endingSong) GD.Print("Song has ended. Returning");
            else GD.PrintErr("There's nothing in tracks. Returning");
            return;
        }

        Instance.position += (float)delta * 1000f * Instance.rate;

        if (Instance.position >= tracks[0].Stream.GetLength())
        {
            EndSong();
            return;
        }

        if (Instance.position >= 0f && startingSong) StartSong();
    }

    public void SyncSong()
    {
        foreach (var track in tracks.Where(track => track.Stream != null)) track.Play((float)Instance.position / 1000f);
    }

    public void GameOver()
    {
        // Handle game over scenario
    }

    public void SkipIntro()
    {
        // Handle skipping the intro
    }
    
    private void InitializeStrumGroups()
    {
        InitializeStrumLine(ref oppStrums, (Global.windowSize.X * 0.5f) - 320f);
        InitializeStrumLine(ref playerStrums, (Global.windowSize.X * 0.5f) + 320f);
    }

    private void InitializeStrumLine(ref StrumLine strumLine, float positionX)
    {
        strumLine = GD.Load<PackedScene>($"res://src/gameplay/elements/strumlines/{Song.KeyCount}K.tscn").Instantiate<StrumLine>();
        strumLine.uiStyle = uiStyle;
        strumGroup.AddChild(strumLine);
        strumLine.Position = new(positionX, 100);
        if (strumLine == playerStrums) strumLine.readsInput = true;
    }

    public void GenerateNotes(float skipTime = -1f)
    {
        foreach (Section section in Song.Sections)
        {
            foreach (SectionNote note in section.SectionNotes)
            {
                if (note.Time > skipTime) continue;

                SectionNote newNote = (SectionNote)note.Duplicate();

                string noteTypePath = $"res://assets/gameplay/notes/{note.Type.ToLower()}/";
                IEnumerable<string> noteTypeDir = Global.FilesInDirectory(noteTypePath);
                foreach (string file in noteTypeDir)
                {
                    if (!CachedNotes.ContainsKey(note.Type) && (file.EndsWith(".tscn") || file.EndsWith(".remap"))) 
                        CachedNotes[note.Type] = GD.Load<Note>(noteTypePath + file.Replace(".remap", ""));
                }

                NoteData.Add(newNote);
            }
        }
        NoteData.Sort((a, b) => a.Time.CompareTo(b.Time));

        foreach (SectionNote note in NoteData) GD.Print(note.Time);
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

    public void GenerateCharacter(ref Character2D character, string characterType, Vector2 position)
    {
        if (character == null) throw new ArgumentNullException(nameof(character));
        string path3d = Song.Is3D ? "3D/" : "";
        string charPath = $"res://assets/gameplay/characters/{path3d + characterType}.tscn";

        character = ResourceLoader.Load<PackedScene>(charPath)?.Instantiate<Character2D>() ?? GD.Load<PackedScene>("res://assets/gameplay/characters/bf.tscn").Instantiate<Character2D>();

        if (character != null)
        {
            character.Position = position;
            AddChild(character);
        }
    }

    public void GenPlayer()
    {
        GenerateCharacter(ref player, Song.Player, stage.characterPositions["Player"]);
        player.isPlayer = true;
    }

    public void GenOpponent()
    {
        GenerateCharacter(ref opponent, Song.Opponent, stage.characterPositions["Opponent"]);

        if (Song.Opponent == Song.Spectator)
        {
            opponent.Position = stage.characterPositions["Spectator"];
            spectator.Visible = false;
        }
    }

    public void GenSpectator() => GenerateCharacter(ref spectator, Song.Spectator, stage.characterPositions["Spectator"]);

    public static void CharacterDance(Character2D charToDance, bool force = false)
    {
        if (charToDance?.danceOnBeat == true && (force || charToDance.lastAnim.StartsWith("sing"))) charToDance.dance();
    }

    private void HandleHoldAnimation()
    {
        if (pressed.Contains(true) || !player.lastAnim.StartsWith("sing") || player.holdTimer < Instance.stepCrochet * player.singDuration * 0.0011)
            return;

        player.holdTimer = 0f;
        player.dance();
    }
    
    private void HandleSmoothZoom(double delta)
    {
        if (!smoothZoom) return;
        float cameraSpeed = Mathf.Clamp((float)delta * ZoomDeltaMultiplier * Instance.rate, 0f, 1f);
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
