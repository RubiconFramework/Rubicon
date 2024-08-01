using System.Linq;
using Rubicon.backend.autoload;
using Rubicon.Backend.Autoload;
using Rubicon.menus.options.objects.sections;
using RubiconSettings = Rubicon.backend.autoload.RubiconSettings;

namespace Rubicon.menus.options;

public enum OptionsMenuSections
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

    [NodePath("Panel/SectionButtons/MainContainer/Gameplay")] private Button GameplaySectionButton;
    [NodePath("Panel/SectionButtons/MainContainer/Audio")] private Button AudioSectionButton;
    [NodePath("Panel/SectionButtons/MainContainer/Video")] private Button VideoSectionButton;
    [NodePath("Panel/SectionButtons/MainContainer/Misc")] private Button MiscSectionButton;
    [NodePath("Panel/SectionButtons/MainContainer/Keybinds")] private Button KeybindsSectionButton;

    [NodePath("Panel/SectionContainer/Option Description")] public Label OptionDescriptionLabel;
    [NodePath("Panel/SectionContainer/Current Section")] public Label CurrentSectionLabel;
    [NodePath("Panel/SectionContainer/Keybind Label")] public Label KeybindLabel;
    [NodePath("Panel/AnimationPlayer")] public AnimationPlayer AnimPlayer;

    [NodePath("Panel/SectionContainer/ScrollContainers")] private Control SectionContainer;
    
    private Button[] SectionButtons;
    private SettingsSectionBase[] SectionUIs;
    private bool isMenuShown;

    public static OptionsMenu Instance { get; private set; }

    public override void _EnterTree() => Instance = this;

    public override void _ExitTree()
    {
        if (RubiconSettings.Misc.DiscordRichPresence)
            DiscordRichPresence.Client.UpdateState(string.Empty);
    }

    public override void _Ready()
    {
        this.OnReady();
        SectionButtons = new[]
        {
            GameplaySectionButton,
            AudioSectionButton,
            VideoSectionButton,
            MiscSectionButton,
            KeybindsSectionButton
        };

        SectionUIs = SectionContainer.GetChildren().OfType<SettingsSectionBase>().ToArray();
        

        foreach (var button in SectionButtons)
            button.Pressed += () => OnSectionButtonPressed(Array.IndexOf(SectionButtons, button));
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (Input.IsActionJustPressed("menu_return") && isMenuShown)
        {
            isMenuShown = false;
            AnimPlayer.Stop();
            AnimPlayer.Play("Return to Main Menu");

            if (RubiconSettings.Misc.DiscordRichPresence)
                DiscordRichPresence.Client.UpdateState("In Options Menu");   
        }
        else if (Input.IsActionJustPressed("menu_return"))
            LoadingHandler.ChangeScene("res://source/menus/mainmenu/MainMenu.tscn");   
    }

    private void OnSectionButtonPressed(int sectionIndex)
    {
        CurrentSection = (OptionsMenuSections)sectionIndex;
        AnimPlayer.Stop();
        AnimPlayer.Play("Selected Section");

        if (RubiconSettings.Misc.DiscordRichPresence)
            DiscordRichPresence.Client.UpdateState($"Current Section: {CurrentSection}");

        CurrentSectionLabel.Text = CurrentSection.ToString();
        
        foreach (var ui in SectionUIs)
            ui.Visible = false;

        SectionUIs[(int)CurrentSection].Visible = true;
        isMenuShown = true;
    }
}
