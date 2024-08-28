using System;
using Godot;
using Godot.Sharp.Extras;
using Newtonsoft.Json;
using Rubicon.Data;

namespace Rubicon.Menus.Options.Objects.Sections.misc;

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
        
        SetupButton(SceneTransitions, _ => SaveData.Misc.SceneTransitions = !SaveData.Misc.SceneTransitions, 
            SaveData.Misc.SceneTransitions);
        
        SetupButton(OptionsMenuAnimations, _ => SaveData.Misc.OptionsMenuAnimations = !SaveData.Misc.OptionsMenuAnimations, 
            SaveData.Misc.OptionsMenuAnimations);
        
        SetupButton(DiscordRichPresence, HelperMethods.SetDiscordRPC,
            SaveData.Misc.DiscordRichPresence);
        
        SetupOptionButton(GameLanguage, i => SaveData.Misc.Languages = (GameLanguages)i, 
            (int)SaveData.Misc.Languages);
        
        SetupOptionButton(GameTransitions, i => SaveData.Misc.Transitions = (TransitionType)i, 
            (int)SaveData.Misc.Transitions);

        ImportFromCode.Pressed += ImportPressed;
        ExportToCode.Pressed += ExportPressed;
    }

    private static void ImportPressed()
    {
        try
        {
            SaveData.Instance = JsonConvert.DeserializeObject<SaveData>(HelperMethods.DecompressString(DisplayServer.ClipboardGet()));
            SaveData.Save();
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
            DisplayServer.ClipboardSet(HelperMethods.CompressString(JsonConvert.SerializeObject(SaveData.Instance)));
            GD.Print("Settings exported and copied to clipboard.");
        }
        catch (Exception e)
        {
            GD.Print($"Failed to export settings: {e.Message}");
        }
    }
}
