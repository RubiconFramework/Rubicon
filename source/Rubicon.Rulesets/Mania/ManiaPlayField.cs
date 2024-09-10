using Rubicon.Core.Chart;

namespace Rubicon.Modes.Mania;

/// <summary>
/// A <see cref="PlayField"/> class with Mania-related gameplay incorporated. Also the main mode for Rubicon Engine.
/// </summary>
public partial class ManiaPlayField : PlayField
{
    [Export] public ManiaBarLine[] BarLines;
    
    public override void Setup(RubiChart chart)
    {
        base.Setup(chart);

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
            curBarLine.Position = new Vector2(i * 480f - (chart.Charts.Length - 1) * 480f / 2f, 0f);
            
            AddChild(curBarLine);
            BarLines[i] = curBarLine;
        }
    }
    
    public override bool GetFailCondition() => Health <= 0;
}