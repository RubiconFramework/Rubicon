using BaseRubicon.Backend.Autoload;
using BaseRubicon.Scenes.Options.Elements;
using BaseRubicon.Scenes.Options.Submenus.Misc.Enums;

namespace BaseRubicon.Scenes.Options.Submenus.Misc;

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
        
        RegisterButton(SceneTransitions, _ => UserSettings.Misc.SceneTransitions = !UserSettings.Misc.SceneTransitions);
        RegisterButton(OptionsMenuAnimations, _ => UserSettings.Misc.OptionsMenuAnimations = !UserSettings.Misc.OptionsMenuAnimations);
        RegisterButton(DiscordRichPresence, OptionsMenu.HelperMethods.SetDiscordRPC);
        RegisterOptionButton(GameLanguage, i => UserSettings.Misc.Languages = (GameLanguages)i);
        RegisterOptionButton(GameTransitions, i => UserSettings.Misc.Transitions = (TransitionType)i);
    }
    
    private void LoadSettings()
    {
        LoadButtonValue(SceneTransitions, UserSettings.Misc.SceneTransitions);
        LoadButtonValue(OptionsMenuAnimations, UserSettings.Misc.OptionsMenuAnimations);
        LoadButtonValue(DiscordRichPresence, UserSettings.Misc.DiscordRichPresence);
        LoadOptionButtonValue(GameLanguage, (int)UserSettings.Misc.Languages);
        LoadOptionButtonValue(GameTransitions, (int)UserSettings.Misc.Transitions);
    }
}
