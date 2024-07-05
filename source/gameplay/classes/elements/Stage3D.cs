
namespace FNFGodot.Gameplay.Classes.Elements;
public partial class Stage3D : Node3D
{
	[Export] public float CameraFov = 1;
	[NodePath("Positions")] public Node3D CharPosGroup;
	public override void _Ready() {
		this.OnReady();
	}

	public Vector3 GetCharPosition(string charName)
	{
		Node3D character = CharPosGroup.GetNodeOrNull<Node3D>(charName);
		if (character is null) character = new Node3D();

		return character.Position;
	}
}
