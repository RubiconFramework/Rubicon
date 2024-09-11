using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rubicon.Core.Chart;
using Rubicon.Rulesets;

namespace Rubicon.Game;

public partial class RubiconGame : Node
{
    public RuleSet RuleSet;
    
    public PlayField PlayField;
    
    public override void _Ready()
    {
        // Shitty
        RubiChart chart = GD.Load<RubiChart>("res://songs/test/data/normal.json");

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
    }
}