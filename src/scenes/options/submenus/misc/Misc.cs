using Godot.Sharp.Extras;
using Rubicon.scenes.options.submenus.misc.enums;
using BaseSubmenu = Rubicon.scenes.options.objects.BaseSubmenu;

namespace Rubicon.scenes.options.submenus.misc;

public partial class Misc : BaseSubmenu
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
        
        RegisterButton(SceneTransitions, _ => Main.RubiconSettings.Misc.SceneTransitions = !Main.RubiconSettings.Misc.SceneTransitions);
        RegisterButton(OptionsMenuAnimations, _ => Main.RubiconSettings.Misc.OptionsMenuAnimations = !Main.RubiconSettings.Misc.OptionsMenuAnimations);
        RegisterButton(DiscordRichPresence, OptionsMenu.Instance.HelperMethods.SetDiscordRPC);
        RegisterOptionButton(GameLanguage, i => Main.RubiconSettings.Misc.Languages = (GameLanguages)i);
        RegisterOptionButton(GameTransitions, i => Main.RubiconSettings.Misc.Transitions = (TransitionType)i);
    }
    
    private void LoadSettings()
    {
        LoadButtonValue(SceneTransitions, Main.RubiconSettings.Misc.SceneTransitions);
        LoadButtonValue(OptionsMenuAnimations, Main.RubiconSettings.Misc.OptionsMenuAnimations);
        LoadButtonValue(DiscordRichPresence, Main.RubiconSettings.Misc.DiscordRichPresence);
        LoadOptionButtonValue(GameLanguage, (int)Main.RubiconSettings.Misc.Languages);
        LoadOptionButtonValue(GameTransitions, (int)Main.RubiconSettings.Misc.Transitions);
    }
}
