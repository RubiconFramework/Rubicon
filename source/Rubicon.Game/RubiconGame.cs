using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rubicon.Core;
using Rubicon.Core.Chart;
using Rubicon.Rulesets;

namespace Rubicon.Game;

public partial class RubiconGame : Node
{
	public RuleSet RuleSet;
	
	public PlayField PlayField;

	[NodePath("Instrumental"), Export] public AudioStreamPlayer Instrumental;
	
	[NodePath("Vocals"), Export] public AudioStreamPlayer Vocals;
	
	public override void _Ready()
	{
		// Shitty
		RubiChart chart = GD.Load<RubiChart>("res://songs/test/data/normal.tres");

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
		PlayField.Setup(chart);
		AddChild(PlayField);

		Conductor.Start(0);
		Instrumental.Play(0f);
		Vocals.Play(0f);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (Input.IsActionJustPressed("GAME_PAUSE"))
		{
			if (!Conductor.Playing)
			{
				Conductor.Play();	
				Instrumental.Play((float)Conductor.RawTime);
				Vocals.Play((float)Conductor.RawTime);
				GD.Print("resume");
			}
			else
			{
				Conductor.Pause();
				Instrumental.Stop();
				Vocals.Stop();
				GD.Print("pause");
			}
		}
	}
}
