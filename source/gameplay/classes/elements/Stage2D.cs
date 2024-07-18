
namespace Rubicon.gameplay.classes.elements;
public partial class Stage2D : Node2D
{
	/*	im tired of these stupid ass descriptions bruh
		like you wont know what a stage is	*/

	[Export] public float CameraZoom = 1;
	[NodePath("Positions")] public Node2D CharPosGroup;
	public override void _Ready() {
		this.OnReady();
	}

	public Vector2 GetCharPosition(string charName)
	{
		Marker2D character = CharPosGroup.GetNodeOrNull<Marker2D>(charName);
		if (character is null) character = new Marker2D();

		return character.Position;
	}

	public Vector2 GetCharCamera(string charName)
	{
		Marker2D character = CharPosGroup.GetNodeOrNull<Marker2D>(charName);
		if (character is null) {
			GD.PrintErr($"Camera position node not found for character: {charName}.");
			return Vector2.Zero;
		}

		return character.GetNode<Node2D>("CameraPos").GlobalPosition;
	}
}
