using Godot.Sharp.Extras;
using Newtonsoft.Json;
using Rubicon.backend.autoload;
using Rubicon.backend.autoload.debug.ScreenNotifier;
using Rubicon.backend.common.enums;
using Rubicon.scenes.options.elements;
using Rubicon.scenes.options.elements.enums;
using JsonSettingsManager = Rubicon.backend.scripts.JsonSettingsManager;
using TransitionManager = Rubicon.backend.autoload.managers.TransitionManager;

namespace Rubicon.scenes.options;
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
	[NodePath("LeftPanel/DescriptionLabel")] private Label OptionDescriptionLabel;
	[NodePath("LeftPanel/SubmenuIndicator")] private Label SubmenuIndicator;
	[NodePath("LeftPanel/SubmenuIndicator/AnimationPlayer")] public AnimationPlayer SubmenuIndicatorAnimationPlayer;
	
	//Left Panel Scroll Containers
	[NodePath("LeftPanel/ScrollContainers/GameplayScrollContainer")] private ScrollContainer GameplayScrollContainer;
	[NodePath("LeftPanel/ScrollContainers/VideoScrollContainer")] private ScrollContainer VideoScrollContainer;
	[NodePath("LeftPanel/ScrollContainers/AudioScrollContainer")] private ScrollContainer AudioScrollContainer;
	[NodePath("LeftPanel/ScrollContainers/MiscScrollContainer")] private ScrollContainer MiscScrollContainer;
	[NodePath("LeftPanel/ScrollContainers/KeybindsScrollContainer")] private ScrollContainer KeybindsScrollContainer;
	
	//Gameplay Settings [Left Panel Gameplay Scroll Container]
	[NodePath("LeftPanel/ScrollContainers/GameplayScrollContainer/GameplaySubmenuItems/Downscroll")] private CheckBox Downscroll;
	[NodePath("LeftPanel/ScrollContainers/GameplayScrollContainer/GameplaySubmenuItems/ScrollSpeedType")] private OptionButton ScrollSpeedType;
	[NodePath("LeftPanel/ScrollContainers/GameplayScrollContainer/GameplaySubmenuItems/ScrollSpeed")] private Label ScrollSpeed;
	
	//Audio Settings [Left Panel Audio Scroll Container]
	[NodePath("LeftPanel/ScrollContainers/AudioScrollContainer/AudioSubmenuItems/SoundOutputMode")] private OptionButton SoundOutputMode;
	[NodePath("LeftPanel/ScrollContainers/AudioScrollContainer/AudioSubmenuItems/MasterVolume")] private Label MasterVolume;
	[NodePath("LeftPanel/ScrollContainers/AudioScrollContainer/AudioSubmenuItems/MusicVolume")] private Label MusicVolume;
	[NodePath("LeftPanel/ScrollContainers/AudioScrollContainer/AudioSubmenuItems/SFXVolume")] private Label SFXVolume;
	[NodePath("LeftPanel/ScrollContainers/AudioScrollContainer/AudioSubmenuItems/InstVolume")] private Label InstVolume;
	[NodePath("LeftPanel/ScrollContainers/AudioScrollContainer/AudioSubmenuItems/VoiceVolume")] private Label VoiceVolume;
	
	//Video Settings [Left Panel Video Scroll Container]
	[NodePath("LeftPanel/ScrollContainers/VideoScrollContainer/VideoSubmenuItems/MaxFPS")] private Label MaxFPS;
	[NodePath("LeftPanel/ScrollContainers/VideoScrollContainer/VideoSubmenuItems/WindowMode")] private OptionButton WindowMode;
	[NodePath("LeftPanel/ScrollContainers/VideoScrollContainer/VideoSubmenuItems/VSync")] private OptionButton VSync;
	
	//Misc Settings [Left Panel Misc Scroll Container]
	[NodePath("LeftPanel/ScrollContainers/MiscScrollContainer/MiscSubmenuItems/GameLanguage")] private OptionButton GameLanguage;
	[NodePath("LeftPanel/ScrollContainers/MiscScrollContainer/MiscSubmenuItems/GameTransitions")] private OptionButton GameTransitions;
	[NodePath("LeftPanel/ScrollContainers/MiscScrollContainer/MiscSubmenuItems/DiscordRPC")] private CheckBox DiscordRichPresence;
	[NodePath("LeftPanel/ScrollContainers/MiscScrollContainer/MiscSubmenuItems/SceneTransitions")] private CheckBox SceneTransitions;
	[NodePath("LeftPanel/ScrollContainers/MiscScrollContainer/MiscSubmenuItems/OptionsMenuAnimations")] private CheckBox OptionsMenuAnimations;
	
	//Keybind Settings [Left Panel Keybinds Scroll Container]
	[NodePath("LeftPanel/ScrollContainers/KeybindsScrollContainer/KeybindsSubmenuItems/NoteLeft")] private Button NoteLeft;

	public static OptionsMenu Instance { get; private set; }
	public OptionsMenuState OptionsMenuCurrentState = OptionsMenuState.Idle;
	
	private bool IsAnimationPlaying;
	private HelperMethods HelperMethods;

	public override void _EnterTree()
	{
		Instance ??= this;
		HelperMethods ??= new();
	}

	public override void _ExitTree()
	{
		HelperMethods = null;
	}
	
	public override void _Ready()
	{
		this.OnReady();
		ChangeSubmenu(CurrentSubmenu);
		LoadSettings();
		
		GameplaySubmenuButton.Pressed += () => ChangeSubmenu(OptionsMenuSubmenus.Gameplay);
		VideoSubmenuButton.Pressed += () => ChangeSubmenu(OptionsMenuSubmenus.Video);
		AudioSubmenuButton.Pressed += () => ChangeSubmenu(OptionsMenuSubmenus.Audio);
		MiscSubmenuButton.Pressed += () => ChangeSubmenu(OptionsMenuSubmenus.Miscellaneous);
		KeybindsSubmenuButton.Pressed += () => ChangeSubmenu(OptionsMenuSubmenus.Keybinds);
		
		RegisterButtonEvents(Downscroll, _ => Global.Settings.Gameplay.Downscroll = !Global.Settings.Gameplay.Downscroll);
		RegisterButtonEvents(SceneTransitions, _ => Global.Settings.Misc.SceneTransitions = !Global.Settings.Misc.SceneTransitions);
		RegisterButtonEvents(OptionsMenuAnimations, _ => Global.Settings.Misc.OptionsMenuAnimations = !Global.Settings.Misc.OptionsMenuAnimations);
		RegisterButtonEvents(DiscordRichPresence, HelperMethods.SetDiscordRPC);

		RegisterOptionButtonEvents(ScrollSpeedType, i => Global.Settings.Gameplay.ScrollSpeedType = (ScrollSpeedType)i);
		RegisterOptionButtonEvents(SoundOutputMode, i => Global.Settings.Audio.OutputMode = (OutputMode)i);
		RegisterOptionButtonEvents(GameLanguage, i => Global.Settings.Misc.Languages = (GameLanguages)i);
		RegisterOptionButtonEvents(GameTransitions, i => Global.Settings.Misc.Transitions = (TransitionType)i);
		RegisterOptionButtonEvents(VSync, i => HelperMethods.SetVSync((DisplayServer.VSyncMode)i));
		RegisterOptionButtonEvents(WindowMode, i => HelperMethods.SetWindowMode((DisplayServer.WindowMode)i));

		RegisterSliderEvents(MasterVolume, "Master Volume", v => HelperMethods.SetVolume(VolumeType.MasterVolume, v), true);
		RegisterSliderEvents(MusicVolume, "Music Volume", v => HelperMethods.SetVolume(VolumeType.MusicVolume, v), true);
		RegisterSliderEvents(SFXVolume, "SFX Volume", v => HelperMethods.SetVolume(VolumeType.SFXVolume, v), true);
		RegisterSliderEvents(InstVolume, "Inst Volume", v => HelperMethods.SetVolume(VolumeType.InstVolume, v), true);
		RegisterSliderEvents(VoiceVolume, "Voice Volume", v => HelperMethods.SetVolume(VolumeType.VoiceVolume, v), true);
		RegisterSliderEvents(MaxFPS, "Max FPS", HelperMethods.SetMaxFPS, false);
		RegisterSliderEvents(ScrollSpeed, "Scroll Speed", v => Global.Settings.Gameplay.ScrollSpeed = v, false);
		
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

	public override void _PhysicsProcess(double delta)
	{
		foreach (var button in new[]
		         {
			         Downscroll, ScrollSpeedType,
			         ScrollSpeedType,
			         WindowMode, VSync,
			         GameplaySubmenuButton, VideoSubmenuButton, AudioSubmenuButton, MiscSubmenuButton,
			         ExportToCode, ImportFromCode
		         })
		{
			if (!button.IsHovered()) continue;
			OptionDescriptionLabel.Text = Tr($"%{button.Name}%");
			return;
		}
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

	private void LoadSettings()
	{
		Downscroll.ButtonPressed = Global.Settings.Gameplay.Downscroll;
		SceneTransitions.ButtonPressed = Global.Settings.Misc.SceneTransitions;
		OptionsMenuAnimations.ButtonPressed = Global.Settings.Misc.OptionsMenuAnimations;
		DiscordRichPresence.ButtonPressed = Global.Settings.Misc.DiscordRichPresence;

		WindowMode.Selected = (int)Global.Settings.Video.WindowMode;
		VSync.Selected = (int)Global.Settings.Video.VSync;
		ScrollSpeedType.Selected = (int)Global.Settings.Gameplay.ScrollSpeedType;
		SoundOutputMode.Selected = (int)Global.Settings.Audio.OutputMode;
		GameLanguage.Selected = (int)Global.Settings.Misc.Languages;
		GameTransitions.Selected = (int)Global.Settings.Misc.Transitions;
		
		UpdateSlider(MasterVolume, "Master Volume", Global.Settings.Audio.MasterVolume, true);
		UpdateSlider(MusicVolume, "Music Volume", Global.Settings.Audio.MusicVolume, true);
		UpdateSlider(SFXVolume, "SFX Volume", Global.Settings.Audio.SFXVolume, true);
		UpdateSlider(InstVolume, "Inst Volume", Global.Settings.Audio.InstVolume, true);
		UpdateSlider(VoiceVolume, "Voice Volume", Global.Settings.Audio.VoiceVolume, true);
		UpdateSlider(MaxFPS, "Max FPS", Global.Settings.Video.MaxFPS);
		UpdateSlider(ScrollSpeed, "Scroll Speed", Global.Settings.Gameplay.ScrollSpeed);

		void UpdateSlider(Label parent, string labelText, float v, bool showPercentage = false)
		{
			var slider = parent.GetNode<Slider>("Slider");
			slider.Value = v;
			parent.Text = showPercentage ? $" {labelText}: [{(int)v}%]" : $" {labelText} [{v}]";
		}
	}
	
	private void RegisterButtonEvents(Button button, Action<bool> updateAction)
	{
		button.Pressed += () =>
		{
			updateAction.Invoke(button.ButtonPressed);
			JsonSettingsManager.SaveSettingsToFile(Global.SettingsFilePath);
		};
		button.MouseEntered += () => OptionDescriptionLabel.Text = Tr($"%{button.Name}%");
	}

	private void RegisterOptionButtonEvents(OptionButton optionButton, Action<int> updateAction)
	{
		optionButton.ItemSelected += index =>
		{
			updateAction.Invoke((int)index);
			JsonSettingsManager.SaveSettingsToFile(Global.SettingsFilePath);
		};
		optionButton.MouseEntered += () => OptionDescriptionLabel.Text = Tr($"%{optionButton.Name}%");
	}
		
	private void RegisterSliderEvents(Label label, string labelText, Action<float> updateAction, bool showPercentage)
	{
		label.GetNode<HSlider>("Slider").ValueChanged += v =>
		{
			label.Text = showPercentage ? $" {labelText}: [{(int)v}%]" : $" {labelText} [{(float)v}]";
			updateAction.Invoke((float)v);
			JsonSettingsManager.SaveSettingsToFile(Global.SettingsFilePath);
		};
		label.MouseEntered += () => OptionDescriptionLabel.Text = Tr($"%{label.Name}%");
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
