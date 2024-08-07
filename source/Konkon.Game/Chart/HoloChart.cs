using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Konkon.Game.Chart
{
    /// <summary>
    /// The general chart format for this engine.
    /// </summary>
    [GlobalClass]
    public partial class HoloChart : RefCounted
    {
        #region Public Variables
        /// <summary>
        /// The number of beats in a measure in this chart.
        /// </summary>
        public int TimeSigNumerator = 4;
        
        /// <summary>
        /// The type of note which equals one beat in this chart.
        /// </summary>
        public int TimeSigDenominator = 4;

        /// <summary>
        /// The index for which chart to select to be the opponent.
        /// </summary>
        public int OpponentChartIndex = 0;

        /// <summary>
        /// The index for which chart to select to be playable.
        /// </summary>
        public int PlayerChartIndex = 1;
        
        /// <summary>
        /// A list of BPM changes.
        /// </summary>
        public BpmInfo[] Bpms = Array.Empty<BpmInfo>();
        
        /// <summary>
        /// The default scroll speed for this chart.
        /// </summary>
        public float ScrollSpeed = 1.6f;
        
        /// <summary>
        /// Whether to enable the countdown or not.
        /// </summary>
        public bool Countdown = true;

        /// <summary>
        /// Turning this on will use the camera positions preset on the stage. If set to false, the camera will follow the individual character camera positions.
        /// </summary>
        public bool UsePresetPositions = false;
        
        /// <summary>
        /// The chart offset.
        /// </summary>
        public float Offset = 0;
        
        /// <summary>
        /// The individual charts (or "strumlines") that each contain its own notes and character names.
        /// </summary>
        public CharacterChart[] Charts = Array.Empty<CharacterChart>();
        
        /// <summary>
        /// The stage to spawn in for this chart.
        /// </summary>
        public string Stage = "stage";
        
        /// <summary>
        /// The UI style to use for this chart.
        /// </summary>
        public string UiStyle = "funkin";
        #endregion
        
        #region Public Methods
        
        #region Parsing and Converting
        /// <summary>
        /// Converts a string in JSON format to a HoloChart instance.
        /// </summary>
        /// <param name="jsonText">The string, in JSON format.</param>
        /// <returns>A HoloChart, assuming things went well.</returns>
        public static HoloChart Parse(string jsonText)
        {
            Godot.Collections.Dictionary parsedDict = Json.ParseString(jsonText).AsGodotDictionary();
            HoloChart chart = new HoloChart();

            // Simple stuff outta the way first
            chart.TimeSigNumerator = parsedDict["timeSigNumerator"].AsInt32();
            chart.TimeSigDenominator = parsedDict["timeSigDenominator"].AsInt32();
            chart.ScrollSpeed = parsedDict["scrollSpeed"].AsSingle();
            chart.Countdown = parsedDict["countdown"].AsBool();
            chart.Offset = parsedDict["offset"].AsSingle();
            chart.Stage = parsedDict["stage"].AsString();
            chart.UiStyle = parsedDict["uiStyle"].AsString();

            // bpms
            Godot.Collections.Array parsedBpms = parsedDict["bpms"].AsGodotArray();
            BpmInfo[] bpms = new BpmInfo[parsedBpms.Count];
            for (int i = 0; i < parsedBpms.Count; i++)
            {
                Godot.Collections.Dictionary parsedBpmInfo = parsedBpms[i].AsGodotDictionary();
                BpmInfo bpm = new BpmInfo();
                bpm.Time = parsedBpmInfo["time"].AsSingle();
                bpm.Bpm = parsedBpmInfo["bpm"].AsSingle();

                bpms[i] = bpm;
            }

            chart.Bpms = bpms;

            // charts
            Godot.Collections.Array parsedCharCharts = parsedDict["characterCharts"].AsGodotArray();
            CharacterChart[] characterCharts = new CharacterChart[parsedCharCharts.Count];
            for (int i = 0; i < parsedCharCharts.Count; i++)
            {
                Godot.Collections.Dictionary curParsedChart = parsedCharCharts[i].AsGodotDictionary();
                CharacterChart curCharChart = new CharacterChart();

                curCharChart.Visible = curParsedChart["visible"].AsBool();
                curCharChart.SpawnPointIndex = curParsedChart["spawnPointIndex"].AsInt32();
                curCharChart.Characters = curParsedChart["characters"].AsStringArray();

                // notes
                Godot.Collections.Array parsedNotes = curParsedChart["notes"].AsGodotArray();
                NoteData[] notes = new NoteData[parsedNotes.Count];
                for (int j = 0; j < parsedNotes.Count; j++)
                {
                    Godot.Collections.Dictionary parsedNote = parsedNotes[j].AsGodotDictionary();
                    NoteData note = new NoteData();

                    note.Lane = parsedNote["lane"].AsInt32();
                    note.Type = parsedNote["type"].AsString();
                    note.Time = parsedNote["time"].AsSingle();
                    note.Length = parsedNote["length"].AsSingle();

                    notes[j] = note;
                }

                curCharChart.Notes = notes;
                characterCharts[i] = curCharChart;
            }

            chart.Charts = characterCharts;

            parsedDict.Dispose();
            return chart.ConvertData();
        }

        /// <summary>
        /// Converts this chart into Dictionary format, which can be saved as a JSON.
        /// </summary>
        /// <returns>Itself, in Dictionary format.</returns>
        public Godot.Collections.Dictionary ToDictionary()
        {
            Godot.Collections.Dictionary serializedChart = new Godot.Collections.Dictionary();

            // Simple stuff
            serializedChart.Add("timeSigNumerator", TimeSigNumerator);
            serializedChart.Add("timeSigDenominator", TimeSigDenominator);
            serializedChart.Add("scrollSpeed", ScrollSpeed);
            serializedChart.Add("countdown", Countdown);
            serializedChart.Add("offset", Offset);
            serializedChart.Add("stage", Stage);
            serializedChart.Add("uiStyle", UiStyle);

            // bpms
            Godot.Collections.Array serializedBpms = new Godot.Collections.Array();
            for (int i = 0; i < Bpms.Length; i++)
            {
                Godot.Collections.Dictionary serializedBpmInfo = new Godot.Collections.Dictionary();
                serializedBpmInfo.Add("time", Bpms[i].Time);
                serializedBpmInfo.Add("bpm", Bpms[i].Bpm);
                serializedBpms.Add(serializedBpmInfo);
            }

            serializedChart.Add("bpms", serializedBpms);

            // charts
            Godot.Collections.Array serializedCharts = new Godot.Collections.Array();
            for (int i = 0; i < Charts.Length; i++)
            {
                Godot.Collections.Dictionary curSeriChart = new Godot.Collections.Dictionary();
                CharacterChart curChart = Charts[i];
                curSeriChart.Add("visible", curChart.Visible);
                curSeriChart.Add("lanes", curChart.Lanes);
                curSeriChart.Add("characters", curChart.Characters);
                curSeriChart.Add("spawnPointIndex", curChart.SpawnPointIndex);

                // notes
                Godot.Collections.Array serializedNotes = new Godot.Collections.Array();
                for (int j = 0; j < curChart.Notes.Length; j++)
                    serializedNotes.Add(curChart.Notes[j].ToDictionary());

                curSeriChart.Add("notes", serializedNotes);
                serializedCharts.Add(curSeriChart);
            }

            serializedChart.Add("characterCharts", serializedCharts);

            return serializedChart;
        }
        #endregion

        /// <summary>
        /// Converts the notes in this chart to millisecond format.
        /// </summary>
        /// <returns>Itself</returns>
        public HoloChart ConvertData()
        {
            for (int i = 1; i < Bpms.Length; i++)
                Bpms[i].MsTime = Bpms[i - 1].MsTime + Util.MeasureToMs(Bpms[i].Time - Bpms[i - 1].Time, Bpms[i - 1].Bpm, TimeSigNumerator);

            for (int i = 0; i < Charts.Length; i++)
                for (int n = 0; n < Charts[i].Notes.Length; n++)
                    Charts[i].Notes[n].ConvertData(Bpms, TimeSigNumerator);

            return this;
        }

        /// <summary>
        /// Sorts the notes properly and attempts to get rid of any duplicate notes and notes inside holds.
        /// </summary>
        public void Format()
        {
            for (int c = 0; c < Charts.Length; c++)
            {
                List<NoteData> notes = new List<NoteData>();

                for (int l = 0; l < Charts[c].Lanes; l++)
                {
                    List<NoteData> lane = Charts[c].Notes.Where(x => x.Lane == l).ToList();
                    lane.Sort((x, y) =>
                    {
                        if (x.Time < y.Time)
                            return -1;
                        if (x.Time > y.Time)
                            return 1;

                        return 0;
                    });

                    for (int i = 0; i < lane.Count - 1; i++)
                    {
                        if (lane[i].Length > 0)
                        {
                            double start = lane[i].Time;
                            double end = lane[i].Time + lane[i].Length;
                            while (i < lane.Count - 1 && lane[i + 1].Time >= start && lane[i + 1].Time < end)
                            {
                                GD.Print(
                                    $"Removed note inside hold note area at {lane[i + 1].Time} in lane {l} ({start}-{end})");
                                lane.RemoveAt(i + 1);
                            }
                        }

                        while (i < lane.Count - 1 && lane[i + 1].Time == lane[i].Time)
                        {
                            GD.Print($"Removed duplicate note at {lane[i + 1].Time} in lane {l}");
                            lane.RemoveAt(i + 1);
                        }
                    }

                    notes.AddRange(lane);
                }

                notes.Sort((x, y) =>
                {
                    if (x.Time < y.Time)
                        return -1;
                    if (x.Time > y.Time)
                        return 1;

                    if (x.Lane < y.Lane)
                        return -1;
                    if (x.Lane > y.Lane)
                        return 1;

                    return 0;
                });

                Charts[c].Notes = notes.ToArray();
            }
        }
        #endregion
    }
}
