using Rubicon.Core.Chart;
using Rubicon.Core.Meta;

namespace Rubicon.Rulesets;

/// <summary>
/// A control node with all general gameplay-related functions. -binpuki
/// </summary>
public partial class PlayField : Control
{
    /// <summary>
    /// The current health the player has.
    /// </summary>
    [Export] public uint Health = 0;

    /// <summary>
    /// The max health the player can have.
    /// </summary>
    [Export] public uint MaxHealth = 2000;

    // lego was probably right in having a high score class, ill save this for him
    [Export] public uint Score = 0;

    [Export] public uint PerfectHits = 0;
    
    [Export] public uint GreatHits = 0;
    
    [Export] public uint GoodHits = 0;
    
    [Export] public uint BadHits = 0;
    
    [Export] public uint Misses = 0;

    /// <summary>
    /// 
    /// </summary>
    [Export] public RubiChart Chart;

    [Export] public SongMeta Metadata;
    
    [Export] public int TargetBarLine = 0;
    
    /// <summary>
    /// A signal that is emitted upon failure.
    /// </summary>
    [Signal] public delegate void OnFailEventHandler();
    
    /// <summary>
    /// Readies the PlayField for gameplay!
    /// </summary>
    /// <param name="meta">The song meta</param>
    /// <param name="chart">The chart loaded</param>
    public virtual void Setup(SongMeta meta, RubiChart chart)
    {
        Name = "Base PlayField";
        Metadata = meta;
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

    /// <summary>
    /// This function is triggered upon an update to the settings.
    /// </summary>
    public virtual void UpdateOptions()
    {
        
    }
    
    /// <summary>
    /// The fail condition for this play field.
    /// </summary>
    /// <returns>Whether the player has failed</returns>
    public virtual bool GetFailCondition() => false;
}