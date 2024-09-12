using Rubicon.Core.Chart;

namespace Rubicon.Rulesets;

/// <summary>
/// A control node with all general gameplay-related functions. -binpuki
/// this is just playstate. -duo
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
    
    [Signal] public delegate void OnFailEventHandler();
    
    public virtual void Setup(RubiChart chart)
    {
        Chart = chart;
        Chart.ConvertData().Format();
        SetAnchorsPreset(LayoutPreset.FullRect);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        
        if (GetFailCondition())
            Fail();
    }

    /// <summary>
    /// Instantly kills the player and emits the signal.
    /// </summary>
    public void Fail()
    {
        EmitSignal(SignalName.OnFail);
    }

    public virtual void UpdateOptions()
    {
        
    }

    public virtual void UpdateScore()
    {
        
    }
    
    public virtual bool GetFailCondition() => false;
}