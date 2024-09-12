using Rubicon.Autoload;
using Rubicon.Core.Chart;

namespace Rubicon.Rulesets.Mania;

/// <summary>
/// A <see cref="PlayField"/> class with Mania-related gameplay incorporated. Also the main mode for Rubicon Engine.
/// </summary>
public partial class ManiaPlayField : PlayField
{
    [Export] public ManiaBarLine[] BarLines;

    [Export] public Control BarLineContainer;
    
    public override void Setup(RubiChart chart)
    {
        base.Setup(chart);

        BarLineContainer = new Control();
        AddChild(BarLineContainer);
        
        UpdateOptions();
        
        BarLines = new ManiaBarLine[chart.Charts.Length];
        
        // REALLY SHITTY, REPLACE BELOW LATER !!!
        ManiaNoteSkin noteSkin = GD.Load<ManiaNoteSkin>("res://resources/ui/funkin/mania.tres");
        for (int i = 0; i < chart.Charts.Length; i++)
        {
            IndividualChart indChart = chart.Charts[i];
            ManiaBarLine curBarLine = new ManiaBarLine();
            curBarLine.Setup(indChart, noteSkin, chart.ScrollSpeed);
            curBarLine.Name = "Mania Bar Line " + i;
            
            // Using Council positioning for now, sorry :/
            curBarLine.Position = new Vector2(i * 720f - (chart.Charts.Length - 1) * 720f / 2f, 0f);
            
            BarLineContainer.AddChild(curBarLine);
            BarLines[i] = curBarLine;
        }
        
        // On Test Chart, index 1 is player sooo
        BarLines[1].SetAutoPlay(false);
    }
    
    public override bool GetFailCondition() => Health <= 0;
    
    public override void UpdateOptions()
    {
        LayoutPreset barLinePreset =
            Settings.ClientSettings.Downscroll ? LayoutPreset.CenterBottom : LayoutPreset.CenterTop;
        BarLineContainer.SetAnchorsPreset(barLinePreset);
        BarLineContainer.Position = new Vector2(0f, Settings.ClientSettings.Downscroll ? -120f : 120f);
    }
}