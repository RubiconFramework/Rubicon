using Rubicon.Core;
using Rubicon.Core.Chart;
using Rubicon.Core.Data;
using Rubicon.Core.Meta;
using Rubicon.Core.UI;

namespace Rubicon.Rulesets;

/// <summary>
/// A control node with all general gameplay-related functions. -binpuki
/// ITS PLAYSTATE -duo
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
    [Export] public uint MaxHealth = 1000;

    // lego was probably right in having a high score class, ill save this for him
    [Export] public uint Score = 0;

    [Export] public float Accuracy = 100f;

    [Export] public uint PerfectHits = 0;
    
    [Export] public uint GreatHits = 0;
    
    [Export] public uint GoodHits = 0;

    [Export] public uint OkayHits = 0;
    
    [Export] public uint BadHits = 0;
    
    [Export] public uint Misses = 0;

    [Export] public uint Combo = 0;

    [Export] public uint HighestCombo = 0;

    /// <summary>
    /// The Chart for this PlayField.
    /// </summary>
    [Export] public RubiChart Chart;

    /// <summary>
    /// The Song meta for this PlayField
    /// </summary>
    [Export] public SongMeta Metadata;

    /// <summary>
    /// The UiStyle currently being used
    /// </summary>
    [Export] public UiStyle UiStyle;
    
    /// <summary>
    /// The bar lines associated with this play field.
    /// </summary>
    [Export] public BarLine[] BarLines;
    
    /// <summary>
    /// The Target Bar Line's name for the player to control
    /// </summary>
    [Export] public string TargetBarLine = "Player";
    
    /// <summary>
    /// The Target Bar Line's index for the player to control
    /// </summary>
    [Export] public int TargetBarLineIndex = 0;
    
    /// <summary>
    /// A signal that is emitted upon failure.
    /// </summary>
    [Signal] public delegate void OnFailEventHandler();

    /// <summary>
    /// The Judgment instance for this play field.
    /// </summary>
    public Judgment Judgment;

    /// <summary>
    /// The ComboDisplay instance for this play field.
    /// </summary>
    public ComboDisplay ComboDisplay;

    /// <summary>
    /// The HitDistance instance for this play field.
    /// </summary>
    public HitDistance HitDistance;
    
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
        Input.UseAccumulatedInput = false;
        
        // Handle UI Style
        string uiStylePath = $"res://Resources/UI/{Metadata.UiStyle}/Style.tres";
        if (!ResourceLoader.Exists(uiStylePath))
        {
            string defaultUiPath = $"res://Resources/UI/{ProjectSettings.GetSetting("rubicon/general/default_ui_style")}/style.tres";
            GD.PrintErr($"UI Style Path: {uiStylePath} does not exist. Defaulting to {defaultUiPath}");
            uiStylePath = defaultUiPath;
        }
        UiStyle = GD.Load<UiStyle>(uiStylePath);

        if (UiStyle.HitDistance != null)
        {
            HitDistance = UiStyle.HitDistance.Instantiate<HitDistance>();
            AddChild(HitDistance);   
        }

        if (UiStyle.Judgment != null)
        {
            Judgment = UiStyle.Judgment.Instantiate<Judgment>();
            Judgment.PerfectMaterial = UiStyle.PerfectMaterial;
            Judgment.GreatMaterial = UiStyle.GreatMaterial;
            Judgment.GoodMaterial = UiStyle.GoodMaterial;
            Judgment.OkayMaterial = UiStyle.OkayMaterial;
            Judgment.BadMaterial = UiStyle.BadMaterial;
            Judgment.MissMaterial = UiStyle.MissMaterial;
            AddChild(Judgment);   
        }

        if (UiStyle.Combo != null)
        {
            ComboDisplay = UiStyle.Combo.Instantiate<ComboDisplay>();
            ComboDisplay.PerfectMaterial = UiStyle.PerfectMaterial;
            ComboDisplay.GreatMaterial = UiStyle.GreatMaterial;
            ComboDisplay.GoodMaterial = UiStyle.GoodMaterial;
            ComboDisplay.OkayMaterial = UiStyle.OkayMaterial;
            ComboDisplay.BadMaterial = UiStyle.BadMaterial;
            ComboDisplay.MissMaterial = UiStyle.MissMaterial;
            AddChild(ComboDisplay);   
        }
        
        for (int i = 0; i < BarLines.Length; i++)
            BarLines[i].NoteHit += OnNoteHit;
        
        UpdateOptions();
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
    /// Triggers every time the player hits a note to update the in-game statistics
    /// </summary>
    public virtual void UpdateStatistics()
    {
        
    }
    
    /// <summary>
    /// The fail condition for this play field.
    /// </summary>
    /// <returns>Whether the player has failed</returns>
    public virtual bool GetFailCondition() => false;

    /// <summary>
    /// The function that is connected to the bar lines when a note is hit. Can be overriden if needed for a specific ruleset.
    /// </summary>
    /// <param name="barLine">The bar line</param>
    /// <param name="lane">The lane</param>
    /// <param name="direction">The sing direction</param>
    /// <param name="inputElement">Info about the input recieved</param>
    protected virtual void OnNoteHit(BarLine barLine, int lane, string direction, NoteInputElement inputElement)
    {
        if (BarLines[TargetBarLineIndex] == barLine)
        {
            HitType hit = inputElement.Hit;
            Combo = hit != HitType.Miss ? Combo + 1 : 0;
            if (Combo > HighestCombo)
                HighestCombo = Combo;

            switch (hit)
            {
                case HitType.Perfect:
                    PerfectHits++;
                    break;
                case HitType.Great:
                    GreatHits++;
                    break;
                case HitType.Good:
                    GoodHits++;
                    break;
                case HitType.Okay:
                    OkayHits++;
                    break;
                case HitType.Bad:
                    BadHits++;
                    break;
                case HitType.Miss:
                    Misses++;
                    break;
            }
            
            UpdateStatistics();
            Judgment?.Play(hit, UiStyle.JudgmentOffset);   
            ComboDisplay?.Play(Combo, hit, UiStyle.ComboOffset);
            HitDistance?.Show(inputElement.Distance, UiStyle.HitDistanceOffset);
        }
    }
}