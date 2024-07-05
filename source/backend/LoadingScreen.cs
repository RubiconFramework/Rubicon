using Rubicon.Backend.Autoload;

namespace Rubicon.Backend;
public partial class LoadingScreen : Node
{
	[NodePath("ProgressBar")] ProgressBar progressBar;
	[NodePath("Sprite2D")] Sprite2D sprite;
	public override void _Ready() => this.OnReady();
	public override void _Process(double delta)
	{
		progressBar.Value = (float)LoadingHandler.LoadingProgress[0];
		sprite.RotationDegrees += (float)delta*100;
	}
}
