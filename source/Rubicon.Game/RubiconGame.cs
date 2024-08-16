using System.Linq;
using Godot;
using Godot.Collections;
using Promise.Framework;
using Promise.Framework.API;
using Promise.Framework.Chart;
using Promise.Framework.Objects;
using Promise.Framework.UI;
using Promise.Framework.UI.Noteskins;
using Rubicon.API.Coroutines;
using Rubicon.API.Events;
using Rubicon.Data;
using Rubicon.Data.Events;
using Rubicon.Data.Meta;
using Rubicon.Game.API.Coroutines;
using Rubicon.Game.UI;
using Rubicon.Game.Utilities;
using Rubicon.Space2D;

namespace Rubicon.Game;

public partial class RubiconGame : Node
{
    public static RubiconGame Instance;
        
    public static bool LoadingFromEditor = true;
        
    [ExportGroup("Debug"), Export] public string SongName;

    [Export] public string Difficulty;

    [Export] public bool DownScroll = false;

    [Export] public string UiStyle = "funklike";
        
    [ExportGroup("Status"), Export] public Array<ChartController> ChartControllers = new Array<ChartController>();

    [Export] public SongMetadata Metadata;

    [Export] public bool Paused { get; private set; } = false;

    [ExportGroup("Settings"), Export] public int TargetController = 0;

    [ExportGroup("References"), Export] public Control ChartManagers;

    [Export] public AudioStreamPlayer Instrumental;

    [Export] public AudioStreamPlayer Vocals;

    [Export] public UiBounce UI;

    [ExportSubgroup("2D"), Export] public Node2D Space2D;
    
    [Export] public Stage2D Stage2D;

    [Export] public CameraController2D CameraController2D;
    
    //[ExportSubgroup("3D"), Export] public Node2D Space3D;
    
    //[Export] public Stage2D Stage3D;
    
    [ExportSubgroup("Sub Viewports"), Export] public SubViewport Viewport;

    [Export] public SubViewportContainer ViewportContainer;

    [ExportSubgroup("API Controllers"), Export] public CoroutineController CoroutineController;

    [Export] public EventController EventController;
        
    public override void _Ready()
    {
        if (Instance != null)
        {
            QueueFree();
            return;
        }

        Instance = this;
            
        SaveData.Data.DownScroll = DownScroll;
        Input.UseAccumulatedInput = false;
            
        Metadata = ResourceLoader.Load<SongMetadata>($"res://{GameData.AssetsFolder}/songs/{SongName}/meta.tres");
        Instrumental.Stream = AudioStreamUtil.LoadStream($"res://{GameData.AssetsFolder}/songs/{SongName}/Inst", Metadata.AudioExtension);
        if (Metadata.UseVocals)
            Vocals.Stream = AudioStreamUtil.LoadStream($"res://{GameData.AssetsFolder}/songs/{SongName}/Vocals", Metadata.AudioExtension);

        int targetController = Metadata.PlayerChartIndex;
        if (!LoadingFromEditor || LoadingFromEditor && TargetController == -1)
            TargetController = targetController;

        if (!LoadingFromEditor || LoadingFromEditor && UiStyle == "")
            UiStyle = Metadata.UiStyle;
            
        ChartManagers.SetAnchorsPreset(SaveData.Data.DownScroll ? Control.LayoutPreset.CenterBottom : Control.LayoutPreset.CenterTop);
        ChartManagers.Position = new Vector2(ChartManagers.Position.X, SaveData.Data.DownScroll ? 940f : 140f);
            
        // Necessary for now
        PromiseData.DefaultNoteSkin = GD.Load<NoteSkin>($"res://{GameData.AssetsFolder}/ui/noteskins/funkin/noteskin.tres");
        PromiseData.DefaultChartHud = GD.Load<PackedScene>("res://assets/ui/styles/funklike/ChartHUD.tscn");
        
        // Find current ChartHud
        PackedScene packedChartHud = FileAccess.FileExists($"res://{GameData.AssetsFolder}/ui/styles/{UiStyle}/ChartHUD.tscn")
            ? GD.Load<PackedScene>($"res://{GameData.AssetsFolder}/ui/styles/{UiStyle}/ChartHUD.tscn") : null;

        SongDifficulty diff = Metadata.GetDifficulty(Difficulty);
        using FileAccess chartFile = FileAccess.Open(diff.ChartPath, FileAccess.ModeFlags.Read);
        HoloChart chart = HoloChart.ParseString(chartFile.GetAsText());
        
        Conductor.ChartOffset = chart.Offset;
        Conductor.BpmList = chart.BpmInfo;
        
        // Initialize stage and characters
        if (Metadata.Stage != "none")
        {
            if (!Metadata.Enable3D) // 2D
            {
                Viewport.Disable3D = true;

                string stagePath = $"res://{GameData.AssetsFolder}/stages/{Metadata.Stage}/stage2d.tscn";
                PackedScene packedStage = null;
                if (ResourceLoader.Exists(stagePath))
                {
                    packedStage = GD.Load<PackedScene>(stagePath);
                }
                else
                {
                    GD.PrintErr($"Stage {Metadata.Stage} at path {stagePath} does not exist! Defaulting to stage");
                    packedStage = GD.Load<PackedScene>($"res://{GameData.AssetsFolder}/stages/stage/stage2d.tscn");
                }

                Stage2D = packedStage.Instantiate<Stage2D>();
                CameraController2D.Stage = Stage2D;
                CameraController2D.TargetZoom = Stage2D.DefaultZoom;
                Stage2D.CameraController = CameraController2D;

                for (int i = 0; i < Metadata.Characters.Length; i++)
                    Stage2D.CreateCharacterGroup(Metadata.Characters[i].Characters, Metadata.Characters[i].SpawnPointIndex);

                CameraController2D.UseCharacterCameras = !Metadata.UsePresetPositions;
                CameraController2D.FocusOnCameraPoint(Stage2D.MainFocus, true);
                Space2D.AddChild(Stage2D);
            }   
        }
            
        // Initialize Chart Controllers (strum lines, whatever)
        int visIdx = 0;
        int visibleAmt = chart.Charts.Count(x => x.Visible);
        for (int i = 0; i < chart.Charts.Length; i++)
        {
            IndividualChart curChart = chart.Charts[i];

            ChartController chartCtrl = new ChartController();
            chartCtrl.Initialize(curChart.Lanes, curChart, SaveData.Data.BotPlay || i != TargetController, chart.ScrollSpeed, null, packedChartHud?.Instantiate<ChartHud>());
            chartCtrl.ChartHud.SwitchDirection(SaveData.Data.DownScroll);
            chartCtrl.Visible = curChart.Visible;
            chartCtrl.ChartHud.Visible = i == TargetController;
            chartCtrl.Name = "ChartController " + i;
                
            for (int j = 0; j < curChart.Lanes; j++)
            {
                chartCtrl.Lanes[j].ActionName = $"MANIA_{curChart.Lanes}K_{j}";
                chartCtrl.Lanes[j].DirectionAngle =
                    SaveData.Data.DownScroll ? Mathf.DegToRad(270f) : Mathf.DegToRad(90f);
            }

            chartCtrl.Position = new Vector2(visIdx * 720f - (visibleAmt - 1) * 720f / 2f, 0f);;
            chartCtrl.NoteHit += OnNoteHit;
            chartCtrl.NoteMiss += OnNoteMiss;
            chartCtrl.Pressed += OnKeyPressed;
				
            ChartManagers.AddChild(chartCtrl);
            ChartControllers.Add(chartCtrl);

            if (chartCtrl.Visible)
                visIdx++;
        }
        
        Conductor.Start(0);

        // Load API stuff
        CoroutineController.Load(SongName, Metadata.Stage);
        
        // Fuckin camera stuff is a load of bull
        // Event stuff after everythin's been initiailized
        string eventsPath = $"res://{GameData.AssetsFolder}/songs/{SongName}/events.json";
        if (FileAccess.FileExists(eventsPath))
        {
            using FileAccess eventFile = FileAccess.Open(eventsPath, FileAccess.ModeFlags.Read);
            ChartEvents eventData = ChartEvents.ParseString(eventFile.GetAsText()).ConvertData(chart.BpmInfo);
            EventController.Load(eventData);
        }
            
        Instrumental.Play(0);
        if (Metadata.UseVocals)
            Vocals.Play(0);
    }
    
    public void Pause()
    {
        if (Paused)
            return;

        Paused = true;
	        
        ProcessMode = ProcessModeEnum.Disabled;
        Conductor.Pause();
        Instrumental.Stop();
        Vocals.Stop();
    }

    public void Resume()
    {
        if (!Paused)
            return;

        Paused = false;

        ProcessMode = ProcessModeEnum.Inherit;
        Instrumental.Play((float)Conductor.RawTime);
        Vocals.Play((float)Conductor.RawTime);
        Conductor.Play();
    }

    private void OnNoteHit(ChartController chartCtrl, NoteData noteData, int hitType, double distanceFromTime, bool held, NoteEventResult result)
    {
        int controlIndex = ChartControllers.IndexOf(chartCtrl);
        if (!result.ProcessFlags.HasFlag(NoteEventProcessFlags.Animation))
        {
            if (!Metadata.Enable3D && Stage2D != null)
            {
                Stage2D.CharacterGroups[controlIndex].PlayNoteAnim(noteData.Lane, false);
                Stage2D.CharacterGroups[controlIndex].Holding = chartCtrl.Lanes.Any(x => x.NoteHeld != null);
            }
        }
        
        if (Metadata.UseVocals)
            Vocals.VolumeDb = Mathf.LinearToDb(1.0f);
    }
    
    public void OnNoteMiss(ChartController chartCtrl, NoteData noteData, double hitMs, NoteEventResult noteEventResult)
    {
        int controlIndex = ChartControllers.IndexOf(chartCtrl);
        if (!noteEventResult.ProcessFlags.HasFlag(NoteEventProcessFlags.Animation))
        {
            if (!Metadata.Enable3D && Stage2D != null)
            {
                Stage2D.CharacterGroups[controlIndex].PlayNoteAnim(noteData.Lane, true);
                Stage2D.CharacterGroups[controlIndex].Holding = chartCtrl.Lanes.Any(x => x.NoteHeld != null);
            }
        }

        if (noteEventResult.Hit != NoteHitType.None && Metadata.UseVocals)
            Vocals.VolumeDb = Mathf.LinearToDb(0.0f);
    }

    public void OnKeyPressed(NoteLaneController noteLaneController)
    {
        int controlIndex = ChartControllers.IndexOf(noteLaneController.ParentController);
        if (!Metadata.Enable3D && Stage2D != null)
            Stage2D.CharacterGroups[controlIndex].LockNote(ChartControllers[controlIndex].Lanes.Any(x => x.Pressed));
    }
}