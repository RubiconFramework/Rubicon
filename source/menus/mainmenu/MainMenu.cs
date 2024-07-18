using Rubicon.backend.autoload;
using Rubicon.Backend.Autoload;
using AudioManager = Rubicon.backend.autoload.AudioManager;

namespace Rubicon.menus.mainmenu;

public partial class MainMenu : Node
{
	[NodePath("Camera2D")] private Camera2D camera;
	[NodePath("BG/MenuBGMagenta")] private Sprite2D bgMagenta;
	[NodePath("BG/AnimationPlayer")] private AnimationPlayer bgAnim;
	[NodePath("ButtonFlash")] private AnimationPlayer buttonAnim;
	[NodePath("Buttons")] private Node2D buttonGroup;

	private bool selected;
	private int curSelected;

	public override void _Ready()
	{
		this.OnReady();
		changeSelected();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		float camZoom = Mathf.Lerp(camera.Zoom.X, 1, Mathf.Clamp(((float)delta * 3), 0f, 1f));
		camera.Zoom = new(camZoom, camZoom);
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		
		if (selected) return;
		if (Input.IsActionJustPressed("menu_up"))
			changeSelected(-1);
		
		if (Input.IsActionJustPressed("menu_down")) 
			changeSelected(1);
		
		if (Input.IsActionJustReleased("menu_accept") && curSelected >= 0) 
			selectOption();
		
		if (Input.IsActionJustPressed("menu_return"))
			LoadingHandler.ChangeScene("res://source/menus/title/Title.tscn");
	}

	/*protected override void OnBeatHit(int beat)
	{
		base.OnBeatHit(beat);
		camera.Zoom = new(1.15f, 1.15f);
		//GD.Print("hi");
	}*/
	
	private async void selectOption()
	{
		selected = true;
		AudioManager.Play(AudioType.Sounds,"menus/confirmMenu");
		bgAnim.Play("press");
		
		buttonAnim.RootNode = buttonGroup.GetChild<AnimatedSprite2D>(curSelected).GetPath();
		buttonAnim.Play("flash");

		for (int i = 0; i < buttonGroup.GetChildCount(); i++)
		{
			AnimatedSprite2D buttonSprite = buttonGroup.GetChild<AnimatedSprite2D>(i);
			buttonGroup.GetChild<AnimatedSprite2D>(i).GetChild<Button>(0).Visible = false;

			if (i == curSelected) continue;
			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(buttonSprite, "modulate:a", 0, 1);
		}

		camera.Zoom = new(1.15f, 1.15f);

		await ToSignal(GetTree().CreateTimer(1.5), "timeout");

		string buttonName = buttonGroup.GetChild<AnimatedSprite2D>(curSelected).Name.ToString().ToLower();
		switch (buttonName)
		{
			case "storymode": LoadingHandler.ChangeScene("res://source/menus/debug/SongSelect.tscn"); break;
			case "freeplay": LoadingHandler.ChangeScene("res://source/menus/freeplay/FreeplayMenu.tscn"); break;
			case "options": LoadingHandler.ChangeScene("res://source/menus/options/OptionsMenu.tscn"); break;
			default:
				//Main.Instance.SendNotification($"Scene {buttonName} not found lol", true, NotificationType.Warning);
				LoadingHandler.ChangeScene("res://source/menus/title/Title.tscn");
				break;
		}
	}

	private void changeSelected(int change = 0, bool changeTo = false, bool sound = true)
	{
		curSelected = !changeTo ? Mathf.Wrap(curSelected + change, 0, buttonGroup.GetChildCount()) : change;

		for (int i = 0; i < buttonGroup.GetChildCount(); i++)
		{
			AnimatedSprite2D button = buttonGroup.GetChild<AnimatedSprite2D>(i);
			if (curSelected == i)
			{
				button.Play("selected");
				button.GetNode<Button>("Button").Scale = new(1.275f, 1.275f);
			}
			else
			{
				button.Play("idle");
				button.GetNode<Button>("Button").Scale = new(1f, 1f);
			}
		}

		if (sound) AudioManager.Play(AudioType.Sounds, "menus/scrollMenu");
	}

	private void FocusButton(int focused) => changeSelected(focused, true); 
	private void UnfocusButton() => changeSelected(-1, true, false);
}
