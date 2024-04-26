using Godot.Collections;

namespace Rubicon.common.stages.basestage;

public partial class Stage : Node2D
{
	[NodePath("../")] GameplayScene GameplayScene;

	[Export] public float defaultCamZoom = 1;
	[ExportGroup("Camera Offset")]
	[Export] public Vector2 playerCamOffset = Vector2.Zero;
	[Export] public Vector2 opponentCamOffset = Vector2.Zero;

	public Dictionary<string, Vector2> characterPositions = new();

	public override void _Ready()
	{
		Node2D charPositions = GetNode<Node2D>("Character Positions");
		for (int i = 0; i < charPositions.GetChildCount(); i++)
		{
			Node2D character = charPositions.GetChild<Node2D>(i);
			characterPositions.Add(character.Name, character.Position);
		}
	}
}
