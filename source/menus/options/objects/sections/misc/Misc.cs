using Rubicon.Backend.Autoload;

namespace Rubicon.menus.options.objects.sections.misc;

public partial class Misc : SettingsSectionBase
{
    [NodePath("Container/GameLanguage")] private OptionButton GameLanguage;
    [NodePath("Container/GameTransitions")] private OptionButton GameTransitions;
    [NodePath("Container/DiscordRPC")] private CheckBox DiscordRichPresence;
    [NodePath("Container/SceneTransitions")] private CheckBox SceneTransitions;
    [NodePath("Container/OptionsMenuAnimations")] private CheckBox OptionsMenuAnimations;
    
    public override void _Ready()
    {
        base._Ready();
        this.OnReady();
        LoadSettings();
        
        RegisterButton(SceneTransitions, _ => RubiconSettings.Misc.SceneTransitions = !RubiconSettings.Misc.SceneTransitions);
        RegisterButton(OptionsMenuAnimations, _ => RubiconSettings.Misc.OptionsMenuAnimations = !RubiconSettings.Misc.OptionsMenuAnimations);
        RegisterButton(DiscordRichPresence, OptionsMenu.Instance.HelperMethods.SetDiscordRPC);
        RegisterOptionButton(GameLanguage, i => RubiconSettings.Misc.Languages = (GameLanguages)i);
        RegisterOptionButton(GameTransitions, i => RubiconSettings.Misc.Transitions = (TransitionType)i);
    }
    
    private void LoadSettings()
    {
        LoadButtonValue(SceneTransitions, RubiconSettings.Misc.SceneTransitions);
        LoadButtonValue(OptionsMenuAnimations, RubiconSettings.Misc.OptionsMenuAnimations);
        LoadButtonValue(DiscordRichPresence, RubiconSettings.Misc.DiscordRichPresence);
        LoadOptionButtonValue(GameLanguage, (int)RubiconSettings.Misc.Languages);
        LoadOptionButtonValue(GameTransitions, (int)RubiconSettings.Misc.Transitions);
    }
}
