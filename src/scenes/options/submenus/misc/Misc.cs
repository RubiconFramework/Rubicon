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
        
        RegisterButton(SceneTransitions, _ => Global.Settings.Misc.SceneTransitions = !Global.Settings.Misc.SceneTransitions);
        RegisterButton(OptionsMenuAnimations, _ => Global.Settings.Misc.OptionsMenuAnimations = !Global.Settings.Misc.OptionsMenuAnimations);
        RegisterButton(DiscordRichPresence, OptionsMenu.Instance.HelperMethods.SetDiscordRPC);
        RegisterOptionButton(GameLanguage, i => Global.Settings.Misc.Languages = (GameLanguages)i);
        RegisterOptionButton(GameTransitions, i => Global.Settings.Misc.Transitions = (TransitionType)i);
    }
    
    private void LoadSettings()
    {
        LoadButtonValue(SceneTransitions, Global.Settings.Misc.SceneTransitions);
        LoadButtonValue(OptionsMenuAnimations, Global.Settings.Misc.OptionsMenuAnimations);
        LoadButtonValue(DiscordRichPresence, Global.Settings.Misc.DiscordRichPresence);
        LoadOptionButtonValue(GameLanguage, (int)Global.Settings.Misc.Languages);
        LoadOptionButtonValue(GameTransitions, (int)Global.Settings.Misc.Transitions);
    }
}
