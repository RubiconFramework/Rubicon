using Rubicon.Backend.Autoload;
using Rubicon.Backend.Classes;
using Rubicon.Gameplay.Classes.Elements;

namespace Rubicon.Gameplay;
public partial class Gameplay2D : GameplayBase
{
	/*	This class handles all 2D specific / 2D sensitive stuff.
		I found it more effective to just separate them in two classes rather
		than actually filling one class with conditions for 2D and 3D handling.
		This also helps to make casting not a headache.	  */

	[NodePath("Camera")] public SmoothCamera2D Camera;

	public Stage2D Stage;
	public Character2D Player, Opponent, Watcher;
	public override void _Ready()
	{
		this.OnReady();
		base._Ready();

		Conductor.OnBeatHit += BeatHit;
		Conductor.OnSectionHit += SectionHit;

		GenerateStage(ref Stage, ChartHandler.CurrentChart.Stage);

		GenerateCharacter(ref Watcher, ChartHandler.CurrentChart.Watcher, Stage.GetCharPosition("Watcher"));
		GenerateCharacter(ref Opponent, ChartHandler.CurrentChart.Opponent, Stage.GetCharPosition("Opponent"));
		GenerateCharacter(ref Player, ChartHandler.CurrentChart.Player, Stage.GetCharPosition("Player"), true);

		GenerateIcon(ref Hud.HealthBar.PlayerIcon, Player.CharacterIcon, Player.IconOffset, true);
		GenerateIcon(ref Hud.HealthBar.OpponentIcon, Opponent.CharacterIcon, Opponent.IconOffset);

		Hud.PlayerStrums.FocusedCharacters.Add(Player);
		Hud.CpuStrums.FocusedCharacters.Add(Opponent);

		/*string charToFocus = ChartHandler.CurrentChart.Sections[0].IsPlayer ? "Player" : "Opponent";
		FocusCharacterCamera(charToFocus);*/

		StartedCountdown = true;
	}

	public void GenerateStage(ref Stage2D stageToGen, string FileName)
	{
		string path = $"res://assets/gameplay/stages/{FileName}.tscn";
		string defaultPath = "res://assets/gameplay/stages/stage.tscn";
		stageToGen = ResourceLoader.Exists(path) ? GD.Load<PackedScene>(path).Instantiate<Stage2D>() : GD.Load<PackedScene>(defaultPath).Instantiate<Stage2D>();
		Camera.StaticZoom = stageToGen.CameraZoom;
		AddChild(stageToGen);
		if (!ResourceLoader.Exists(path)) GD.Print($"Stage '{FileName}' not found. Generated placeholder instead.");
		else GD.Print($"Stage '{FileName}' Generated");
	}

	public void GenerateCharacter(ref Character2D charToGen, string FileName, Vector2 Position, bool isPlayer = false)
	{
		string path = $"res://assets/gameplay/characters/{FileName}.tscn";
		string defaultPath = "res://assets/gameplay/characters/bf.tscn";
		charToGen = ResourceLoader.Exists(path) ? GD.Load<PackedScene>(path).Instantiate<Character2D>() : GD.Load<PackedScene>(defaultPath).Instantiate<Character2D>();
		charToGen.Position = Position;
		charToGen.IsPlayer = isPlayer;
		CharGroup.AddChild(charToGen);
		if (!ResourceLoader.Exists(path)) GD.Print($"Character '{FileName}' not found. Generated placeholder instead.");
		else GD.Print($"Character '{FileName}' Generated");
	}

	public void GenerateIcon(ref Icon icon, SpriteFrames spriteFrames, Vector2 offset, bool isPlayer = false)
	{
		icon.Offset = offset;
		icon.SpriteFrames = spriteFrames;
		icon.Play("idle");
		icon.IsPlayer = isPlayer;
	}

	public void CharacterDance(ref Character2D Character, bool force = false)
	{
		if(Character is not null && Character.ShouldDance) {
			bool shouldForce = !force ? Character.DanceList[0].StartsWith("dance") : true;
			Character.Dance(shouldForce);
			//GD.Print("someone is dancing");
		}
	}

	public void FocusCharacterCamera(string character = "Player")
	{
		Camera.Position = Stage.GetCharCamera(character);
		//GD.Print($"Camera now focusing player: {character}.");
	}

	public int BeatHit(int beat)
	{

		CharacterDance(ref Player);
		CharacterDance(ref Watcher);
		CharacterDance(ref Opponent);

		if(CameraBumping && Conductor.CurBeat % CameraBumpInterval == 0)
			Camera.Zoom += new Vector2(CameraBumpAmount,CameraBumpAmount);

		//GD.Print("should dance");
		return beat;
	}

	public int SectionHit(int cursection)
	{
		if (ChartHandler.CurrentChart.chartType == ChartTypeEnum.Default && CameraUpdating && ChartHandler.CurrentChart.Sections.Count-1 >= Conductor.CurSection && !EndedSong)
		{
			bool section = ChartHandler.CurrentChart.Sections[Conductor.CurSection] is null ? ChartHandler.CurrentChart.Sections[Conductor.CurSection-1].IsPlayer : ChartHandler.CurrentChart.Sections[Conductor.CurSection].IsPlayer;
			string charToFocus = section ? "Player" : "Opponent";
			FocusCharacterCamera(charToFocus);
		}

		return cursection;
	}

	public override void _ExitTree() {
		base._ExitTree();

		Conductor.OnBeatHit -= BeatHit;
		Conductor.OnSectionHit -= SectionHit;
	}
}
