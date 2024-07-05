#region Imports
using FNFGodot.Backend.Autoload;
using FNFGodot.Backend.Classes;
using FNFGodot.Gameplay.Classes;
using FNFGodot.Gameplay.Resources;
using FNFGodot.Gameplay.Classes.Notes;
using System.Collections.Generic;
using FNFGodot.Gameplay.Classes.Strums;
using System.Linq;
using FNFGodot.Gameplay.Classes.Elements;
using FNFGodot.Gameplay.Classes.Events;
#endregion

namespace FNFGodot.Gameplay;
public partial class GameplayBase : Node
{
	/*	The base class that both Gameplay2D and Gameplay3D inherit from.
		Self-explanatory, works as the bridge that holds stuff for both
		classes that isnt affected by the dimensions.	*/

	public UIStyle Style;
	
	[NodePath("Song")] public Node songGroup;
	[NodePath("CharacterGroup")] public Node CharGroup;
	[NodePath("ScriptGroup")] public Node ScriptGroup;
	[NodePath("EventHandler")] public Node eventHandler;

	public bool StartedSong, StartedCountdown, EndedSong = false;
	float SongStartDelay;

	public float HealthLossOnMiss = 0.0675f;
	public float ScrollSpeed = 1;

	public HighScore highScore = new();
	
	private Dictionary<string,Note> NoteCache = new();
	public List<RawNote> RawNoteData = new();
	private Dictionary<string,AnimatedSprite2D> SplashCache = new();
	private float NoteSpawnOffset = 2;

	public bool CameraUpdating = true;
	public bool CameraBumping = true;
	public int CameraBumpInterval = 4;
	public float CameraBumpAmount = 0.01f;

	public bool HudBumping = true;
	public int HudBumpInterval = 4;
	public float HudBumpAmount = 0.006f;

	public bool IconBumping = true;
	public int IconBumpInterval = 2;
	public float IconBumpAmount = 0.2f;

	[NodePath("GameplayHUD")] public GameplayHUD Hud;

    public override void _Ready()
    {
		this.OnReady();

		Input.MouseMode = Input.MouseModeEnum.Hidden;

		GenerateSong();
		Conductor.StepHitEvent += StepHit;
		Conductor.BeatHitEvent += BeatHit;
		Conductor.SectionHitEvent += SectionHit;

		ScrollSpeed = ChartHandler.CurrentChart.ScrollSpeed;
		GD.Print($"Scroll Speed: {ScrollSpeed}");

		Style = GD.Load<UIStyle>($"res://assets/gameplay/hudStyles/{ChartHandler.CurrentChart.UiStyle}/style.tres");
		HighScore.SetJudgementArray(Style.JudgementSet);
		Hud.GenerateHealthbar(Style);

		Hud.JudgementTemplate.GetChild<AnimatedSprite2D>(0).Scale = new Vector2(Style.RatingScale,Style.RatingScale);
		Hud.JudgementTemplate.GetChild<AnimatedSprite2D>(0).Modulate = new Color(1,1,1,Style.RatingTransparency);

		// Generating strumlines
		float StrumY = -(Main.WindowSize.Y/2) + 100;
		if (Preferences.placeholderSettings["downscroll"]) StrumY = (Main.WindowSize.Y/2) - 100;
		Hud.GenerateStrum(ref Hud.PlayerStrums, ChartHandler.CurrentChart.KeyCount, new Vector2(380, StrumY), true);
		Hud.GenerateStrum(ref Hud.CpuStrums, ChartHandler.CurrentChart.KeyCount, new Vector2(-380, StrumY));

		SongStartDelay = Conductor.BeatDuration*4/1000;
		SceneTreeTimer timer = GetTree().CreateTimer(SongStartDelay);
		timer.Connect("timeout", Callable.From(StartSong));
	}

	private bool debugSwap = false;
	public override void _Process(double delta) {
		if (!EndedSong) {
			Conductor.UpdatePosition = true;
			if(Conductor.SongPosition > 0) SeekPosition((float)Conductor.SongPosition);
		}

		if(!StartedSong && StartedCountdown)
			Conductor.SongPosition = Mathf.Clamp(Conductor.SongPosition,-SongStartDelay,-1);

		if(Conductor.SongPosition/1000 >= Conductor.SongDuration && !EndedSong)
			EndSong();
		
		if(Input.IsActionJustPressed("menu_pause")) {
			AddChild(GD.Load<PackedScene>("res://source/gameplay/PauseSubmenu.tscn").Instantiate<PauseSubmenu>());
			GetTree().Paused = true;
		}
		if(OS.IsDebugBuild()) {
			if(Input.IsActionJustPressed("debug_reset"))
				LoadingHandler.ChangeScene("res://source/menus/debug/SongSelect.tscn");
			if(Input.IsActionJustPressed("debug_autoplay"))
				Hud.strumHandler.FocusedStrumline.AutoPlay = !Hud.strumHandler.FocusedStrumline.AutoPlay;
			if(Input.IsActionJustPressed("debug_swap")){
				debugSwap = !debugSwap;
				Hud.strumHandler.FocusStrumline(ref debugSwap ? ref Hud.CpuStrums : ref Hud.PlayerStrums);
			}
		}

		/*if(ChartHandler.CurrentChart.chartType == ChartTypeEnum.VSlice)
			CheckForVSliceEvents();*/
	}

	public override void _PhysicsProcess(double delta) {
		/*if(!StartedSong && StartedCountdown)
			Conductor.SongPosition = Mathf.Clamp(Conductor.SongPosition,-SongStartDelay,-1);*/
		
		GenerateNotes();
	}

	
	public void GenerateSong()
	{
		// Conductor/Song stuff
		if (ChartHandler.CurrentChart is null) ChartHandler.NewChart("test","normal");
		Conductor.UpdateBpm(ChartHandler.CurrentChart.Bpm);
		Conductor.SongPosition = -SongStartDelay;

		// i LOVE changing fnf's folder structure!
		string songFolder = $"res://assets/songs/{ChartHandler.CurrentChart.NameRaw.ToLower()}/song/";

		AudioStreamPlayer inst = songGroup.GetNode<AudioStreamPlayer>("Inst");
		AudioStreamPlayer mainVocals = songGroup.GetNode<AudioStreamPlayer>("Vocals");

		inst.Stream = Main.SearchAllAudioFormats($"{songFolder}Inst",false);
		// this looks weird but its checking if Vocals or Vocals-Player exists, if not, it will look for Voices instead
		string FinalVocalName = Main.SearchAllAudioFormats($"{songFolder}Vocals",false) is null && Main.SearchAllAudioFormats($"{songFolder}Vocals-Player",false) is null ? "Voices" : "Vocals";
		// then figures out if theres vocals file per character or just one
		bool FilePerCharacter = Main.SearchAllAudioFormats($"{songFolder+FinalVocalName}-Player",false) is not null;

		if(!FilePerCharacter)
			mainVocals.Stream = Main.SearchAllAudioFormats(songFolder+FinalVocalName,false);
		else {
			AudioStreamPlayer opponentVocals = mainVocals.Duplicate() as AudioStreamPlayer;
			opponentVocals.Name = "VocalsOpponent";
			opponentVocals.Bus = new StringName("VocalsOpponent");
			songGroup.AddChild(opponentVocals);

			mainVocals.Stream = Main.SearchAllAudioFormats($"{songFolder+FinalVocalName}-Player",false);
			opponentVocals.Stream = Main.SearchAllAudioFormats($"{songFolder+FinalVocalName}-Opponent",false);
		}

		if(inst.Stream is null) {
			GD.PushError("Could not find inst with any file type.\nReturning...");
			return;
		} else GD.Print("Inst found.");

		if(mainVocals.Stream is null)
			GD.PushWarning($"Vocal file not found on path:{songFolder}\nInst will continue playing.");
		else {
			if(FilePerCharacter) GD.Print("Vocals found for each character.");
			else GD.Print("Vocals found in a single file.");
		}

		Conductor.SongDuration = inst.Stream.GetLength();

		GetNotesFromChart();
		GetEventsFromChart();
	}

	public void GetNotesFromChart(double skipTime = -1)
	{
		foreach(RawNote rawNote in ChartHandler.CurrentChart.RawNotes)
		{
			if(rawNote.Time >= skipTime)
			{
				//RawNote NewNote = rawNote.Duplicate() as RawNote;
				//^ cant duplicate as that would turn it into Godot.Resource
				//and casting RawNote would literally make anything fucking null
				//this is not runtime so who cares anyways

				RawNote NewNote = new(){
					Time = rawNote.Time,
					Direction = rawNote.Direction,
					Type = rawNote.Type,
					Length = rawNote.Length,
					AltAnim = rawNote.AltAnim,
					PlayerSection = rawNote.PlayerSection
				};

				string NoteTypePath = $"res://assets/gameplay/noteTypes/{rawNote.Type.ToLower()}.tscn";
				if(!ResourceLoader.Exists(NoteTypePath)) {
					NoteTypePath = "res://assets/gameplay/noteTypes/default.tscn";
					if(!NoteCache.ContainsKey(rawNote.Type))
						GD.PrintErr($"Unable to find note type: {rawNote.Type}. Replacing with default notes.");
				}
				NoteCache[rawNote.Type] = GD.Load<PackedScene>(NoteTypePath).Instantiate<Note>();
				if(NoteCache[rawNote.Type].SplashTexture is not null) {
					SplashCache[rawNote.Type+"Splash"] = new(){
						SpriteFrames = NoteCache[rawNote.Type].SplashTexture
					};
				}
				if(NoteCache[rawNote.Type].SustainSplashTexture is not null) {
					SplashCache[rawNote.Type+"Sustain"] = new(){
						SpriteFrames = NoteCache[rawNote.Type].SustainSplashTexture
					};
				}
					
				RawNoteData.Add(NewNote);
			}
		}
		RawNoteData.Sort((a, b) => a.Time.CompareTo(b.Time));
		/*for(int i = 0; i < RawNoteData.Count; i++) {
			if(i < RawNoteData.Count) {
				if(RawNoteData[i].Time == RawNoteData[i+1].Time && RawNoteData[i].Direction == RawNoteData[i+1].Direction)
					RawNoteData.Remove(RawNoteData[i]);
			}
		}*/
		GD.Print("Succesfully got notes from chart.");
	}

	public void GetEventsFromChart(double skipTime = -1)
	{
		foreach(RawEvent rawEvent in ChartHandler.CurrentChart.RawEvents)
		{
			if(rawEvent.Time >= skipTime)
			{
				string FinalEventPath = $"res://source/gameplay/resources/events/{rawEvent.EventName}.tscn";
				if(ResourceLoader.Exists(FinalEventPath)) {
					SongEvent newEvent = GD.Load<PackedScene>(FinalEventPath).Instantiate<SongEvent>();
					newEvent.Values = rawEvent.Values;
					newEvent.Time = rawEvent.Time;
					eventHandler.AddChild(newEvent);
				}
			}
		}
	}

	public void GenerateNotes()
	{
		foreach (RawNote note in RawNoteData.ToList())
		{
			if(note.Time > Conductor.SongPosition + (NoteSpawnOffset*1000/ScrollSpeed)) break;
			if(note.Direction < 0)
			{
				RawNoteData.Remove(note);
			}
			string FinalNoteType = NoteCache.ContainsKey(note.Type) ? note.Type : "default";

			Note HandledNote = NoteCache[note.Type].Duplicate() as Note;
			HandledNote.PlayerNote = note.PlayerSection;
			HandledNote.CurrentStrumline = HandledNote.PlayerNote ? Hud.PlayerStrums : Hud.CpuStrums;
			
			HandledNote.Direction = note.Direction % ChartHandler.CurrentChart.KeyCount;
			HandledNote.Position = new Vector2(HandledNote.CurrentStrumline.GetChild<Strum>(HandledNote.Direction).GlobalPosition.X, -9999);
			HandledNote.Scale = new Vector2(Style.NoteScale,Style.NoteScale);

			HandledNote.Time = note.Time;
			HandledNote.SustainLength = note.Length*0.85f;

			HandledNote.NoteType = FinalNoteType;
			HandledNote.RawType = note.Type;
			HandledNote.uiStyle = Style;

			Strum strum = HandledNote.CurrentStrumline.GetChild<Strum>(HandledNote.Direction);

			if(SplashCache[HandledNote.RawType+"Splash"] is not null && !strum.HasNode($"{HandledNote.RawType}Splash")) {
				AnimatedSprite2D NewSplash = SplashCache[HandledNote.RawType+"Splash"].Duplicate() as AnimatedSprite2D;
				NewSplash.Name = $"{HandledNote.RawType}Splash";
				NewSplash.Visible = false;
				NewSplash.ZIndex = 1;
				NewSplash.Connect("animation_finished", Callable.From(() => NewSplash.Visible = false));
				strum.AddChild(NewSplash);
			}
			if(SplashCache[HandledNote.RawType+"Sustain"] is not null && !strum.HasNode($"{HandledNote.RawType}Sustain")) {
				AnimatedSprite2D SusSplash = SplashCache[HandledNote.RawType+"Sustain"].Duplicate() as AnimatedSprite2D;
				SusSplash.Name = $"{HandledNote.RawType}Sustain";
				SusSplash.Visible = false;
				SusSplash.ZIndex = 1;
				SusSplash.Connect("animation_finished", Callable.From(() => {
					if(SusSplash.Animation.ToString().EndsWith("end"))
						SusSplash.Visible = false;
					else
						SusSplash.Play($"{strum.Name} hold");
						SusSplash.Offset = HandledNote.SustainSplashOffset["hold"];
					}));
				strum.AddChild(SusSplash);
			}
			
			Hud.noteHandler.AddChild(HandledNote);
			RawNoteData.Remove(note);
		}
	}

	public float LatencyThreshold = 150;
	public void SeekPosition(float PosToSeek)
	{
		// Resyncs the damn things
		double FixedPos = AudioServer.GetTimeSinceLastMix() + songGroup.GetChild<AudioStreamPlayer>(0).GetPlaybackPosition() * 1000;
        if (Mathf.Abs(FixedPos-PosToSeek) > LatencyThreshold && songGroup.GetChild<AudioStreamPlayer>(0).Playing) {
			GD.Print("Desynchronization found, Resyncing song...");
			foreach(AudioStreamPlayer player in songGroup.GetChildren()) {
				player.Seek(PosToSeek/1000);
			}
		}
	}

	public void StartSong()
	{
		StartedSong = true;
		Conductor.SongPosition = 0;
		foreach (AudioStreamPlayer player in songGroup.GetChildren())
			player.Play((float)Conductor.SongPosition);
		
		GD.Print("Started song.");
	}

	public void EndSong()
	{
		EndedSong = true;
		foreach (AudioStreamPlayer player in songGroup.GetChildren())
			player.Stop();
		
		LoadingHandler.ChangeScene("res://source/menus/debug/SongSelect.tscn");

		GD.Print("Ended song.");
	}

	public void StepHit()
	{
		//GD.Print($"step hit at step: {Conductor.CurStep}");
	}

	public void BeatHit()
	{
		//GD.Print($"beat hit at beat: {Conductor.CurBeat}");

		if(HudBumping && Conductor.CurBeat % HudBumpInterval == 0)
			Hud.Scale += new Vector2(HudBumpAmount,HudBumpAmount);

		if(IconBumping && Conductor.CurBeat % IconBumpInterval == 0) {
			foreach(Icon icon in Hud.HealthBar.IconGroup.GetChildren())
				icon.Scale = new Vector2(icon.DefaultScale.X+IconBumpAmount,icon.DefaultScale.Y+IconBumpAmount);
		}
	}

	public void SectionHit()
	{
		//GD.Print($"section hit at sec: {Conductor.CurSection}");
	}

	public override void _ExitTree() {
		base._ExitTree();

		Conductor.UpdatePosition = false;

		Conductor.StepHitEvent -= StepHit;
		Conductor.BeatHitEvent -= BeatHit;
		Conductor.SectionHitEvent -= SectionHit;

		Input.MouseMode = Input.MouseModeEnum.Visible;
		ChartHandler.CurrentChart.Dispose();
	}
}
