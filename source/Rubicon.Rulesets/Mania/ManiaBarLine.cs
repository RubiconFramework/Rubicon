using Rubicon.Core.Chart;

namespace Rubicon.Rulesets.Mania;

public partial class ManiaBarLine : BarLine
{
    [Export] public ManiaNoteSkin NoteSkin;
    
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