using Rubicon.Core;
using Rubicon.Core.Chart;
using Rubicon.Core.Data;

namespace Rubicon.Rulesets;

/// <summary>
/// A base bar line for Rubicon rulesets
/// </summary>
public partial class BarLine : Control
{
	/// <summary>
	/// The individual chart for this bar line. Contains notes and scroll velocity changes.
	/// </summary>
	[Export] public IndividualChart Chart;
	
	/// <summary>
	/// Contains all the nodes used to manage notes.
	/// </summary>
	[Export] public NoteManager[] Managers;

	/// <summary>
	/// The distance to offset notes by position-wise.
	/// </summary>
	[Export] public float DistanceOffset = 0;
	
	/// <summary>
	/// The index of the current scroll velocity.
	/// </summary>
	[Export] public int ScrollVelocityIndex = 0;
	
	/// <summary>
	/// A signal that is emitted every time a manager in this bar line hits a note. Can be a miss.
	/// </summary>
	[Signal] public delegate void NoteHitEventHandler(BarLine barLine, int lane, NoteData noteData, int hitType, double distance, bool holding);
	
	public override void _Process(double delta)
	{
		base._Process(delta);
		
		// Handle SV changes
		if (Chart?.SvChanges == null)
			return;
		
		double time = Conductor.Time * 1000d;
		if (ScrollVelocityIndex < Chart.SvChanges.Length)
			while (ScrollVelocityIndex + 1 < Chart.SvChanges.Length && Chart.SvChanges[ScrollVelocityIndex].MsTime - time <= 0)
				ScrollVelocityIndex++;
		
		SvChange currentScrollVel = Chart.SvChanges[ScrollVelocityIndex];
		DistanceOffset = -(float)(currentScrollVel.Position + (Conductor.Time * 1000d - currentScrollVel.MsTime) * currentScrollVel.Multiplier);
	}

	/// <summary>
	/// Triggers upon one of its note managers hitting a note in any way, even if it's a miss.
	/// </summary>
	/// <param name="lane">The lane index of the note manager</param>
	/// <param name="noteData">The note's data</param>
	/// <param name="hit">The type of hit</param>
	/// <param name="distance">The hit distance from the note's time</param>
	/// <param name="holding">Whether the note is being held down</param>
	public virtual void OnNoteHit(int lane, NoteData noteData, HitType hit, double distance, bool holding)
	{
		EmitSignal(SignalName.NoteHit, this, lane, noteData, (int)hit, distance, holding);
	}

	public void SetAutoPlay(bool autoplay)
	{
		foreach (NoteManager noteManager in Managers)
			noteManager.Autoplay = autoplay;
	}

	public void SetScrollSpeed(float scrollSpeed)
	{
		foreach (NoteManager noteManager in Managers)
			noteManager.ScrollSpeed = scrollSpeed;
	}
}
