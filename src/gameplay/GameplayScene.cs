using System.Collections.Generic;
using System.Linq;
using Rubicon.Backend.Autoload.Debug.ScreenNotifier;
using Rubicon.Backend.Autoload.Managers.AudioManager.Enums;
using Rubicon.Gameplay.Elements.Classes.Song;
using Rubicon.Gameplay.Elements.Resources;
using Rubicon.Gameplay.Elements.Scripts;
using Rubicon.Gameplay.Elements.StrumLines;
using Rubicon.Common.Stages.Base;
using AudioManager = Rubicon.Backend.Autoload.Managers.AudioManager.AudioManager;
using Rubicon.Backend.Autoload.Managers.TransitionManager;
using Rubicon.Backend.Scripts;

namespace Rubicon.Gameplay;

public partial class GameplayScene : Conductor
{
	private Dictionary<string, Note> CachedNotes { get; set; } = new();
	private List<SectionNote> NoteData { get; set; } = new();
	private string[] noteTypeIgnore = Array.Empty<string>();
	private Chart Song;
	
	//music
	private AudioStreamPlayer inst;
	private AudioStreamPlayer vocals;
	
	//Song Vars
	private bool startingSong = false, endingSong, introSkipped, skipCountdown;
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

    //maybe a new camera for only the strums/notes?

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

		AudioManager.Instance.music.Stop();
		//GetTree().Paused = false;

		scrollSpeed = /*Song.ScrollSpeed*/ 1.0f;
		var settingSpeed = Main.GameSettings.Gameplay.ScrollSpeed;
		switch (settingSpeed)
		{
			case > 0:
				scrollSpeed *= settingSpeed;
				break;
		}

		if (Main.Song == null)
		{
			Main.Song = Chart.LoadChart("+eraby+e-connec+10n", "hard");
			Song = Main.Song;
		}

		Instance.MapBPMChanges(Song);
		Instance.bpm = Song.Bpm;
		Instance.position = Instance.stepCrochet * 5;

		//string songPath = /*$"res://assets/songs/{Song.SongName.ToLower()}/"*/Paths.Inst(Song.SongName);
		//GD.PrintErr("the path of the music is: "+songPath);

		foreach (var f in Main.AudioFormats)
		{   //also, maybe this step will move into paths
			if (ResourceLoader.Exists(/*$"{songPath}inst.{f}")*/Paths.Inst(Song.SongName)+"."+f))
			{
				GD.Print("final inst path: "+Paths.Inst(Song.SongName)+"."+f);
				inst.Stream = GD.Load<AudioStream>(/*$"{songPath}inst.{f}"*/Paths.Inst(Song.SongName)+"."+f);
				if (/*vocals.Stream == null && */ResourceLoader.Exists(/*$"{songPath}voices.{f}"*/Paths.Voices(Song.SongName)+"."+f)) 
					vocals.Stream = GD.Load<AudioStream>(/*$"{songPath}voices.{f}"*/Paths.Voices(Song.SongName)+"."+f);
			}
		}

		/*if (inst.Stream == null)
		{
			Main.Instance.Alert($"Inst not found on path: '{Paths.Inst(Song.SongName)}inst' with every format, ending song", true, NotificationType.Error);
			endingSong = true;
			EndSong();
			return;
		}*/

		//foreach (AudioStreamPlayer track in tracks) track.PitchScale = Instance.rate;
		
		InitializeStrumGroups();

		//
		//initialize countdown here
		//
		
		//LoadStage();
		StartSong();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (startingSong)
		{
			/*if (endingSong || tracks == null || tracks.Count == 0 || tracks[0].Stream == null)
			{
				if (endingSong) GD.Print("Song has ended. Returning");
				else {
					//GD.PrintErr("There's nothing in tracks. Returning");
					Main.Instance.Alert("There's nothing in tracks. Returning", true, NotificationType.Error);
					TransitionManager.Instance.ChangeScene("res://src/scenes/MainMenu.tscn");
				return;
				}
			}*/
			if (inst == null) {GD.PrintErr("NO MUSIC");return;}
			Instance.position += /*(float)delta * 1000f * Instance.rate*/ inst.GetPlaybackPosition();

			/*if (Instance.position >= tracks[0].Stream.GetLength())
			{
				EndSong();
				return;
			}*/

			if (Instance.position >= 0f && startingSong) StartSong();
		}
		
		HandleHoldAnimation();
		HandleSmoothZoom(delta);

		if (health <= 0) GameOver();
		//healthBar.Value = health;
		
		//if (Input.IsActionJustPressed("space_bar") && (introSkipped || startingSong)) SkipIntro();
	}

	public void StartSong()
	{
		Instance.position = 0f;
		inst.Play();
		if (vocals != null)
			vocals.Play();
		//foreach (AudioStreamPlayer track in tracks) track.Play(/*(float)Instance.position / 1000*/);
		startingSong = true;
	}

	public void EndSong()
	{
		// Handle end of the song
	}

	public void SyncSong()
	{
		//foreach (var track in tracks.Where(track => track.Stream != null)) track.Play((float)Instance.position / 1000f);
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
        InitializeStrumLine(ref oppStrums, (Main.WindowSize.X * 0.5f) - 320f);
        InitializeStrumLine(ref playerStrums, (Main.WindowSize.X * 0.5f) + 320f);
    }

	private void InitializeStrumLine(ref StrumLine strumLine, float positionX)
	{
		/*Song.KeyCount*/
		strumLine = GD.Load<PackedScene>($"res://src/gameplay/elements/strumlines/4K.tscn").Instantiate<StrumLine>();
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
				IEnumerable<string> noteTypeDir = Paths.FilesInDirectory(noteTypePath);
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

	private void LoadStage()
	{
		string path3d = Song.Is3D ? "3D/" : "";
		string stagePath = $"res://assets/gameplay/stages/{path3d + Song.Stage}/";

		IEnumerable<string> stageDir = Paths.FilesInDirectory(stagePath);
		stagePath = stageDir.Where(file => file.EndsWith(".tscn") || file.EndsWith(".remap")).Aggregate(stagePath, (current, file) => current + file.Replace(".remap", ""));

		stage = ResourceLoader.Exists(stagePath) 
			? GD.Load<PackedScene>(stagePath).Instantiate<Stage>() 
			: GD.Load<PackedScene>("res://assets/gameplay/stages/stage/Stage.tscn".Replace(".remap", "")).Instantiate<Stage>();
		AddChild(stage);
		camZoom = stage.defaultCamZoom;
		camera.Zoom = new(camZoom, camZoom);
	}

	public void GenerateCharacter(ref Character2D character, string characterType, Vector2 characterPosition)
	{
		if (character == null) throw new ArgumentNullException(nameof(character));
		string path3d = Song.Is3D ? "3D/" : "";
		string charPath = $"res://assets/gameplay/characters/{path3d + characterType}.tscn";

		character = ResourceLoader.Load<PackedScene>(charPath)?.Instantiate<Character2D>() ?? GD.Load<PackedScene>("res://assets/gameplay/characters/bf.tscn").Instantiate<Character2D>();

		if (character != null)
		{
			character.Position = characterPosition;
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
		/*if(player != null)
			if (pressed.Contains(true) || !player.lastAnim.StartsWith("sing") || player.holdTimer < Instance.stepCrochet * player.singDuration * 0.0011)
			return;

        player.holdTimer = 0f;
        player.dance();*/
    }
    
    private void HandleSmoothZoom(double delta)
    {
        if (!smoothZoom) return;
        float cameraSpeed = Mathf.Clamp((float)delta * ZoomDeltaMultiplier * Instance.rate, 0f, 1f);
        if (!Song.Is3D) camera.Zoom = new(Mathf.Lerp(camera.Zoom.X, camZoom, cameraSpeed), Mathf.Lerp(camera.Zoom.Y, camZoom, cameraSpeed));
        HUD.Scale = new(Mathf.Lerp(HUD.Scale.X, 1f, cameraSpeed), Mathf.Lerp(HUD.Scale.Y, 1f, cameraSpeed));
        HUD.Offset = new((HUD.Scale.X - 1f) * -(Main.WindowSize.X * 0.5f), (HUD.Scale.Y - 1f) * -(Main.WindowSize.Y * 0.5f));
    }

	protected override void OnBeatHit(int beat)
	{
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
