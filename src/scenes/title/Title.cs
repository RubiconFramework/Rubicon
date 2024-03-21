using BaseRubicon.Backend.Autoload.Debug.ScreenNotifier;
using BaseRubicon.Backend.Autoload.Managers.AudioManager;
using BaseRubicon.Backend.UI.Alphabet;
using Godot.Collections;
using AudioManager = BaseRubicon.Backend.Autoload.Managers.AudioManager.AudioManager;
using TransitionManager = BaseRubicon.Backend.Autoload.Managers.TransitionManager;

namespace BaseRubicon.Scenes.Title;

public partial class Title : Conductor
{
    [NodePath("TitleGroup/Girlfriend/AnimationPlayer")] private AnimationPlayer Girlfriend;
    [NodePath("TitleGroup/Logo")] private AnimatedSprite2D Logo;
    [NodePath("TitleGroup/TitleEnter")] private AnimatedSprite2D TitleEnter;
    [NodePath("TitleGroup")] private Node2D TitleGroup;
    [NodePath("TextTemplate")] private Alphabet TextTemplate;
    [NodePath("TextGroup")] private Node2D textGroup;
    [NodePath("NewgroundsSprite")] private Sprite2D NewgroundsSprite;
    [NodePath("Flash/AnimationPlayer")] private AnimationPlayer Flash;
    [NodePath("lol")] private VideoStreamPlayer lol;
    
    private string[] LoadedIntroTexts = new[] { "yoooo swag shit", "ball shit" };
    private bool skippedIntro;
    private bool transitioning;

    [Export]
    private Dictionary<int, string> beatActions = new()
    {
        { 1, "AddTextArray:duntine,legntle0" },
        { 3, "AddText:present" },
        { 4, "DeleteText" },
        { 5, "AddTextArray:You should,check out" },
        { 7, "GoCheckOutGodotPleaseWereDying" },
        { 8, "DeleteText" },
        { 9, "AddTextArray:LoadedIntroTexts[0]" },
        { 11, "AddText:LoadedIntroTexts[1]" },
        { 12, "DeleteText" },
        { 13, "AddText:Friday" },
        { 14, "AddText:Night" },
        { 15, "AddText:Funkin" },
        { 16, "SkipIntro" }
    };

    public override void _Ready()
    {
        this.OnReady();
        AudioStreamPlayer Audio = AudioManager.Instance.PlayAudio(AudioType.Music, "jestersPity", 0.5f, true);
        Instance.position = Audio.GetPlaybackPosition();
        
        if (GD.RandRange(1, 500000) == 30000)
        {
            lol.Play();
            SetProcessInput(false);
            Audio.Stop();
            
            lol.Finished += () =>
            {
                SetProcessInput(true);
                TransitionManager.Instance.ChangeScene("res://src/scenes/mainmenu/MainMenu.tscn");
            };
        }
        
        Girlfriend.Play("danceLeft");
        Logo.Play("BumpIn");
        TitleEnter.Play("Press Enter to Begin");
        LoadedIntroTexts = GetIntroTexts();
        Instance.ChangeBPM(100f);
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event.IsActionPressed("ui_accept"))
        {
            if (!skippedIntro) SkipIntro();
            else if (!transitioning)
            {
                Flash.Play("Flash");
                transitioning = true;
                TitleEnter.Play("ENTER PRESSED");

                AudioManager.Instance.PlayAudio(AudioType.Sounds, "menus/confirmMenu");
                SceneTreeTimer timer = GetTree().CreateTimer(2.0f);
                timer.Timeout += () => TransitionManager.Instance.ChangeScene("res://src/scenes/mainmenu/MainMenu.tscn");
            }
        }
    }

    public override void _Process(double delta)
    {
        //int axis = Mathf.RoundToInt(Input.GetActionStrength("ui_left") - Input.GetActionStrength("ui_right"));
         //if (axis != 0) colorSwap.SetShaderParameter("time", colorSwap.GetShaderParameter("time") + (delta * 0.1f) * axis);
    }

    protected override void OnBeatHit(int beat)
    {
        base.OnBeatHit(beat);

        if (beatActions.TryGetValue(beat, out string action))
        {
            string[] parts = action.Split(':');
            string methodName = parts[0];
            string[] parameters = parts.Length > 1 ? parts[1].Split(',') : null;

            if (parameters == null) Call(methodName);
            else
            {
                switch (parameters.Length)
                {
                    case 1 when parameters[0].StartsWith("[") && parameters[0].EndsWith("]"):
                    {
                        string arrayContent = parameters[0].Substring(1, parameters[0].Length - 2);
                        string[] arrayItems = arrayContent.Split(',');
                        Call(methodName, arrayItems);
                        break;
                    }
                    case 1:
                        if (!parameters[0].StartsWith("LoadedIntroTexts[") || !parameters[0].EndsWith("]")) Call(methodName, parameters[0]);
                        else
                        {
                            int index = int.Parse(parameters[0].Substring(16, parameters[0].Length - 17));
                            Call(methodName, LoadedIntroTexts[index]);
                        }
                        break;
                    case 2:
                        Call(methodName, parameters[0], bool.Parse(parameters[1]));
                        break;
                }
            }
        }
    }

    private void SkipIntro()
    {
        skippedIntro = true;
        DeleteText();
        TitleGroup.Visible = true;
        NewgroundsSprite.Visible = false;
        Flash.Play("Flash");
    }

    private void AddText(string text)
    {
        if (TextTemplate.Duplicate() is Alphabet a)
        {
            a.text = text;
            a.ScreenCenter("X");
            a.Position = new(0, textGroup.GetChildCount() * 60);
            a.Visible = true;
            textGroup.AddChild(a);
        }
    }
    
    private void AddTextArray(string[] textArray)
    {
        foreach (var text in textArray) AddText(text);
    }

    private void DeleteText()
    {
        while (textGroup.GetChildCount() > 0 && textGroup.GetChild(0) is Alphabet a)
        {
            a.QueueFree();
            textGroup.RemoveChild(a);
        }
    }

    private string[] GetIntroTexts()
    {
        using FileAccess introTextFile = FileAccess.Open("res://assets/introTexts.txt", FileAccess.ModeFlags.Read);
        if (introTextFile != null)
        {
            string[] textLines = introTextFile.GetAsText().Split('\n');
            int randomIndex = GD.RandRange(0, textLines.Length - 1);
            return textLines[randomIndex].Split("--");   
        }

        ScreenNotifier.Instance.Notify("Intro Texts file is null. Skipping.", true, NotificationType.Error);
        return null;
    }
}
