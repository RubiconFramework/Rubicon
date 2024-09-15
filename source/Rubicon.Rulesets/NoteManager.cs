using System.Linq;
using Godot.Collections;
using Rubicon.Core;
using Rubicon.Core.Chart;
using Rubicon.Core.Data;
using Array = Godot.Collections.Array;

namespace Rubicon.Rulesets;

/// <summary>
/// A base note manager for Rubicon rulesets.
/// </summary>
public partial class NoteManager : Control
{
	/// <summary>
	/// The lane index of this note manager.
	/// </summary>
	[Export] public int Lane = 0;
	
	/// <summary>
	/// Contains the individual notes for this manager.
	/// </summary>
	[Export] public NoteData[] Notes;

	/// <summary>
	/// Contains the visual hit objects for this manager. Notes are recycled.
	/// </summary>
	[Export] public Array<Note> HitObjects = new();

	/// <summary>
	/// If true, the computer will hit the notes that come by.
	/// </summary>
	[Export] public bool Autoplay = true;

	/// <summary>
	/// If false, nothing can be input through this note manager. Not even the computer.
	/// </summary>
	[Export] public bool InputsEnabled = true;

	/// <summary>
	/// The constant rate at which the notes go down. This is different from scroll velocities.
	/// </summary>
	[Export] public virtual float ScrollSpeed { get; set; } = 1f;

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

	/// <summary>
	/// The queue list for notes to be processed next frame.
	/// </summary>
	[Export] public Array<NoteInputElement> ProcessQueue = new();

	public override void _Process(double delta)
	{
		base._Process(delta);
		
		// Handle note spawning
		double time = Conductor.Time * 1000d;
		if (NoteSpawnIndex < Notes.Length && Visible)
		{
			while (NoteSpawnIndex < Notes.Length && Notes[NoteSpawnIndex].MsTime - time <= 2000)
			{
				if (Notes[NoteSpawnIndex].MsTime - time < 0 || Notes[NoteSpawnIndex].WasSpawned)
				{
					NoteSpawnIndex++;
					continue;
				}

				Note note = HitObjects.FirstOrDefault(x => !x.Active);
				if (note == null)
				{
					note = CreateNote();
					HitObjects.Add(note);
					AddChild(note);
				}
				else
				{
					note.MoveToFront();
					note.Reset();
				}

				note.Name = $"Note {NoteSpawnIndex}";
				SetupNote(note, Notes[NoteSpawnIndex]);
				Notes[NoteSpawnIndex].WasSpawned = true;
				NoteSpawnIndex++;
			}
		}
		
		if (!IsComplete)
		{
			NoteData curNoteData = Notes[NoteHitIndex];
			if (Autoplay && InputsEnabled)
			{
				while (curNoteData.MsTime - time <= 0)
				{
					if (!Notes[NoteHitIndex].ShouldMiss)
						ProcessQueue.Add(new NoteInputElement{ Note = curNoteData, Distance = 0, Holding = curNoteData.MsLength > 0});
				
					NoteHitIndex++;
					if (NoteHitIndex >= Notes.Length)
						break;
				
					curNoteData = Notes[NoteHitIndex];
				}
			}

			if (curNoteData.MsTime - time <= -ProjectSettings.GetSetting("rubicon/judgments/horrible_hit_window").AsDouble())
			{
				ProcessQueue.Add(new NoteInputElement{ Note = Notes[NoteHitIndex], Distance = -ProjectSettings.GetSetting("rubicon/judgments/horrible_hit_window").AsDouble() - 1, Holding = false});
				NoteHitIndex++;
			}	
		}

		for (int i = 0; i < ProcessQueue.Count; i++)
		{
			OnNoteHit(ProcessQueue[i]);
			ProcessQueue[i].Free();
		}
			
		ProcessQueue.Clear();
	}

	private int GetNoteScrollVelocityIndex(NoteData noteData)
	{
		SvChange[] svChangeList = ParentBarLine.Chart.SvChanges;
		int index = svChangeList.Length - 1;
		for (int i = 0; i < svChangeList.Length; i++)
		{
			if (svChangeList[i].Time > noteData.Time)
			{
				index = i - 1;
				break;
			}
		}

		return index;
	}

	#region Virtual (Overridable) Methods

	/// <summary>
	/// Is called when creating a new note. Override to replace with a type that inherits from <see cref="Note"/>.
	/// </summary>
	/// <returns>A new note.</returns>
	protected virtual Note CreateNote() => new Note();

	/// <summary>
	/// Called when setting up a note. Notes will be recycled.
	/// </summary>
	/// <param name="note">The note passed in</param>
	/// <param name="data">The note data</param>
	/// <param name="svChange">The SV change associated</param>
	protected virtual void SetupNote(Note note, NoteData data)
	{
		
	}

	/// <summary>
	/// Triggers upon this note manager hitting/missing a note.
	/// </summary>
	/// <param name="element">Contains information about a note and its hits</param>
	protected virtual void OnNoteHit(NoteInputElement element)
	{
		element.Note.WasHit = true;
		ParentBarLine.OnNoteHit(Lane, element);
	}
	#endregion
}
