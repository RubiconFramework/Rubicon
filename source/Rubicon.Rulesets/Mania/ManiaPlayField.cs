using Rubicon.Autoload;
using Rubicon.Core.Chart;
using Rubicon.Core.Meta;

namespace Rubicon.Rulesets.Mania;

/// <summary>
/// A <see cref="PlayField"/> class with Mania-related gameplay incorporated. Also the main mode for Rubicon Engine.
/// </summary>
public partial class ManiaPlayField : PlayField
{
    /// <summary>
    /// The bar lines associated with this play field. Sometimes referred to as "strumlines".
    /// </summary>
    [Export] public ManiaBarLine[] BarLines;

    /// <summary>
    /// A control node for the general location for the bar lines.
    /// </summary>
    [Export] public Control BarLineContainer;
    
    /// <summary>
    /// Readies this PlayField for Mania gameplay!
    /// </summary>
    /// <param name="meta">The song meta</param>
    /// <param name="chart">The chart loaded</param>
    public override void Setup(SongMeta meta, RubiChart chart)
    {
        base.Setup(meta, chart);
        Name = "Mania PlayField";

        BarLineContainer = new Control();
        BarLineContainer.Name = "Bar Line Container";
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
            curBarLine.SetDirectionAngle(!Settings.General.Downscroll ? Mathf.Pi / 2f : -Mathf.Pi / 2f);
            
            // Using Council positioning for now, sorry :/
            curBarLine.Position = new Vector2(i * 720f - (chart.Charts.Length - 1) * 720f / 2f, 0f);
            
            BarLineContainer.AddChild(curBarLine);
            BarLines[i] = curBarLine;
        }
        
        BarLines[meta.PlayerChartIndex].SetAutoPlay(false);
    }
    
    /// <inheritdoc/>
    public override void UpdateOptions()
    {
        LayoutPreset barLinePreset =
            Settings.General.Downscroll ? LayoutPreset.CenterBottom : LayoutPreset.CenterTop;
        BarLineContainer.SetAnchorsPreset(barLinePreset);
        BarLineContainer.Position = new Vector2(0f, Settings.General.Downscroll ? -120f : 120f);
    }
    
    /// <inheritdoc />
    public override bool GetFailCondition() => Health <= 0;
}