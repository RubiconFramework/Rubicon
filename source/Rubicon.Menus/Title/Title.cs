using Godot;
using Godot.Sharp.Extras;
using Promise.Framework;
using Rubicon.Autoload.API;

namespace Rubicon.Menus.Title;

public partial class Title : Node
{
	[NodePath("TitleGroup/Girlfriend/AnimationPlayer")] private AnimationPlayer Girlfriend;
	[NodePath("TitleGroup/Logo")] private AnimatedSprite2D Logo;
	[NodePath("TitleGroup/TitleEnter")] private AnimatedSprite2D TitleEnter;
	[NodePath("TitleGroup")] private Node2D TitleGroup;
	[NodePath("TextGroup")] private Node2D textGroup;
	[NodePath("NewgroundsSprite")] private Sprite2D NewgroundsSprite;
	[NodePath("Flash/AnimationPlayer")] private AnimationPlayer Flash;
	[NodePath("Camera2D")] private Camera2D Camera;

	private string[] LoadedIntroTexts = { "yoooo swag shit", "ball shit" };
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

	public override void _EnterTree()
	{
		base._EnterTree();
		Conductor.OnBeatHit += OnBeatHit;
		Conductor.OnMeasureHit += OnMeasureHit;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		Conductor.OnBeatHit -= OnBeatHit;
		Conductor.OnMeasureHit -= OnMeasureHit;	
	}

	public override void _Ready()
	{
		base._Ready();
		this.OnReady();

		AudioManager.Play(AudioType.Music, "MainMenuMusic", 1f, true);
		Conductor.Bpm = 180;
		Flash.Play("Flash");

		//LoadedIntroTexts = GetIntroTexts();
		TitleEnter.Play("Press Enter to Begin");
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (!@event.IsActionPressed("menu_accept")) return;
		if (!skippedIntro) SkipIntro();
		else if (!transitioning)
		{
			transitioning = true;
			TitleEnter.Play("ENTER PRESSED");

			AudioManager.Play(AudioType.Sounds, "menus/confirmMenu");
			SceneTreeTimer timer = GetTree().CreateTimer(2.0f);
			timer.Timeout += () => LoadingHandler.ChangeScene("res://source/menus/mainmenu/MainMenu.tscn", true);
		}
	}

	private bool isDancingLeft = true;
	private void OnBeatHit(int beat)
	{
		isDancingLeft = !isDancingLeft;
		Girlfriend.Play(isDancingLeft ? "danceRight" : "danceLeft");
		Logo.Play("BumpIn");
	}

	private void OnMeasureHit(int section) => Camera.Zoom = new(1.03f, 1.03f);

	private void SkipIntro()
	{
		skippedIntro = true;
		//DeleteText();
		TitleGroup.Visible = true;
		NewgroundsSprite.Visible = false;
		Flash.Play("Flash");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		Camera.Zoom = new(Mathf.Lerp(Camera.Zoom.X, 1, Mathf.Clamp(((float)delta * 3), 0f, 1f)),Mathf.Lerp(Camera.Zoom.X, 1, Mathf.Clamp(((float)delta * 3), 0f, 1f)));
	}

	/*protected override void OnBeatHit(int beat)
	{
		//base.OnBeatHit(beat);
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
						if (!parameters[0].StartsWith("LoadedIntroTexts[") || !parameters[0].EndsWith("]"))
							Call(methodName, parameters[0]);
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

		//Main.Instance.SendNotification("Intro Texts file is null. Skipping.", true, NotificationType.Error);
		return null;
	}
	*/
}
