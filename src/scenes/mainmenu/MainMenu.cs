using Rubicon.autoload.enums;
using Rubicon.backend.notification;
using AudioManager = Rubicon.autoload.AudioManager;
using TransitionManager = Rubicon.autoload.TransitionManager;

namespace Rubicon.scenes.mainmenu;

public partial class MainMenu : Conductor
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

		float cameraSpeed = Mathf.Clamp(((float)delta * 3), 0f, 1f);
		float camZoom = Mathf.Lerp(camera.Zoom.X, 1, cameraSpeed);
		camera.Zoom = new(camZoom, camZoom);
		
		if (selected) return;
		if (Input.IsActionJustPressed("menu_up")) changeSelected(-1);
		if (Input.IsActionJustPressed("menu_down")) changeSelected(1);
		if (Input.IsActionJustReleased("menu_accept") && curSelected >= 0) selectOption();
	}
	
	protected override void OnBeatHit(int beat)
	{
		base.OnBeatHit(beat);
		camera.Zoom = new(1.15f, 1.15f);
		//GD.Print("hi");
	}
	
	private async void selectOption()
	{
		selected = true;
		AudioManager.Instance.PlayAudio(AudioType.Sounds,"menus/confirmMenu");
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
			case "storymode": TransitionManager.Instance.ChangeScene("res://src/gameplay/GameplayScene.tscn"); break;
			case "freeplay": TransitionManager.Instance.ChangeScene("res://src/scenes/freeplay/FreeplayMenu.tscn"); break;
			case "options": TransitionManager.Instance.ChangeScene("res://src/scenes/options/OptionsMenu.tscn"); break;
			default:
				Main.Instance.SendNotification($"Scene {buttonName} not found lol", true, NotificationType.Warning);
				TransitionManager.Instance.ChangeScene("res://src/scenes/title/Title.tscn");
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

		if (sound) AudioManager.Instance.PlayAudio(AudioType.Sounds, "menus/scrollMenu");
	}

	private void FocusButton(int focused) => changeSelected(focused, true); 
	private void UnfocusButton() => changeSelected(-1, true, false);
}
