namespace Rubicon.Sequence;

/// <summary>
/// The sequence controller used for progressing tasks. Used for Story Mode and dialogue scenes.
/// </summary>
[GlobalClass]
public partial class SequenceController : Node
{
    public static SequenceController Instance;
    
    [Export] public bool Active { get; protected set; } = false;

    [Export] public SequenceEvent[] Data;

    [Export] public int Index = 0;

    [Export] public string Interacting = "";
    
    [ExportGroup("Settings"), Export] public double TickLength = 1d / 60d;
    
    //[ExportGroup("References"), Export] public DialogueBox DialogueBox;

    private double _ticksPassed = 0d;
    private bool _firstFrame = false;

    public override void _Ready()
    {
        Instance ??= this; // Assume the first instance is the Global one used for story mode.

        base._Ready();
    }

    public void Start(SequenceEvent[] data)
    {
        Data = data;
        Index = 0;
        Active = true;
        _firstFrame = true;
        
        Data[Index].Begin(this);
    }
    
    public override void _Process(double delta)
    {
        if (_firstFrame)
        {
            _firstFrame = false;
            return;
        }
        
        if (!Active)
            return;
        
        _ticksPassed += delta;
        Data[Index].Process(Mathf.FloorToInt(_ticksPassed / TickLength), this);

        if (_ticksPassed > TickLength)
            _ticksPassed %= TickLength;

        if (!Data[Index].Advance) 
            return;
        
        Index++;

        if (Index < Data.Length)
            Data[Index].Begin(this);
        else
            End();
    }

    public void End()
    {
        Active = false;
        Index = 0;
        Data = null;
    }
}