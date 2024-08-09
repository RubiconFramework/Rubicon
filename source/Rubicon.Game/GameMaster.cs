
#define PROMISE
//#define TRANSCENDENCE

#if PROMISE && TRANSCENDENCE
#error You can not both Promise.Framework and Transcendence.Framework running at the same time. Please define only PROMISE or TRANSCENDENCE.
#elif !PROMISE && !TRANSCENDENCE
#error You need either PROMISE (for Promise.Framework) or TRANSCENDENCE (for Transcendence.Framework) defined in order to have the game run successfully.
#endif

using System.Linq;
using Godot;
using Godot.Collections;
using Rubicon.Game.API.Controllers;
using Rubicon.Game.Chart;
using Rubicon.Game.Data;
using Rubicon.Game.UI;
using Rubicon.Game.Utilities;

#if PROMISE
using Promise.Framework;
using Promise.Framework.Chart;
using Promise.Framework.Objects;
using Promise.Framework.UI.Noteskins;
#endif

namespace Rubicon.Game
{
    public partial class GameMaster : Node
    {
        public static GameMaster Instance;
        
        public static bool LoadingFromEditor = true;
        
        [ExportGroup("Debug"), Export] public string SongName;

        [Export] public string Difficulty;

        [Export] public bool DownScroll = false;

#if PROMISE
        [ExportGroup("Promise"), Export] public Array<ChartController> ChartControllers = new Array<ChartController>();
#elif TRANSCENDENCE
        // Transcendence specific exports here
#endif

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

#if PROMISE
            SetupPromise(meta);
#elif TRANSCENDENCE
            SetupTranscendence(meta);
#endif
            // Load coroutines
            CoroutineController.Load(SongName, meta.Stage);
            
            Instrumental.Play(0);
            if (meta.UseVocals)
                Vocals.Play(0);
        }

#if PROMISE
        private void SetupPromise(SongMetadata meta)
        {
            // Necessary for now
            PromiseData.DefaultNoteSkin = GD.Load<NoteSkin>("res://assets/ui/noteskins/funkin/noteskin.tres");
            PromiseData.DefaultChartHud = GD.Load<PackedScene>("res://assets/ui/styles/funklike/ChartHUD.tscn");

            SongDifficulty diff = meta.GetDifficulty(Difficulty);
            using FileAccess chartFile = FileAccess.Open(diff.ChartPath, FileAccess.ModeFlags.Read);
            HoloChart chart = HoloChart.ParseString(chartFile.GetAsText());
            
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
				
                ChartManagers.AddChild(chartCtrl);
                ChartControllers.Add(chartCtrl);

                if (chartCtrl.Visible)
                    visIdx++;
            }
            
            Conductor.Instance.ChartOffset = chart.Offset;
            Conductor.Instance.SetBpms(chart.BpmInfo);
            Conductor.Instance.Start(0);
        }
#elif TRANSCENDENCE
        private void SetupTranscendence(SongMetadata meta)
        {
            
        }
#endif
    }
}