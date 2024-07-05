
namespace FNFGodot.Gameplay;
public partial class PauseSubmenu : CanvasLayer
{
    [NodePath("../")] GameplayBase Gameplay;
    public override void _Ready() => this.OnReady();
    public override void _Process(double delta) {
        if(Input.IsActionJustPressed("menu_pause"))
        {
            GetTree().Paused = false;
            QueueFree();
        }
    }
}
