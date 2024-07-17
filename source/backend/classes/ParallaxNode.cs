
namespace Rubicon.backend.classes;

[Icon("res://assets/misc/parallax_node.svg")]
[GlobalClass]
public partial class ParallaxNode : Node2D
{
	// C# port of the ParallaxNode addon by Midnight.
	// All credits to them, i just wanted that 100% c# repo

	[Export] private Vector2 ParallaxFactor = new Vector2(1,1);
	[Export] private bool PositionAsOffset;
	[Export] private bool IgnoreCameraChanges = true;

	private Camera2D camera;
	private Vector2 offset = new Vector2(0,0);

	public override void _Ready()
	{
		camera = GetViewport().GetCamera2D();
		if (PositionAsOffset) offset = Position;
	}
		
	public override void _Process(double delta) => CallDeferred("UpdateCamera");

	private void UpdateCamera() 
	{
		if(!IgnoreCameraChanges)
			camera = GetViewport().GetCamera2D();

		if(camera != null) {
			Position = new Vector2(0,0);
		}

		if(IsInsideTree())
			Position = offset + (camera.GetScreenCenterPosition() -
			                     (GetViewportRect().Size / 2)) * (Vector2.One - ParallaxFactor);
	}
}