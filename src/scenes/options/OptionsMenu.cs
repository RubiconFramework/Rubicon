using BaseRubicon.Backend.Autoload;
using BaseRubicon.Backend.Autoload.Debug.ScreenNotifier;
using BaseRubicon.Scenes.Options.Elements;
using BaseRubicon.Scenes.Options.Elements.Enums;
using Newtonsoft.Json;
using JsonSettingsManager = BaseRubicon.Backend.Scripts.JsonSettingsManager;
using TransitionManager = BaseRubicon.Backend.Autoload.Managers.TransitionManager;

namespace BaseRubicon.Scenes.Options;
public partial class OptionsMenu : Control
{
	[Export] private OptionsMenuSubmenus CurrentSubmenu = OptionsMenuSubmenus.Gameplay;

	//Right Panel VBoxes
	[NodePath("RightPanel/UpperVBox/GameplaySubmenu")] private Button GameplaySubmenuButton;
	[NodePath("RightPanel/UpperVBox/VideoSubmenu")] private Button VideoSubmenuButton;
	[NodePath("RightPanel/UpperVBox/AudioSubmenu")] private Button AudioSubmenuButton;
	[NodePath("RightPanel/UpperVBox/MiscSubmenu")] private Button MiscSubmenuButton;
	[NodePath("RightPanel/UpperVBox/KeybindsSubmenu")] private Button KeybindsSubmenuButton;
	[NodePath("RightPanel/LowerVBox/ImportFromCode")] private Button ImportFromCode;
	[NodePath("RightPanel/LowerVBox/ExportToCode")] private Button ExportToCode;

	//Left Panel Specific
	[NodePath("LeftPanel/DescriptionLabel")] public Label OptionDescriptionLabel;
	[NodePath("LeftPanel/SubmenuIndicator")] private Label SubmenuIndicator;
	[NodePath("LeftPanel/SubmenuIndicator/AnimationPlayer")] public AnimationPlayer SubmenuIndicatorAnimationPlayer;
	
	//Left Panel Scroll Containers
	[NodePath("LeftPanel/ScrollContainers/Gameplay")] private ScrollContainer GameplayScrollContainer;
	[NodePath("LeftPanel/ScrollContainers/Video")] private ScrollContainer VideoScrollContainer;
	[NodePath("LeftPanel/ScrollContainers/Audio")] private ScrollContainer AudioScrollContainer;
	[NodePath("LeftPanel/ScrollContainers/Misc")] private ScrollContainer MiscScrollContainer;
	[NodePath("LeftPanel/ScrollContainers/Keybinds")] private ScrollContainer KeybindsScrollContainer;

	public static OptionsMenu Instance { get; private set; }
	public OptionsMenuState OptionsMenuCurrentState = OptionsMenuState.Idle;
	
	private bool IsAnimationPlaying;
	public HelperMethods HelperMethods;

	public override void _EnterTree()
	{
		Instance ??= this;
		HelperMethods ??= new();
	}

	public override void _ExitTree()
	{
		if (Global.Settings.Misc.DiscordRichPresence) Global.DiscordRpcClient.UpdateState(string.Empty);
		HelperMethods = null;
	}
	
	public override void _Ready()
	{
		this.OnReady();
		ChangeSubmenu(CurrentSubmenu);
		
		GameplaySubmenuButton.Pressed += () => ChangeSubmenu(OptionsMenuSubmenus.Gameplay);
		VideoSubmenuButton.Pressed += () => ChangeSubmenu(OptionsMenuSubmenus.Video);
		AudioSubmenuButton.Pressed += () => ChangeSubmenu(OptionsMenuSubmenus.Audio);
		MiscSubmenuButton.Pressed += () => ChangeSubmenu(OptionsMenuSubmenus.Miscellaneous);
		KeybindsSubmenuButton.Pressed += () => ChangeSubmenu(OptionsMenuSubmenus.Keybinds);

		ImportFromCode.Pressed += () =>
		{
			try
			{
				Global.Settings = JsonConvert.DeserializeObject<SettingsData>(Global.DecompressString(DisplayServer.ClipboardGet()));
				JsonSettingsManager.SaveSettingsToFile(Global.SettingsFilePath);
				ScreenNotifier.Instance.Notify("Settings imported.");
			}
			catch (Exception e)
			{
				ScreenNotifier.Instance.Notify($"Failed to import settings: {e.Message}", true, NotificationType.Error);
			}
		};

		ExportToCode.Pressed += () =>
		{
			try
			{
				DisplayServer.ClipboardSet(Global.CompressString(JsonConvert.SerializeObject(Global.Settings)));
				ScreenNotifier.Instance.Notify("Settings exported and copied to clipboard.");
			}
			catch (Exception e)
			{
				ScreenNotifier.Instance.Notify($"Failed to export settings: {e.Message}", true, NotificationType.Error);
			}
		};
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("menu_return")) TransitionManager.Instance.ChangeScene("res://src/scenes/mainmenu/MainMenu.tscn");

		if (@event is not InputEventKey { Pressed: true } eventKey) return;
		switch (eventKey.KeyLabel)
		{
			case Key.Up:
			case Key.Left:
				ChangeSubmenu(-1);
				break;
			case Key.Down:
			case Key.Right:
				ChangeSubmenu(1);
				break;
		}
	}

	private void ChangeSubmenu(int direction)
	{
		if (IsAnimationPlaying) return;
		int newSubmenuIndex = ((int)CurrentSubmenu + direction + 5) % 5;
		CurrentSubmenu = (OptionsMenuSubmenus)newSubmenuIndex;
    
		if (Global.Settings.Misc.OptionsMenuAnimations)
		{
			IsAnimationPlaying = true;
			SubmenuIndicatorAnimationPlayer.Play("SubmenuTransition/StartTransition");
			SubmenuIndicatorAnimationPlayer.AnimationFinished += AnimFinished;
		}
		else
		{
			UpdateSubmenuUI();
			SubmenuIndicator.Text = CurrentSubmenu.ToString();
			IsAnimationPlaying = false;
		}

		void AnimFinished(StringName name)
		{
			SubmenuIndicatorAnimationPlayer.AnimationFinished -= AnimFinished;
			if (name == "SubmenuTransition/EndTransition") return;
			IsAnimationPlaying = false;
			SubmenuIndicatorAnimationPlayer.Play("SubmenuTransition/EndTransition");
			UpdateSubmenuUI();
			SubmenuIndicator.Text = CurrentSubmenu.ToString();
		}
	}

	private void ChangeSubmenu(OptionsMenuSubmenus menuSubmenus)
	{
		if (IsAnimationPlaying) return;
		CurrentSubmenu = menuSubmenus;

		if (Global.Settings.Misc.OptionsMenuAnimations)
		{
			IsAnimationPlaying = true;
			SubmenuIndicatorAnimationPlayer.Play("SubmenuTransition/StartTransition");
			SubmenuIndicatorAnimationPlayer.AnimationFinished += AnimFinished;
		}
		else
		{
			UpdateSubmenuUI();
			SubmenuIndicator.Text = CurrentSubmenu.ToString();
			IsAnimationPlaying = false;
		}

		void AnimFinished(StringName name)
		{
			SubmenuIndicatorAnimationPlayer.AnimationFinished -= AnimFinished;
			if (name == "SubmenuTransition/EndTransition") return;
			IsAnimationPlaying = false;
			SubmenuIndicatorAnimationPlayer.Play("SubmenuTransition/EndTransition");
			UpdateSubmenuUI();
			SubmenuIndicator.Text = CurrentSubmenu.ToString();
		}
	}

	private void UpdateSubmenuUI()
	{
		if (Global.Settings.Misc.DiscordRichPresence) Global.DiscordRpcClient.UpdateState($"Current Submenu: {CurrentSubmenu}");
		
		GameplaySubmenuButton.Text = "Gameplay";
		VideoSubmenuButton.Text = "Video";
		AudioSubmenuButton.Text = "Audio";
		MiscSubmenuButton.Text = "Miscellaneous";
		KeybindsSubmenuButton.Text = "Keybinds";

		GameplayScrollContainer.Visible = false;
		VideoScrollContainer.Visible = false;
		AudioScrollContainer.Visible = false;
		MiscScrollContainer.Visible = false;
		KeybindsScrollContainer.Visible = false;

		switch (CurrentSubmenu)
		{
			case OptionsMenuSubmenus.Gameplay:
				GameplaySubmenuButton.Text += " ↩"; 
				GameplayScrollContainer.Visible = true; 
				break;
			case OptionsMenuSubmenus.Video:
				VideoSubmenuButton.Text += " ↩"; 
				VideoScrollContainer.Visible = true;
				break;
			case OptionsMenuSubmenus.Audio:
				AudioSubmenuButton.Text += " ↩"; 
				AudioScrollContainer.Visible = true;
				break;
			case OptionsMenuSubmenus.Miscellaneous:
				MiscSubmenuButton.Text += " ↩";
				MiscScrollContainer.Visible = true;
				break;
			case OptionsMenuSubmenus.Keybinds:
				KeybindsSubmenuButton.Text += " ↩";
				KeybindsScrollContainer.Visible = true;
				break;
		}
	}
}
