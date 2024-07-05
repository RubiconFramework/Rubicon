#region Imports
	using System.Collections.Generic;
	using Rubicon.Gameplay.Classes.Notes;
	using Rubicon.Gameplay.Resources;
#endregion

namespace Rubicon.Gameplay.Classes.Strums;

public partial class StrumLine : Node2D
{
	public List<dynamic> FocusedCharacters = new();
	public bool AutoPlay = true;
	public List<Note> NotesToHit = new();

	public void RemoveFromList(Note note)
	{
		if(NotesToHit.Contains(note))
			NotesToHit.Remove(note);
			note.QueueFree();
	}

	public override void _Ready()
	{
		for (int i = 0; i < GetChildCount(); i++)
			GetChild<Strum>(i).Direction = i;
	}

	public void ApplyStyle(UIStyle Style)
	{
		foreach (Strum strum in GetChildren())
		{
			strum.SpriteFrames = Style.StrumTexture;
			strum.PlayAnim("static");
			strum.Scale = new Vector2(Style.StrumScale,Style.StrumScale);
			strum.idleTransparency = Style.StrumTransparency;
			strum.Modulate = new(1, 1, 1, strum.idleTransparency);
		}
		GD.Print("Strumline generated.");
	}
}
