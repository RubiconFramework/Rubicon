using System.Collections.Generic;
using System.Linq;
using Rubicon.backend.autoload;
using Rubicon.gameplay.classes.elements;
using Rubicon.gameplay.classes.notes;

namespace Rubicon.gameplay.classes.strums;
[GlobalClass]
public partial class StrumHandler : Node
{
	/*	Handles the currently selected strumline and lets you
		quickly swap to a different strumline.
		Useful for multi-character support.	*/

	public StrumLine FocusedStrumline;
	public List<string> Controls = new();
	[NodePath("../NoteHandler")] public NoteHandler noteHandler;

	public override void _Ready() => this.OnReady();

	public void FocusStrumline(ref StrumLine strumline, bool setupFocusedChar = true)
	{
		if(FocusedStrumline is not null)
		{
			if (setupFocusedChar) 
				foreach(Character2D character in FocusedStrumline.FocusedCharacters) character.IsPlayer = false;
			
			foreach(Strum strum in FocusedStrumline.GetChildren())
			{
				strum.PlayAnim("static");
				strum.Pressed = false;
			}
			FocusedStrumline.AutoPlay = true;
		}

		FocusedStrumline = strumline;
		strumline.AutoPlay = false;
		Controls.Clear();
		foreach(Strum strum in strumline.GetChildren())
			Controls.Add($"note_{strum.Name.ToString().ToLower()}");

		if (setupFocusedChar) 
			foreach(Character2D character in FocusedStrumline.FocusedCharacters) character.IsPlayer = true;
		
		GD.Print(Controls.Count > 0 ? "StrumHandler recieved the strumline correctly." : "StrumHandler recieved an empty strumline.");
	}

	public override void _Input(InputEvent @event) {
		if (@event is InputEventKey && FocusedStrumline is not null && !FocusedStrumline.AutoPlay)
		{
			foreach(Strum strum in FocusedStrumline.GetChildren())
			{
				if(Input.IsActionJustPressed(Controls[strum.Direction]))
				{
					strum.Pressed = true;
					strum.PlayAnim("pressed");
					
					if(FocusedStrumline.NotesToHit.Count > 0){
						List<Note> ClosestNotes = FocusedStrumline.NotesToHit.Where(note => note.Direction == strum.Direction).ToList();

						Note ClosestNote = null;
						if(ClosestNotes.Count > 0)
						{
							ClosestNote = ClosestNotes.OrderBy(note => Math.Abs(note.Time - Conductor.SongPosition)).First();
							foreach (var note in ClosestNotes.Where(note => note.Time/2 < ClosestNote.Time && note.TimeToHit && !note.WasHit))
							{
								//note.GetParent<NoteHandler>().NoteMiss(note,false);
								FocusedStrumline.NotesToHit.Remove(note);
							}
						}
						if (ClosestNote is not null && !ClosestNote.WasMissed) noteHandler.NoteHit(ClosestNote);
					}
				}
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		foreach(var node in FocusedStrumline.GetChildren())
		{
			var strum = (Strum)node;
			if(strum.Pressed && !Input.IsActionPressed(Controls[strum.Direction]))
			{
				strum.Pressed = false;
				strum.PlayAnim("static",true);
			}
		}

		bool IsPressingNote = false;
		foreach(Strum strum in FocusedStrumline.GetChildren())
			if(strum.Pressed) IsPressingNote = true;
		
		if(!IsPressingNote)
		{
			foreach (Character2D character in FocusedStrumline.FocusedCharacters.Where(character => character.LastAnim.AnimName.StartsWith("sing") && character.SingTimer >= Conductor.StepDuration * (character.SingDuration * 0.0011)))
			{
				character.Dance(true);
				character.SingTimer = 0;
			}
		}
	}
}
