using Godot;
using Godot.Sharp.Extras;
using Rubicon.scenes.options.elements;
using Rubicon.scenes.options.submenus.misc.enums;

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
        
        RegisterButton(SceneTransitions, _ => Main.GameSettings.Misc.SceneTransitions = !Main.GameSettings.Misc.SceneTransitions);
        RegisterButton(OptionsMenuAnimations, _ => Main.GameSettings.Misc.OptionsMenuAnimations = !Main.GameSettings.Misc.OptionsMenuAnimations);
        RegisterButton(DiscordRichPresence, OptionsMenu.Instance.HelperMethods.SetDiscordRPC);
        RegisterOptionButton(GameLanguage, i => Main.GameSettings.Misc.Languages = (GameLanguages)i);
        RegisterOptionButton(GameTransitions, i => Main.GameSettings.Misc.Transitions = (TransitionType)i);
    }
    
    private void LoadSettings()
    {
        LoadButtonValue(SceneTransitions, Main.GameSettings.Misc.SceneTransitions);
        LoadButtonValue(OptionsMenuAnimations, Main.GameSettings.Misc.OptionsMenuAnimations);
        LoadButtonValue(DiscordRichPresence, Main.GameSettings.Misc.DiscordRichPresence);
        LoadOptionButtonValue(GameLanguage, (int)Main.GameSettings.Misc.Languages);
        LoadOptionButtonValue(GameTransitions, (int)Main.GameSettings.Misc.Transitions);
    }
}
