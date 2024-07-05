using Rubicon.Backend.Autoload;

public partial class Start : Node
{
	public override void _Ready()
	{
		SceneTreeTimer timer = GetTree().CreateTimer(0.5);
		timer.Timeout += () => LoadingHandler.ChangeScene("res://source/gameplay/Gameplay2D.tscn");
	}
}
