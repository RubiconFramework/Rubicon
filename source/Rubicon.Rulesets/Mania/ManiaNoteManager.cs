using System;
using System.Linq;
using Godot.Collections;
using Rubicon.Core;
using Rubicon.Core.Chart;
using Rubicon.Core.Data;
using Array = System.Array;

namespace Rubicon.Rulesets.Mania;

public partial class ManiaNoteManager : NoteManager
{
	[Export] public string Direction = "";

	[Export] public override float ScrollSpeed
	{
		get => base.ScrollSpeed;
		set
		{
			base.ScrollSpeed = value;
			foreach (Note note in HitObjects)
				if (note is ManiaNote maniaNote)
					maniaNote.AdjustTailSize();
		}
	}

	[Export] public NoteData NoteHeld;
	
	[Export] public float DirectionAngle = Mathf.Pi / 2f;

	[Export] public ManiaNoteSkin NoteSkin;

	[Export] public AnimatedSprite2D LaneObject;

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

	public void ChangeNoteSkin(ManiaNoteSkin noteSkin)
	{
		NoteSkin = noteSkin;

		LaneObject = new AnimatedSprite2D();
		LaneObject.Scale = Vector2.One * NoteSkin.Scale;
		LaneObject.SpriteFrames = NoteSkin.LaneAtlas;
		LaneObject.Play($"{Direction}ManagerNeutral", 1f, true);
		LaneObject.AnimationFinished += OnAnimationFinish;
		AddChild(LaneObject);
		MoveChild(LaneObject, 0);
	}
	
	protected override Note CreateNote() => new ManiaNote();

	protected override void SetupNote(Note note, NoteData data, SvChange svChange)
	{
		if (note is not ManiaNote maniaNote)
			return;
		
		maniaNote.Setup(data, svChange, this, NoteSkin);
	}

	protected override void OnNoteHit(NoteData note, double distance, bool holding)
	{
		LaneObject.Play($"{Direction}ManagerConfirm");
		if (!holding)
		{
			NoteHeld = null;
			note.HitObject.PrepareRecycle();
		}
		else
		{
			NoteHeld = note;
			LaneObject.Pause();   
		}
		
		base.OnNoteHit(note, distance, holding);
	}
	
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
				LaneObject.Play($"{Direction}ManagerPress", 1f, true);
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
				LaneObject.Play($"{Direction}ManagerConfirm", 1f, true);
				OnNoteMiss(notes[NoteHitIndex], hitTime, true);
				NoteHitIndex++;
			}
			else
			{
				LaneObject.Play($"{Direction}ManagerPress", 1f, true);
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

			LaneObject.Play($"{Direction}ManagerNeutral", 1f, true);
		}
	}

	private void OnAnimationFinish()
	{
		if (!Autoplay || LaneObject.Animation != $"{Direction}ManagerConfirm")
			return;

		LaneObject.Play($"{Direction}ManagerNeutral");
		LaneObject.Frame = 0;
	}
}
