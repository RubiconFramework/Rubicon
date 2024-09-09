using Rubicon.Core.Chart;

namespace Rubicon.Modes;

/// <summary>
/// A control node with all general gameplay-related functions.
/// </summary>
public partial class PlayField : Control
{
    [Export] public uint Health = 0;

    [Export] public uint MaxHealth = 200;

    [Export] public uint Score = 0;

    [Export] public uint PerfectHits = 0;
    
    [Export] public uint GreatHits = 0;
    
    [Export] public uint GoodHits = 0;
    
    [Export] public uint BadHits = 0;
    
    [Export] public uint Misses = 0;

    [Export] public RubiChart Chart;
    
    [Export] public int TargetBarLine = 0;
    
    

    public virtual void Setup(RubiChart chart)
    {
        Chart = chart;
    }

    public virtual void UpdateScore()
    {
        
    }
    
    public virtual bool GetFailCondition() => false;
}