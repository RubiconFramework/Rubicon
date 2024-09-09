using Godot.Collections;
using Rubicon.Core;
using Rubicon.Core.Chart;
using Rubicon.Core.Data;

namespace Rubicon.Modes;

public partial class NoteManager : Control
{
    /// <summary>
    /// Contains the individual chart for this manager.
    /// </summary>
    [Export] public NoteData[] Notes;

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
    [Export] public float ScrollSpeed = 1f;

    /// <summary>
    /// This note manager's parent bar line.
    /// </summary>
    [Export] public BarLine ParentBarLine;
    
    /// <summary>
    /// Is true when the manager has gone through all notes present in <see cref="Chart">Chart</see>.
    /// </summary>
    public bool IsComplete => NoteHitIndex >= Notes.Length;

    /// <summary>
    /// Is true when the manager has no notes to hit for at least a measure.
    /// </summary>
    public bool OnBreak => !IsComplete && Notes[NoteHitIndex].MsTime - Conductor.Time * 1000d >
        ConductorUtility.MeasureToMs(Conductor.CurrentMeasure, Conductor.Bpm, Conductor.TimeSigNumerator);
    
    /// <summary>
    /// The next note's index to be hit.
    /// </summary>
    [ExportGroup("Advanced"), Export] public int NoteHitIndex = 0; 
    
    /// <summary>
    /// The next hit object's index to be spawned in.
    /// </summary>
    [Export] public int NoteSpawnIndex = 0;

    public override void _Process(double delta)
    {
        base._Process(delta);
        
        // Handle note spawning
        double time = Conductor.Time * 1000d;
        SvChange currentScrollVel = ParentBarLine.Chart.SvChanges[ParentBarLine.ScrollVelocityIndex];
        if (NoteSpawnIndex < Notes.Length && Visible)
        {
            while (NoteSpawnIndex < Notes.Length && Notes[NoteSpawnIndex].MsTime - time <= 2000)
            {
                if (Notes[NoteSpawnIndex].MsTime - time < 0 || Notes[NoteSpawnIndex].WasSpawned)
                {
                    NoteSpawnIndex++;
                    continue;
                }

                Note note = SpawnNote(Notes[NoteSpawnIndex], currentScrollVel);
                AddChild(note);
                Notes[NoteSpawnIndex].WasSpawned = true;
                NoteSpawnIndex++;
            }
        }
        
        // If note hitting is done, stop here
        if (IsComplete)
            return;
        
        NoteData curNoteData = Notes[NoteHitIndex];
        if (Autoplay && InputsEnabled)
        {
            while (curNoteData.MsTime - time <= 0)
            {
                if (!Notes[NoteHitIndex].ShouldMiss)
                    OnNoteHit(curNoteData, 0, curNoteData.MsLength > 0);
                
                NoteHitIndex++;
                curNoteData = Notes[NoteHitIndex];
            }
        }

        if (curNoteData.MsTime - time <= -EngineSettings.BadHitWindow)
        {
            OnNoteMiss(curNoteData, -EngineSettings.BadHitWindow - 1, false);
            NoteHitIndex++;
        }
    }

    #region Virtual (Overridable) Methods
    protected virtual Note SpawnNote(NoteData data, SvChange svChange)
    {
        return null;
    }

    protected virtual void OnNoteHit(NoteData note, double distance, bool holding)
    {
        note.WasHit = true;
    }
    
    protected virtual void OnNoteMiss(NoteData note, double distance, bool holding)
    {
        note.WasHit = true;
    }
    #endregion
}