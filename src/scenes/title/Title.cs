using System.Collections.Generic;
using Rubicon.autoload.enums;
using AudioManager = Rubicon.autoload.AudioManager;
using TransitionManager = Rubicon.autoload.TransitionManager;

namespace Rubicon.scenes.title;

public partial class Title : Conductor
{
	[NodePath("TitleGroup/Girlfriend/AnimationPlayer")] private AnimationPlayer Girlfriend;
	[NodePath("TitleGroup/Logo")] private AnimatedSprite2D Logo;
	[NodePath("TitleGroup/TitleEnter")] private AnimatedSprite2D TitleEnter;
	[NodePath("TitleGroup")] private Node2D TitleGroup;
	[NodePath("TextGroup")] private Node2D textGroup;
	[NodePath("NewgroundsSprite")] private Sprite2D NewgroundsSprite;
	[NodePath("Flash/AnimationPlayer")] private AnimationPlayer Flash;
	[NodePath("Camera2D")] private Camera2D camera;

	private string[] LoadedIntroTexts = new[] { "yoooo swag shit", "ball shit" };
	private bool skippedIntro = true;
	private bool transitioning;

	[Export] private Godot.Collections.Dictionary<int, string> beatActions = new()
	{
		{ 1, "AddTextArray:duntine,legntle0" },
		{ 3, "AddText:present" },
		{ 4, "DeleteText" },
		{ 5, "AddTextArray:You should,check out" },
		{ 7, "GoCheckOutGodotPleaseWereDying" },
		{ 8, "DeleteText" },
		{ 9, "AddTextArray:LoadedIntroTexts[0]" },
		{ 11, "AddText:LoadedIntroTexts[1]" },
		{ 12, "DeleteText" },
		{ 13, "AddText:Friday" },
		{ 14, "AddText:Night" },
		{ 15, "AddText:Funkin" },
		{ 16, "SkipIntro" }
	};
	
	public override void _Ready()
	{
		base._Ready();
		this.OnReady();

		AnimationPlayer anim = GetNode<AnimationPlayer>("hi/AnimationPlayer");
		anim.Play("Intro");
		anim.AnimationFinished += _ =>
		{
			Flash.Play("Flash");
			GetNode<ColorRect>("hi").Visible = false;
		};

		LoadedIntroTexts = GetIntroTexts();
		TitleEnter.Play("Press Enter to Begin");
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (@event.IsActionPressed("menu_accept"))
		{
			if (!skippedIntro) SkipIntro();
			else if (!transitioning)
			{
				transitioning = true;
				TitleEnter.Play("ENTER PRESSED");

				AudioManager.Instance.PlayAudio(AudioType.Sounds, "menus/confirmMenu");
				SceneTreeTimer timer = GetTree().CreateTimer(2.0f);
				timer.Timeout += () => TransitionManager.Instance.ChangeScene("res://src/scenes/mainmenu/MainMenu.tscn");
			}
		}
	}

	private bool isDancingLeft = true;
	protected override void OnStepHit(int step)
	{
		base.OnStepHit(step);

		isDancingLeft = !isDancingLeft;
		if (isDancingLeft) Girlfriend.Play("danceRight");
		else Girlfriend.Play("danceLeft");

		Logo.Play("BumpIn");
	}

	protected override void OnBeatHit(int beat)
	{
		base.OnBeatHit(beat);

		/*
		if (beatActions.TryGetValue(beat, out string action))
		{
			string[] parts = action.Split(':');
			string methodName = parts[0];
			string[] parameters = parts.Length > 1 ? parts[1].Split(',') : null;

			if (parameters == null) Call(methodName);
			else
			{
				switch (parameters.Length)
				{
					case 1 when parameters[0].StartsWith("[") && parameters[0].EndsWith("]"):
					{
						string arrayContent = parameters[0].Substring(1, parameters[0].Length - 2);
						string[] arrayItems = arrayContent.Split(',');
						Call(methodName, arrayItems);
						break;
					}
					case 1:
						if (!parameters[0].StartsWith("LoadedIntroTexts[") || !parameters[0].EndsWith("]")) Call(methodName, parameters[0]);
						else
						{
							int index = int.Parse(parameters[0].Substring(16, parameters[0].Length - 17));
							Call(methodName, LoadedIntroTexts[index]);
						}
						break;
					case 2:
						Call(methodName, parameters[0], bool.Parse(parameters[1]));
						break;
				}
			}
		}
		*/
	}

	private void SkipIntro()
	{
		skippedIntro = true;
		DeleteText();
		TitleGroup.Visible = true;
		NewgroundsSprite.Visible = false;
		Flash.Play("Flash");
	}

	private void AddText(string text) => Main.Instance.CreateAlphabet(textGroup, text, true, false, 0);

	private void AddTextArray(string[] textArray)
	{
		foreach (var text in textArray) AddText(text);
	}

	private void DeleteText()
	{
		foreach (Node child in new List<Node>(textGroup.GetChildren()))
		{
			child.QueueFree();
			textGroup.RemoveChild(child);
		}
	}

	private string[] GetIntroTexts()
	{
		using FileAccess introTextFile = FileAccess.Open("res://src/scenes/title/introTexts.txt", FileAccess.ModeFlags.Read);
		if (introTextFile != null)
		{
			string[] textLines = introTextFile.GetAsText().Split('\n');
			int randomIndex = GD.RandRange(0, textLines.Length - 1);
			return textLines[randomIndex].Split("--");   
		}

		Main.Instance.SendNotification("Intro Texts file is null. Skipping.", true, NotificationType.Error);
		return null;
	}
}
