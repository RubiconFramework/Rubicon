using Newtonsoft.Json;
using Rubicon.backend.autoload;
using RubiconSettings = Rubicon.backend.autoload.RubiconSettings;

namespace Rubicon.menus.options.objects.sections.misc;

public partial class Misc : SettingsSectionBase
{
    [NodePath("Container/GameLanguage")] private OptionButton GameLanguage;
    [NodePath("Container/GameTransitions")] private OptionButton GameTransitions;
    [NodePath("Container/DiscordRPC")] private CheckBox DiscordRichPresence;
    [NodePath("Container/SceneTransitions")] private CheckBox SceneTransitions;
    [NodePath("Container/OptionsMenuAnimations")] private CheckBox OptionsMenuAnimations;
    [NodePath("Container/ImportSettings")] private Button ImportFromCode;
    [NodePath("Container/ExportSettings")] private Button ExportToCode;

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

        ImportFromCode.Pressed += ImportPressed;
        ExportToCode.Pressed += ExportPressed;
    }

    private static void ImportPressed()
    {
        try
        {
            RubiconSettings.Instance = JsonConvert.DeserializeObject<RubiconSettings>(HelperMethods.DecompressString(DisplayServer.ClipboardGet()));
            RubiconSettings.Save();
            GD.Print("Settings imported.");
        }
        catch (Exception e)
        {
            GD.Print($"Failed to import settings: {e.Message}");
        }   
    }

    private static void ExportPressed()
    {
        try
        {
            DisplayServer.ClipboardSet(HelperMethods.CompressString(JsonConvert.SerializeObject(RubiconSettings.Instance)));
            GD.Print("Settings exported and copied to clipboard.");
        }
        catch (Exception e)
        {
            GD.Print($"Failed to export settings: {e.Message}");
        }
    }
}
