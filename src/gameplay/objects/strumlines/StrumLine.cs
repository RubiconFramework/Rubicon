using System;
using Rubicon.gameplay.objects.resources;

namespace Rubicon.gameplay.objects.strumlines;

public partial class StrumLine : Node2D
{
	[Export] public bool readsInput;
	[Export] public string[] controls = Array.Empty<string>();

	public UIStyle uiStyle;

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey && readsInput) detectInput();
	}

	private void detectInput()
	{
		for (int i = 0; i < GetChildCount(); i++)
		{
			if(Input.IsActionJustPressed(controls[i])){
				Strum strum = (Strum)GetChild(i);
				strum.playAnim("pressed");
			}

			if (!Input.IsActionJustReleased(controls[i])) continue;
			{
				Strum strum = (Strum)GetChild(i);
				strum.playAnim("static");
			}
		}
	}
}
