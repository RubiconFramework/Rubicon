using Rubicon.Core.Chart;

namespace Rubicon.Rulesets.Mania;

/// <summary>
/// The bar line class for Mania gameplay.
/// </summary>
public partial class ManiaBarLine : BarLine
{
    /// <summary>
    /// The note skin associated with this bar line.
    /// </summary>
    [Export] public ManiaNoteSkin NoteSkin;
    
    /// <summary>
    /// Sets up this bar line for usage in Mania gameplay.
    /// </summary>
    /// <param name="chart">The individual chart provided</param>
    /// <param name="noteSkin">The note skin</param>
    /// <param name="scrollSpeed">The scroll speed</param>
    public void Setup(IndividualChart chart, ManiaNoteSkin noteSkin, float scrollSpeed)
    {
        Chart = chart;
        NoteSkin = noteSkin;

        Managers = new NoteManager[chart.Lanes];
        for (int i = 0; i < chart.Lanes; i++)
        {
            ManiaNoteManager noteMan = new ManiaNoteManager();
            noteMan.Setup(this, i, noteSkin);
            noteMan.Position = new Vector2(i * NoteSkin.LaneSize - ((chart.Lanes - 1) * NoteSkin.LaneSize / 2f), 0);
            noteMan.Name = $"Mania Note Manager {i}";
            noteMan.ScrollSpeed = scrollSpeed;
            
            AddChild(noteMan);
            Managers[i] = noteMan;
        }
    }
}