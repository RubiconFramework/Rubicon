using System;
using System.Linq;
using Godot.Collections;
using Rubicon.Core;
using Rubicon.Core.Chart;
using Rubicon.Core.Data;
using Array = System.Array;

namespace Rubicon.Rulesets.Mania;

/// <summary>
/// A bar line class for Mania gameplay. Also referred to as a "strum" by some.
/// </summary>
public partial class ManiaNoteManager : NoteManager
{
	/// <summary>
	/// The direction of this note manager.
	/// </summary>
	[Export] public string Direction = "";
	
	/// <inheritdoc/>
	[Export] public override float ScrollSpeed
	{
		get => base.ScrollSpeed;
		set
		{
			base.ScrollSpeed = value;
			foreach (Note note in HitObjects)
				if (note is ManiaNote maniaNote)
					maniaNote.AdjustInitialTailSize();
		}
	}

	/// <summary>
	/// The note that is currently being held.
	/// </summary>
	[Export] public NoteData NoteHeld;
	
	/// <summary>
	/// The angle the notes come from in radians.
	/// </summary>
	[Export] public float DirectionAngle = Mathf.Pi / 2f;

	/// <summary>
	/// The note skin for this manager. Please change via <see cref="ChangeNoteSkin"/>!
	/// </summary>
	[Export] public ManiaNoteSkin NoteSkin;

	/// <summary>
	/// The lane graphic for this manager.
	/// </summary>
	[Export] public AnimatedSprite2D LaneObject;

	/// <summary>
	/// Sets up this manager for Mania gameplay.
	/// </summary>
	/// <param name="parent">The parent <see cref="ManiaBarLine"/>></param>
	/// <param name="lane">The lane index</param>
	/// <param name="noteSkin">The note skin provided</param>
	public void Setup(ManiaBarLine parent, int lane, ManiaNoteSkin noteSkin)
	{
		ParentBarLine = parent;
		Lane = lane;
		Direction = noteSkin.GetDirection(lane, parent.Chart.Lanes);
		ChangeNoteSkin(noteSkin);
		
		Notes = parent.Chart.Notes.Where(x => x.Lane == Lane).ToArray();
		Array.Sort(Notes, (a, b) =>
		{
			if (a.Time < b.Time)
				return -1;
			if (a.Time > b.Time)
				return 1;
			
			return 0;
		});
	}

	public override void _Process(double delta)
	{
		if (NoteHeld != null && NoteHeld.MsTime + NoteHeld.MsLength < Conductor.Time * 1000d)
			OnNoteHit(NoteHeld, 0, false);
		
		base._Process(delta);
	}

	/// <summary>
	/// Changes the note skin for this manager. Does not change the notes on-screen automatically!
	/// </summary>
	/// <param name="noteSkin">The note skin</param>
	public void ChangeNoteSkin(ManiaNoteSkin noteSkin)
	{
		NoteSkin = noteSkin;

		LaneObject = new AnimatedSprite2D();
		LaneObject.Scale = Vector2.One * NoteSkin.Scale;
		LaneObject.SpriteFrames = NoteSkin.LaneAtlas;
		LaneObject.Play($"{Direction}LaneNeutral", 1f, true);
		LaneObject.AnimationFinished += OnAnimationFinish;
		AddChild(LaneObject);
		MoveChild(LaneObject, 0);
	}
	
	/// <inheritdoc/>
	protected override Note CreateNote() => new ManiaNote();

	/// <inheritdoc/>
	protected override void SetupNote(Note note, NoteData data, int svChange)
	{
		if (note is not ManiaNote maniaNote)
			return;
		
		maniaNote.Setup(data, svChange, this, NoteSkin);
	}

	/// <inheritdoc/>
	protected override void OnNoteHit(NoteData note, double distance, bool holding)
	{
		LaneObject.Animation = $"{Direction}LaneConfirm";
		if (!holding)
		{
			NoteHeld = null;
			LaneObject.Play();
			note.HitObject?.PrepareRecycle();
		}
		else
		{
			NoteHeld = note;
			LaneObject.Pause();   
		}
		
		base.OnNoteHit(note, distance, holding);
	}
	
	/// <inheritdoc/>
	protected override void OnNoteMiss(NoteData note, double distance, bool holding)
	{
		if (note == NoteHeld)
		{
			if (note.HitObject is ManiaNote maniaNote)
				maniaNote.UnsetHold();
			
			NoteHeld = null;
		}
		
		if (note.MsLength <= 0)
			note.HitObject.PrepareRecycle();
		
		base.OnNoteMiss(note, distance, holding);
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		string actionName = $"MANIA_{ParentBarLine.Managers.Length}K_{Lane}";
		if (Autoplay || !InputsEnabled || !InputMap.HasAction(actionName) || !@event.IsAction(actionName) || @event.IsEcho())
			return;

		if (@event.IsPressed())
		{
			NoteData[] notes = Notes;
			if (NoteHitIndex >= notes.Length)
			{
				if (LaneObject.Animation != $"{Direction}LanePress")
					LaneObject.Play($"{Direction}LanePress");
				
				return;
			}

			double songPos = Conductor.Time * 1000d; // calling it once since this can lag the game HORRIBLY if used without caution
			while (notes[NoteHitIndex].MsTime - songPos <= -(float)ProjectSettings.GetSetting("rubicon/judgments/bad_hit_window"))
			{
				// Miss every note thats too late first
				OnNoteMiss(notes[NoteHitIndex], -ProjectSettings.GetSetting("rubicon/judgments/bad_hit_window").AsDouble() - 1, false);
				NoteHitIndex++;
			}

			double hitTime = notes[NoteHitIndex].MsTime - songPos;
			if (Mathf.Abs(hitTime) <= ProjectSettings.GetSetting("rubicon/judgments/bad_hit_window").AsDouble()) // Literally any other rating
			{
				OnNoteHit(notes[NoteHitIndex], hitTime, notes[NoteHitIndex].Length > 0);
				NoteHitIndex++;
			}
			else if (hitTime < -ProjectSettings.GetSetting("rubicon/judgments/bad_hit_window").AsDouble()) // Your Miss / "SHIT" rating
			{
				LaneObject.Animation = $"{Direction}LaneConfirm";
				LaneObject.Play();
				OnNoteMiss(notes[NoteHitIndex], hitTime, true);
				NoteHitIndex++;
			}
			else
			{
				if (LaneObject.Animation != $"{Direction}LanePress")
					LaneObject.Play($"{Direction}LanePress");
			}
		}
		else if (@event.IsReleased())
		{
			if (NoteHeld != null)
			{
				double length = NoteHeld.MsTime + NoteHeld.MsLength - (Conductor.Time * 1000d);
				if (length <= ProjectSettings.GetSetting("rubicon/judgments/bad_hit_window").AsDouble())
					OnNoteHit(NoteHeld, length, false);
				else
					OnNoteMiss(NoteHeld, length, true);
			}

			if (LaneObject.Animation != $"{Direction}LaneNeutral")
				LaneObject.Play($"{Direction}LaneNeutral", 1f, true);
		}
	}

	/// <summary>
	/// Mainly for when the autoplay finishes hitting a note.
	/// </summary>
	private void OnAnimationFinish()
	{
		if (!Autoplay || LaneObject.Animation != $"{Direction}LaneConfirm")
			return;

		if (LaneObject.Animation != $"{Direction}LaneNeutral")
			LaneObject.Play($"{Direction}LaneNeutral");
	}
}
