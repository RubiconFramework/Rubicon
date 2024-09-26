using Rubicon.Menus;

public partial class SettingsMenu : BaseMenu
{
	[NodePath("Main/Sidebar/AnimationPlayer")] private AnimationPlayer _sidebarAnimationPlayer;
	[NodePath("Main/Sidebar")] private Control _sidebar;

	public override void _Ready()
	{
		this.OnReady();
		base._Ready();

		_sidebar.MouseEntered += () => _sidebarAnimationPlayer.Play("Enter");
		_sidebar.MouseExited += () => _sidebarAnimationPlayer.Play("Leave");
	}
}
