using Newtonsoft.Json;
using Rubicon.Backend.Autoload;
using Rubicon.menus.options.objects.sections;

namespace Rubicon.menus.options;
enum OptionsMenuSections
{
	Gameplay,
	Audio,
	Video,
	Misc,
	Keybinds
}

public partial class OptionsMenu : Control
{
	[Export] private OptionsMenuSections CurrentSection = OptionsMenuSections.Gameplay;
	[NodePath("LeftPanel/SectionIndicator/AnimationPlayer/KeybindLabel")] public Label KeybindLabel;

	//Right Panel VBoxes
	[NodePath("RightPanel/UpperVBox/GameplaySection")] private Button GameplaySectionButton;
	[NodePath("RightPanel/UpperVBox/VideoSection")] private Button VideoSectionButton;
	[NodePath("RightPanel/UpperVBox/AudioSection")] private Button AudioSectionButton;
	[NodePath("RightPanel/UpperVBox/MiscSection")] private Button MiscSectionButton;
	[NodePath("RightPanel/UpperVBox/KeybindsSection")] private Button KeybindsSectionButton;
	
	[NodePath("RightPanel/LowerVBox/ImportFromCode")] private Button ImportFromCode;
	[NodePath("RightPanel/LowerVBox/ExportToCode")] private Button ExportToCode;

	//Left Panel Specific
	[NodePath("LeftPanel/DescriptionLabel")] public Label OptionDescriptionLabel;
	[NodePath("LeftPanel/SectionIndicator")] private Label SectionIndicator;
	[NodePath("LeftPanel/SectionIndicator/AnimationPlayer")] public AnimationPlayer OptionsMenuAnimPlayer;
	
	//Left Panel Scroll Containers
	[NodePath("LeftPanel/ScrollContainers/Gameplay")] private ScrollContainer GameplayScrollContainer;
	[NodePath("LeftPanel/ScrollContainers/Video")] private ScrollContainer VideoScrollContainer;
	[NodePath("LeftPanel/ScrollContainers/Audio")] private ScrollContainer AudioScrollContainer;
	[NodePath("LeftPanel/ScrollContainers/Misc")] private ScrollContainer MiscScrollContainer;
	[NodePath("LeftPanel/ScrollContainers/Keybinds")] private ScrollContainer KeybindsScrollContainer;

	public static OptionsMenu Instance { get; private set; }
	public readonly HelperMethods HelperMethods = new();

	public override void _EnterTree() => Instance = this;

	public override void _ExitTree()
	{
		//if (Main.RubiconSettings.Misc.DiscordRichPresence)
			//DiscordRichPresence.Client.UpdateState(string.Empty);
	}
	
	public override void _Ready()
	{
		this.OnReady();
		ChangeSection(CurrentSection);

		GameplaySectionButton.Pressed += () => ChangeSection(OptionsMenuSections.Gameplay);
		VideoSectionButton.Pressed += () => ChangeSection(OptionsMenuSections.Video);
		AudioSectionButton.Pressed += () => ChangeSection(OptionsMenuSections.Audio);
		MiscSectionButton.Pressed += () => ChangeSection(OptionsMenuSections.Misc);
		KeybindsSectionButton.Pressed += () => ChangeSection(OptionsMenuSections.Keybinds);

		ImportFromCode.Pressed += () =>
		{
			try
			{
				RubiconSettings.Instance = JsonConvert.DeserializeObject<RubiconSettings>(HelperMethods.DecompressString(DisplayServer.ClipboardGet()));
				RubiconSettings.Instance.Save();
				GD.Print("Settings imported.");
			}
			catch (Exception e)
			{
				GD.Print($"Failed to import settings: {e.Message}");
			}
		};

		ExportToCode.Pressed += () =>
		{
			try
			{
				DisplayServer.ClipboardSet(HelperMethods.CompressString(JsonConvert.SerializeObject(RubiconSettings.Instance)));
				GD.Print("Settings exported and copied to clipboard.");
			}
			catch (Exception e)
			{
				GD.Print($"Failed to export settings: {e.Message}");
			}
		};
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("menu_return")) GetTree().ChangeSceneToFile("res://source/menus/mainmenu/MainMenu.tscn");

		if (@event is not InputEventKey { Pressed: true } eventKey) return;
		switch (eventKey.KeyLabel)
		{
			case Key.Up:
			case Key.Left:
				ChangeSection(-1);
				break;
			case Key.Down:
			case Key.Right:
				ChangeSection(1);
				break;
		}
	}

	private bool IsAnimationPlaying;
	private void ChangeSection(int direction)
	{
		if (IsAnimationPlaying) return;
		int newSectionIndex = ((int)CurrentSection + direction + 5) % 5;
		CurrentSection = (OptionsMenuSections)newSectionIndex;
    
		if (RubiconSettings.Misc.OptionsMenuAnimations)
		{
			IsAnimationPlaying = true;
			OptionsMenuAnimPlayer.Play("SectionTransition/StartTransition");
			OptionsMenuAnimPlayer.AnimationFinished += AnimFinished;
		}
		else
		{
			UpdateSectionUI();
			SectionIndicator.Text = CurrentSection.ToString();
			IsAnimationPlaying = false;
		}
	}

	private void ChangeSection(OptionsMenuSections menuSections)
	{
		if (IsAnimationPlaying) return;
		CurrentSection = menuSections;

		if (RubiconSettings.Misc.OptionsMenuAnimations)
		{
			IsAnimationPlaying = true;
			OptionsMenuAnimPlayer.Play("SectionTransition/StartTransition");
			OptionsMenuAnimPlayer.AnimationFinished += AnimFinished;
		}
		else
		{
			UpdateSectionUI();
			SectionIndicator.Text = CurrentSection.ToString();
			IsAnimationPlaying = false;
		}
	}
	
	private void AnimFinished(StringName name)
	{
		OptionsMenuAnimPlayer.AnimationFinished -= AnimFinished;
		if (name == "SectionTransition/EndTransition") return;
		IsAnimationPlaying = false;
		OptionsMenuAnimPlayer.Play("SectionTransition/EndTransition");
		UpdateSectionUI();
		SectionIndicator.Text = CurrentSection.ToString();
	}

	private void UpdateSectionUI()
	{
		//if (Main.RubiconSettings.Misc.DiscordRichPresence) DiscordRichPresence.Client.UpdateState($"Current Section: {CurrentSection}");
		
		GameplaySectionButton.Text = "Gameplay";
		VideoSectionButton.Text = "Video";
		AudioSectionButton.Text = "Audio";
		MiscSectionButton.Text = "Miscellaneous";
		KeybindsSectionButton.Text = "Keybinds";

		GameplayScrollContainer.Visible = false;
		VideoScrollContainer.Visible = false;
		AudioScrollContainer.Visible = false;
		MiscScrollContainer.Visible = false;
		KeybindsScrollContainer.Visible = false;

		switch (CurrentSection)
		{
			case OptionsMenuSections.Gameplay:
				GameplaySectionButton.Text += " ↩"; 
				GameplayScrollContainer.Visible = true; 
				break;
			case OptionsMenuSections.Video:
				VideoSectionButton.Text += " ↩"; 
				VideoScrollContainer.Visible = true;
				break;
			case OptionsMenuSections.Audio:
				AudioSectionButton.Text += " ↩"; 
				AudioScrollContainer.Visible = true;
				break;
			case OptionsMenuSections.Misc:
				MiscSectionButton.Text += " ↩";
				MiscScrollContainer.Visible = true;
				break;
			case OptionsMenuSections.Keybinds:
				KeybindsSectionButton.Text += " ↩";
				KeybindsScrollContainer.Visible = true;
				break;
		}
	}
}
