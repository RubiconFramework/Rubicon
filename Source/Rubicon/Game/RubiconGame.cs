using Rubicon.Core;
using Rubicon.Core.Chart;
using Rubicon.Core.Meta;
using Rubicon.Rulesets;

namespace Rubicon.Game;

/// <summary>
/// The main node that brings characters, stages, and ruleset gameplay together. Serves as "PlayState" in other Funkin' engines.
/// </summary>
public partial class RubiconGame : Node
{
	public static RubiconGame Instance { get; private set; }
	
	#if TOOLS
	[Export] public string EditorSongName = "Test";

	[Export] public string EditorChartName = "Mania-Normal";
	#endif

	[ExportGroup("Status"), Export] public bool Paused = false;
	
	[Export] public RuleSet RuleSet;
	
	[Export] public PlayField PlayField;

	[ExportGroup("Audio"), Export] public AudioStreamPlayer Instrumental;
	
	[Export] public AudioStreamPlayer Vocals;
	
	public override void _Ready()
	{
		if (Instance != null)
		{
			QueueFree();
			return;
		}
		Instance = this;

		string songName = ""; // Should probably change this tbh
		string chartName = "";
		#if TOOLS
		if (RubiconEngine.StartingSceneType == typeof(RubiconGame))
		{
			songName = EditorSongName;
			chartName = EditorChartName;
		}
		#endif
		
		SongMeta meta = GD.Load<SongMeta>($"res://Songs/{songName}/Data/Meta.tres");
		RubiChart chart = GD.Load<RubiChart>($"res://Songs/{songName}/Data/{chartName}.tres");
		chart.ConvertData().Format();
		
		//GetTree().Root.FsrSharpness

		string instPath = $"res://Songs/{songName}/Inst.ogg";
		string vocalsPath = $"res://Songs/{songName}/Vocals.ogg";
		if (ResourceLoader.Exists(instPath))
			Instrumental.Stream = GD.Load<AudioStream>(instPath);
		else
			GD.PrintErr($"Audio file at path \"{instPath}\" was not found!");

		if (ResourceLoader.Exists(vocalsPath))
			Vocals.Stream = GD.Load<AudioStream>(vocalsPath);

		Conductor.Reset();
		Conductor.ChartOffset = chart.Offset;
		Conductor.BpmList = chart.BpmInfo;
		
		// Set up rule set, and check paths too
		string ruleSetName = chart.DefaultRuleset;
		RuleSet = LoadRuleSet(ruleSetName);
		if (RuleSet is null)
		{
			string fallbackName = ProjectSettings.GetSetting("rubicon/rulesets/default_ruleset").AsString();
			GD.PrintErr($"Rule set \"{ruleSetName}\" was not able to be loaded. Falling back to \"{fallbackName}\".");
			RuleSet = LoadRuleSet(fallbackName);
		}

		if (RuleSet is null) // You fucked up bro
			throw new Exception("RuleSet is still null. Please check your Project Settings at \"rubicon/rulesets\"");
		
		// Set up play field
		PlayField = LoadPlayField(RuleSet);
		PlayField.Setup(meta, chart);
		AddChild(PlayField);
		
		// TODO: Countdown
		Conductor.Play();
		
		if (Instrumental.Stream != null)
			Instrumental.Play();
		
		if (Vocals.Stream != null)
			Vocals.Play();
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (@event.IsActionPressed("GAME_PAUSE"))
		{
			if (!Paused)
				Pause();
			else
				Resume();
		}
	}

	/// <summary>
	/// Pauses the game.
	/// </summary>
	public void Pause()
	{
		Conductor.Pause();	
		PlayField.ProcessMode = ProcessModeEnum.Disabled;
		Paused = true;
		
		if (Instrumental.Stream != null)
			Instrumental.Stop();
		
		if (Vocals.Stream != null)
			Vocals.Stop();
	}

	/// <summary>
	/// Resumes the game.
	/// </summary>
	public void Resume()
	{
		Conductor.Resume();	
		PlayField.ProcessMode = ProcessModeEnum.Inherit;
		Paused = false;

		float time = (float)Conductor.RawTime;
		if (Instrumental.Stream != null)
			Instrumental.Play(time);

		if (Vocals.Stream != null)
			Vocals.Play(time);
	}
	
	private RuleSet LoadRuleSet(string ruleSetName)
	{
		string ruleSetResourcePath = $"res://Resources/Rulesets/{ruleSetName}.tres";
		if (!ResourceLoader.Exists(ruleSetResourcePath))
		{
			GD.PrintErr($"No resource exists at path \"{ruleSetResourcePath}\".");
			return null;
		}
		
		Resource ruleSetResource = GD.Load<Resource>(ruleSetResourcePath);
		if (ruleSetResource is not RuleSet ruleSet)
		{
			GD.PrintErr($"Resource at path \"{ruleSetResourcePath}\" found, but does not inherit RuleSet.cs");
			return null;
		}

		return ruleSet;
	}

	private PlayField LoadPlayField(RuleSet ruleSet)
	{
		/*
		Script script = ruleSet.PlayFieldScript;
		if (script is not CSharpScript cSharpScript)
		{
			GD.PrintErr($"Rulesets do not support other languages other than C# at the moment. (path: \"{ruleSet.ResourcePath}\")");
			return null;
		}*/

		GodotObject ruleSetObject = ruleSet.PlayFieldScript.New().AsGodotObject();
		if (ruleSetObject is not PlayField playField)
		{
			GD.PrintErr($"Ruleset at path \"{ruleSet.ResourcePath}\" does not contain a valid PlayField script.");
			return null;
		}

		return playField;
	}
}
