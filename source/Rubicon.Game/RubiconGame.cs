using System.Collections.Generic;
using System.Linq;
using Rubicon.Core.Chart;
using Rubicon.Rulesets;

namespace Rubicon.Game;

public partial class RubiconGame : Node
{
    public PlayField PlayField;
    
    public override void _Ready()
    {
        RubiChart chart = GD.Load<RubiChart>("res://songs/test/data/normal.json");
    }
}