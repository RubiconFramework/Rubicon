using Godot;

[Tool]
public partial class FlashInitializer : EditorPlugin
{
	private static Control dockScene;

	public override void _EnterTree()
	{
		dockScene = GD.Load<PackedScene>("res://addons/flashimport/FlashImport.tscn").Instantiate<Control>();
        AddControlToDock(DockSlot.LeftUr, dockScene);
    }

	public override void _ExitTree()
	{
		RemoveControlFromDocks(dockScene);
		dockScene.QueueFree();
	}
}
