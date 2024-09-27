namespace Rubicon.Menus;

public partial class BaseMenu : Node
{
	[ExportGroup("Status")] 
	[Export] public int Selection = 0;
		
	[ExportGroup("Settings")] 
	[Export] public bool AllowScrollWheel = true;
	[Export] public bool AllowEcho = true;
		
	[ExportGroup("References")] 
	[Export] public AudioStream MoveCursor;
	[Export] public AudioStream ConfirmAudio;
	[Export] public AudioStream BackAudio;

	public virtual void OnDownPressed(bool isPressed) {}
	public virtual void OnUpPressed(bool isPressed) {}
	public virtual void OnLeftPressed(bool isPressed) {}
	public virtual void OnRightPressed(bool isPressed) {}
	public virtual void OnConfirmPressed(bool isPressed) {}
	public virtual void OnBackPressed(bool isPressed) {}
	public virtual void OnScroll(byte direction) {}

	public virtual void UpdateSelection() {}

	public override void _Ready() => UpdateSelection();

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (@event.IsAction("MENU_DOWN", AllowEcho))
			OnDownPressed(@event.IsPressed());
		else if (@event.IsAction("MENU_UP", AllowEcho))
			OnUpPressed(@event.IsPressed());
		else if (@event.IsAction("MENU_LEFT", AllowEcho))
			OnLeftPressed(@event.IsPressed());
		else if (@event.IsAction("MENU_RIGHT", AllowEcho))
			OnRightPressed(@event.IsPressed());
		else if (@event.IsAction("MENU_CONFIRM"))
			OnConfirmPressed(@event.IsPressed());
		else if (@event.IsAction("MENU_BACK", AllowEcho))
			OnBackPressed(@event.IsPressed());

		if (!AllowScrollWheel)
			return;

		if (@event is InputEventMouseButton mouseEvent)
		{
			switch (mouseEvent.ButtonIndex)
			{
				case MouseButton.WheelDown:
					OnScroll(0);
					break;
				case MouseButton.WheelUp:
					OnScroll(1);
					break;
			}
		}
	}
}
