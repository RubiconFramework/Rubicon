using Rubicon.Backend.Autoload;
using Rubicon.Backend.Classes;

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
        
        SetupButton(SceneTransitions, _ => RubiconSettings.Misc.SceneTransitions = !RubiconSettings.Misc.SceneTransitions, 
            RubiconSettings.Misc.SceneTransitions);
        
        SetupButton(OptionsMenuAnimations, _ => RubiconSettings.Misc.OptionsMenuAnimations = !RubiconSettings.Misc.OptionsMenuAnimations, 
            RubiconSettings.Misc.OptionsMenuAnimations);
        
        SetupButton(DiscordRichPresence, HelperMethods.SetDiscordRPC,
            RubiconSettings.Misc.DiscordRichPresence);
        
        SetupOptionButton(GameLanguage, i => RubiconSettings.Misc.Languages = (GameLanguages)i, 
            (int)RubiconSettings.Misc.Languages);
        
        SetupOptionButton(GameTransitions, i => RubiconSettings.Misc.Transitions = (TransitionType)i, 
            (int)RubiconSettings.Misc.Transitions);
    }
}
