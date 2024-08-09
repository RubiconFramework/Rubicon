using System.Linq;
using Godot;
using Godot.Collections;
using Promise.Framework;
using Promise.Framework.Chart;
using Promise.Framework.Objects;
using Promise.Framework.UI.Noteskins;
using Rubicon.Game.Chart;
using Rubicon.Game.Data;
using Rubicon.Game.Utilities;

namespace Rubicon.Game
{
    public partial class GameMaster : Node
    {
        public static GameMaster Instance;
        
        public static bool LoadingFromEditor = true;
        
        [ExportGroup("Debug"), Export] public string SongName;

        [Export] public string Difficulty;

        [Export] public bool DownScroll = false;
        
        [ExportGroup("Status"), Export] public Array<ChartController> ChartControllers = new Array<ChartController>();

        [Export] public bool Paused { get; private set; } = false;

        [ExportGroup("Settings"), Export] public int TargetController = 0;

        [ExportGroup("References"), Export] public Control ChartManagers;

        [Export] public AudioStreamPlayer Instrumental;

        [Export] public AudioStreamPlayer Vocals;
        
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
            SongDifficulty diff = meta.GetDifficulty(Difficulty);
            
            using FileAccess chartFile = FileAccess.Open(diff.ChartPath, FileAccess.ModeFlags.Read);
            HoloChart chart = HoloChart.ParseString(chartFile.GetAsText());

            Instrumental.Stream = AudioStreamUtil.LoadStream($"res://assets/songs/{SongName}/Inst", meta.AudioExtension);
            if (meta.UseVocals)
                Vocals.Stream = AudioStreamUtil.LoadStream($"res://assets/songs/{SongName}/Vocals", meta.AudioExtension);
            
            // Necessary
            PromiseData.DefaultNoteSkin = GD.Load<NoteSkin>("res://assets/ui/noteskins/funkin/noteskin.tres");
            PromiseData.DefaultChartHud = GD.Load<PackedScene>("res://assets/ui/styles/funklike/ChartHUD.tscn");
            
            Control chartMan = ChartManagers;
            chartMan.SetAnchorsPreset(SaveData.Data.DownScroll ? Control.LayoutPreset.CenterBottom : Control.LayoutPreset.CenterTop);
            chartMan.Position = new Vector2(chartMan.Position.X, SaveData.Data.DownScroll ? 940f : 140f);
            
            int visIdx = 0;
            int visibleAmt = chart.Charts.Count(x => x.Visible);
            for (int i = 0; i < chart.Charts.Length; i++)
            {
                IndividualChart curChart = chart.Charts[i];

                ChartController chartCtrl = new ChartController();
                chartCtrl.Initialize(curChart.Lanes, curChart, SaveData.Data.BotPlay || i != TargetController, chart.ScrollSpeed);
                chartCtrl.ChartHud.SwitchDirection(SaveData.Data.DownScroll);
                chartCtrl.Visible = curChart.Visible;
                chartCtrl.Name = "ChartController " + i;
                
                // Lazy :P
                for (int j = 0; j < curChart.Lanes; j++)
                {
                    chartCtrl.Lanes[j].ActionName = $"M_{curChart.Lanes}K_{j}";
                    chartCtrl.Lanes[j].DirectionAngle =
                        SaveData.Data.DownScroll ? Mathf.DegToRad(270f) : Mathf.DegToRad(90f);
                }

                chartCtrl.Position = new Vector2(visIdx * 720f - (visibleAmt - 1) * 720f / 2f, 0f);;
				
                chartMan.AddChild(chartCtrl);
                ChartControllers.Add(chartCtrl);

                if (chartCtrl.Visible)
                    visIdx++;
            }
            
            Instrumental.Play(0);
            if (meta.UseVocals)
                Vocals.Play(0);

            Conductor.Instance.ChartOffset = chart.Offset;
            Conductor.Instance.SetBpms(chart.BpmInfo);
            Conductor.Instance.Start(0);
        }

        public override void _Input(InputEvent @event)
        {
            // ik this is scuffed but just for now
            if (@event is InputEventKey eventKey)
            {
                if (eventKey.Keycode == Key.Enter && eventKey.Pressed && !eventKey.Echo)
                {
                    Paused = !Paused;
                    if (!Paused)
                    {
                        Instrumental.Play((float)(Conductor.Instance.RawTime));

                        if (Vocals.Stream != null)
                            Vocals.Play((float)(Conductor.Instance.RawTime));
                        
                        Conductor.Instance.Play();
                    }
                    else
                    {
                        Instrumental.Stop();

                        if (Vocals.Stream != null)
                            Vocals.Stop();
                        
                        Conductor.Instance.Pause();
                    }
                }
            }
        }
    }
}