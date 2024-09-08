using Rubicon.Core.Chart;

namespace Rubicon.Modes.Mania;

public partial class ManiaPlayField : PlayField
{
    [Export] public ManiaBarLine[] BarLines;
    
    public override void Setup(RubiChart chart)
    {
        base.Setup(chart);

        BarLines = new ManiaBarLine[chart.Charts.Length];
        foreach (var indChart in chart.Charts)
        {
            ManiaBarLine curBarLine = new ManiaBarLine();
            
        }
    }
    
    public override bool GetFailCondition() => Health <= 0;
}