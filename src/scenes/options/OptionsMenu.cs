using Godot.Sharp.Extras;
using Newtonsoft.Json;
using Rubicon.backend.ui.notification;
using Rubicon.scenes.options.objects;
using Rubicon.scenes.options.objects.enums;
using TransitionManager = Rubicon.autoload.managers.transitionmanager.TransitionManager;

namespace Rubicon.scenes.options;
public partial class OptionsMenu : Control
{
	[Export] private OptionsMenuSubmenus CurrentSubmenu = OptionsMenuSubmenus.Gameplay;
	[NodePath("LeftPanel/SubmenuIndicator/AnimationPlayer/KeybindLabel")] public Label KeybindLabel;

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
	[NodePath("LeftPanel/SubmenuIndicator/AnimationPlayer")] public AnimationPlayer OptionsMenuAnimPlayer;
	
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
		if (Main.RubiconSettings.Misc.DiscordRichPresence) Main.DiscordRpcClient.UpdateState(string.Empty);
	}
	
	public override void _Ready()
	{
		this.OnReady();
		ChangeSubmenu(CurrentSubmenu);
		
		GameplaySubmenuButton.Pressed += () => ChangeSubmenu(OptionsMenuSubmenus.Gameplay);
		VideoSubmenuButton.Pressed += () => ChangeSubmenu(OptionsMenuSubmenus.Video);
		AudioSubmenuButton.Pressed += () => ChangeSubmenu(OptionsMenuSubmenus.Audio);
		MiscSubmenuButton.Pressed += () => ChangeSubmenu(OptionsMenuSubmenus.Misc);
		KeybindsSubmenuButton.Pressed += () => ChangeSubmenu(OptionsMenuSubmenus.Keybinds);

		ImportFromCode.Pressed += () =>
		{
			try
			{
				Main.RubiconSettings = JsonConvert.DeserializeObject<RubiconSettings>(HelperMethods.DecompressString(DisplayServer.ClipboardGet()));
				Main.RubiconSettings.Save();
				Main.Instance.Alert("Settings imported.");
			}
			catch (Exception e)
			{
				Main.Instance.Alert($"Failed to import settings: {e.Message}", true, NotificationType.Error);
			}
		};

		ExportToCode.Pressed += () =>
		{
			try
			{
				DisplayServer.ClipboardSet(HelperMethods.CompressString(JsonConvert.SerializeObject(Main.RubiconSettings)));
				Main.Instance.Alert("Settings exported and copied to clipboard.");
			}
			catch (Exception e)
			{
				Main.Instance.Alert($"Failed to export settings: {e.Message}", true, NotificationType.Error);
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

	private bool IsAnimationPlaying;
	private void ChangeSubmenu(int direction)
	{
		if (IsAnimationPlaying) return;
		int newSubmenuIndex = ((int)CurrentSubmenu + direction + 5) % 5;
		CurrentSubmenu = (OptionsMenuSubmenus)newSubmenuIndex;
    
		if (Main.RubiconSettings.Misc.OptionsMenuAnimations)
		{
			IsAnimationPlaying = true;
			OptionsMenuAnimPlayer.Play("SubmenuTransition/StartTransition");
			OptionsMenuAnimPlayer.AnimationFinished += AnimFinished;
		}
		else
		{
			UpdateSubmenuUI();
			SubmenuIndicator.Text = CurrentSubmenu.ToString();
			IsAnimationPlaying = false;
		}
	}

	private void ChangeSubmenu(OptionsMenuSubmenus menuSubmenus)
	{
		if (IsAnimationPlaying) return;
		CurrentSubmenu = menuSubmenus;

		if (Main.RubiconSettings.Misc.OptionsMenuAnimations)
		{
			IsAnimationPlaying = true;
			OptionsMenuAnimPlayer.Play("SubmenuTransition/StartTransition");
			OptionsMenuAnimPlayer.AnimationFinished += AnimFinished;
		}
		else
		{
			UpdateSubmenuUI();
			SubmenuIndicator.Text = CurrentSubmenu.ToString();
			IsAnimationPlaying = false;
		}
	}
	
	private void AnimFinished(StringName name)
	{
		OptionsMenuAnimPlayer.AnimationFinished -= AnimFinished;
		if (name == "SubmenuTransition/EndTransition") return;
		IsAnimationPlaying = false;
		OptionsMenuAnimPlayer.Play("SubmenuTransition/EndTransition");
		UpdateSubmenuUI();
		SubmenuIndicator.Text = CurrentSubmenu.ToString();
	}

	private void UpdateSubmenuUI()
	{
		if (Main.RubiconSettings.Misc.DiscordRichPresence) Main.DiscordRpcClient.UpdateState($"Current Submenu: {CurrentSubmenu}");
		
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
			case OptionsMenuSubmenus.Misc:
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
