using System.Linq;
using Godot;
using Godot.Collections;
using Promise.Framework;
using Promise.Framework.Chart;
using Promise.Framework.Objects;
using Promise.Framework.UI;
using Promise.Framework.UI.Noteskins;
using Rubicon.Data;
using Rubicon.Data.Meta;
using Rubicon.Game.API.Controllers;
using Rubicon.Game.UI;
using Rubicon.Game.Utilities;

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

    [Export] public bool Paused { get; private set; } = false;

    [ExportGroup("Settings"), Export] public int TargetController = 0;

    [ExportGroup("References"), Export] public Control ChartManagers;

    [Export] public AudioStreamPlayer Instrumental;

    [Export] public AudioStreamPlayer Vocals;

    [Export] public UiBounce UI;

    [ExportSubgroup("API Controllers"), Export] public SongCoroutineController CoroutineController;
        
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
            
        SongMetadata meta = ResourceLoader.Load<SongMetadata>($"res://assets/songs/{SongName}/meta.tres");
        Instrumental.Stream = AudioStreamUtil.LoadStream($"res://assets/songs/{SongName}/Inst", meta.AudioExtension);
        if (meta.UseVocals)
            Vocals.Stream = AudioStreamUtil.LoadStream($"res://assets/songs/{SongName}/Vocals", meta.AudioExtension);
            
        ChartManagers.SetAnchorsPreset(SaveData.Data.DownScroll ? Control.LayoutPreset.CenterBottom : Control.LayoutPreset.CenterTop);
        ChartManagers.Position = new Vector2(ChartManagers.Position.X, SaveData.Data.DownScroll ? 940f : 140f);
            
        // Necessary for now
        PromiseData.DefaultNoteSkin = GD.Load<NoteSkin>("res://assets/ui/noteskins/funkin/noteskin.tres");
        PromiseData.DefaultChartHud = GD.Load<PackedScene>("res://assets/ui/styles/funklike/ChartHUD.tscn");
        
        // Find current ChartHud
        PackedScene packedChartHud = FileAccess.FileExists($"res://assets/ui/styles/{UiStyle}/ChartHUD.tscn")
            ? GD.Load<PackedScene>($"res://assets/ui/styles/{UiStyle}/ChartHUD.tscn") : null;

        SongDifficulty diff = meta.GetDifficulty(Difficulty);
        using FileAccess chartFile = FileAccess.Open(diff.ChartPath, FileAccess.ModeFlags.Read);
        HoloChart chart = HoloChart.ParseString(chartFile.GetAsText());
        
        Conductor.Instance.ChartOffset = chart.Offset;
        Conductor.Instance.SetBpms(chart.BpmInfo);
            
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
				
            ChartManagers.AddChild(chartCtrl);
            ChartControllers.Add(chartCtrl);

            if (chartCtrl.Visible)
                visIdx++;
        }
        
        Conductor.Instance.Start(0);

        // Load coroutines
        CoroutineController.Load(SongName, meta.Stage);
            
        Instrumental.Play(0);
        if (meta.UseVocals)
            Vocals.Play(0);
    }
    
    public void Pause()
    {
        if (Paused)
            return;

        Paused = true;
	        
        ProcessMode = ProcessModeEnum.Disabled;
        Conductor.Instance.Pause();
        Instrumental.Stop();
        Vocals.Stop();
    }

    public void Resume()
    {
        if (!Paused)
            return;

        Paused = false;

        ProcessMode = ProcessModeEnum.Inherit;
        Instrumental.Play((float)Conductor.Instance.RawTime);
        Vocals.Play((float)Conductor.Instance.RawTime);
        Conductor.Instance.Play();
    }
}