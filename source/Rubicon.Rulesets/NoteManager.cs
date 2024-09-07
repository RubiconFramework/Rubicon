using Godot.Collections;
using Rubicon.Core;
using Rubicon.Core.Chart;
using Rubicon.Core.Data;

namespace Rubicon.Rulesets;

public partial class NoteManager : Control
{
    /// <summary>
    /// Contains the individual chart for this manager.
    /// </summary>
    [Export] public IndividualChart Chart;

    /// <summary>
    /// The distance to offset notes by position-wise.
    /// </summary>
    [Export] public float DistanceOffset;

    /// <summary>
    /// If true, the computer will hit the notes that come by.
    /// </summary>
    [Export] public bool Autoplay = true;

    /// <summary>
    /// If false, nothing can be input through this note manager. Not even the computer.
    /// </summary>
    [Export] public bool InputsEnabled = true;

    /// <summary>
    /// The scroll speed for this note manager.
    /// </summary>
    [Export]
    public float ScrollSpeed
    {
        get => _scrollSpeed;
        set => SetScrollSpeed(value);
    }
    
    /// <summary>
    /// Is true when the manager has gone through all notes present in <see cref="Chart">Chart</see>.
    /// </summary>
    public bool IsComplete => NoteHitIndex >= Chart.Notes.Length;

    /// <summary>
    /// Is true when the manager has no notes to hit for at least a measure.
    /// </summary>
    public bool OnBreak => !IsComplete && Chart.Notes[NoteHitIndex].MsTime - Conductor.Time * 1000d >
        ConductorUtility.MeasureToMs(Conductor.CurrentMeasure, Conductor.Bpm, Conductor.TimeSigNumerator);
    
    /// <summary>
    /// The next note's index to be hit.
    /// </summary>
    [ExportGroup("Advanced"), Export] public int NoteHitIndex = 0; 
    
    /// <summary>
    /// The next hit object's index to be spawned in.
    /// </summary>
    [Export] public int NoteSpawnIndex = 0;

    [Export] public int ScrollVelocityIndex = 0;

    private float _scrollSpeed = 0f;

    public override void _Process(double delta)
    {
        base._Process(delta);
        
        double time = Conductor.Time * 1000d;
        DistanceOffset = -(float)time; // Bad coding, but just for now cuz scroll velocities can come later.
        
        // Handle SV changes
        if (ScrollVelocityIndex < Chart.SvChanges.Length)
            while (ScrollVelocityIndex < Chart.SvChanges.Length && Chart.SvChanges[ScrollVelocityIndex].MsTime - time <= 0)
                ScrollVelocityIndex++;
        
        // Handle note spawning
        if (NoteSpawnIndex < Chart.Notes.Length && Visible)
        {
            while (NoteSpawnIndex < Chart.Notes.Length && Chart.Notes[NoteSpawnIndex].MsTime - time <= 2000)
            {
                if (Chart.Notes[NoteSpawnIndex].MsTime - time < 0)
                {
                    NoteSpawnIndex++;
                    continue;
                }

                Note note = SpawnNote(Chart.Notes[NoteSpawnIndex], Chart.SvChanges[ScrollVelocityIndex]);
                AddChild(note);
                NoteSpawnIndex++;
            }
        }
        
        // If note hitting is done, stop here
        if (IsComplete)
            return;
        
        NoteData curNoteData = Chart.Notes[NoteHitIndex];
        if (Autoplay && InputsEnabled)
        {
            while (curNoteData.MsTime - time <= 0)
            {
                if (!Chart.Notes[NoteHitIndex].ShouldMiss)
                    OnNoteHit(curNoteData, 0, curNoteData.MsLength > 0);

                NoteHitIndex++;
                curNoteData = Chart.Notes[NoteHitIndex];
            }
        }

        if (curNoteData.MsTime - time <= -EngineSettings.BadHitWindow)
        {
            OnNoteMiss(curNoteData, -EngineSettings.BadHitWindow - 1, false);
            NoteHitIndex++;
        }
    }

    protected void SetScrollSpeed(float scrollSpeed)
    {
        _scrollSpeed = scrollSpeed;

        if (Chart.SvChanges.Length <= 1)
            return;

        for (int i = 1; i < Chart.SvChanges.Length; i++)
        {
            SvChange currentChange = Chart.SvChanges[i];
            SvChange previousChange = Chart.SvChanges[i - 1];

            currentChange.StartingPosition =
                (float)(previousChange.StartingPosition + (currentChange.MsTime - previousChange.MsTime)) * scrollSpeed;
        }
    }

    #region Virtual (Overridable) Methods
    protected virtual Note SpawnNote(NoteData data, SvChange svChange)
    {
        return null;
    }

    protected virtual void OnNoteHit(NoteData note, double distance, bool holding)
    {
        
    }
    
    protected virtual void OnNoteMiss(NoteData note, double distance, bool holding)
    {
        
    }
    #endregion
}