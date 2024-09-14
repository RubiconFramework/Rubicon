using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rubicon.Core;
using Rubicon.Core.Chart;
using Rubicon.Core.Meta;
using Rubicon.Rulesets;
using Rubicon.Rulesets.Mania;

namespace Rubicon.Game;

public partial class RubiconGame : Node
{
	public static RubiconGame Instance { get; private set; }

	[Export] public bool Paused = false;
	
	[Export] public RuleSet RuleSet;
	
	[Export] public PlayField PlayField;

	[ExportGroup("Audio"), Export] public AudioStreamPlayer Instrumental;
	
	[Export] public AudioStreamPlayer Vocals;

	private Label _distanceDebugLabel;
	
	public override void _Ready()
	{
		if (Instance != null)
		{
			QueueFree();
			return;
		}
		Instance = this;
		
		// Shitty
		SongMeta meta = GD.Load<SongMeta>("res://songs/test/data/meta.tres");
		RubiChart chart = GD.Load<RubiChart>("res://songs/test/data/normal.tres");
		chart.ConvertData().Format();

		Conductor.Reset();
		Conductor.ChartOffset = chart.Offset;
		Conductor.BpmList = chart.BpmInfo;
		
		// Set up rule set
		string ruleSet = chart.DefaultRuleset;
		string ruleSetScriptPath = ProjectSettings.GetSetting($"rubicon/rulesets/{ruleSet}").AsString();
		CSharpScript ruleSetScript = GD.Load<CSharpScript>(ruleSetScriptPath);
		GodotObject ruleSetObject = ruleSetScript.New().AsGodotObject();

		if (ruleSetObject is RuleSet set)
		{
			RuleSet = set;   
		}
		else // Fallback to Mania
		{
			GD.PrintErr($"Could not find ruleset {ruleSet} in project setting \"rubicon/rulesets/{ruleSet}\". Falling back to Mania.");
			ruleSetObject.Free();
			
			ruleSetScriptPath = ProjectSettings.GetSetting("rubicon/rulesets/mania").AsString();
			ruleSetScript = GD.Load<CSharpScript>(ruleSetScriptPath);
			RuleSet = ruleSetScript.New().AsGodotObject() as RuleSet;
		}

		if (RuleSet is null)
		{
			GD.PrintErr("RuleSet is still null. Please check your Project Settings at \"rubicon/rulesets\"");
			return;
		}
		
		// Set up play field
		PlayField = RuleSet.CreatePlayField();
		PlayField.Setup(meta, chart);
		AddChild(PlayField);

		Conductor.Start(0);
		Instrumental.Play(0f);
		Vocals.Play(0f);
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
		Instrumental.Stop();
		Vocals.Stop();
		PlayField.ProcessMode = ProcessModeEnum.Disabled;
		Paused = true;
	}

	/// <summary>
	/// Resumes the game.
	/// </summary>
	public void Resume()
	{
		Conductor.Play();	
		Instrumental.Play((float)Conductor.RawTime);
		Vocals.Play((float)Conductor.RawTime);
		PlayField.ProcessMode = ProcessModeEnum.Inherit;
		Paused = false;
	}
}
