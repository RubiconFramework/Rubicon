using System.Collections.Generic;
using System.Linq;
using Godot;
using Promise.Framework.Chart;
using Promise.Framework.Utilities;
using Rubicon.Extras.Data;
using Rubicon.Game.Chart;
using Rubicon.Game.Data;

namespace Rubicon.Extras.Utilities
{
    /// <summary>
    /// Utility to convert other chart formats to HoloChart format.
    /// </summary>
    public static class ChartConverter
    {
        /// <summary>
        /// Converts data from a typical FNF engine's chart JSON
        /// into SongTriplet form, for use with Rubicon.
        /// </summary>
        /// <param name="jsonText">The FNF chart, in JSON form</param>
        /// <returns>The Funkin' chart given, in SongTriplet form.</returns>
        public static SongTriplet FromFunkin(string jsonText)
        {
            HoloChart chart = new HoloChart();

            Godot.Collections.Dictionary swagSong = Json.ParseString(jsonText).AsGodotDictionary()["song"].AsGodotDictionary();

            List<BpmInfo> bpmChanges = new List<BpmInfo>();
            bpmChanges.Add(new BpmInfo { Time = 0, Bpm = (float)swagSong["bpm"].AsDouble() });

            if (swagSong.ContainsKey("speed"))
                chart.ScrollSpeed = (float)swagSong["speed"].AsDouble() / 1.75f;

            List<NoteData> playerNotes = new List<NoteData>();
            List<NoteData> opponentNotes = new List<NoteData>();
            List<NoteData> speakerNotes = new List<NoteData>();
            
            List<EventData> cameraChanges = new List<EventData>();
            
            int lastCamera = 0;
            double measureTime = 0f;
            Godot.Collections.Array sections = swagSong["notes"].AsGodotArray();
            for (int i = 0; i < sections.Count; i++)
            {
                Godot.Collections.Dictionary curSection = sections[i].AsGodotDictionary();
                if (bpmChanges.Where(x => x.Time == i).Count() == 0 && curSection.ContainsKey("changeBPM") && curSection["changeBPM"].AsBool() == true)
                    bpmChanges.Add(new BpmInfo { Time = i, Bpm = (float)curSection["bpm"].AsDouble() });
                    
                double measureBpm = bpmChanges.Last(x => x.Time <= i).Bpm;

                bool playerSection = curSection["mustHitSection"].AsBool();
                int sectionCamera = playerSection ? 1 : 0;
                
                bool gfSection = curSection.ContainsKey("gfSection") ? curSection["gfSection"].AsBool() : false;
                if (gfSection)
                    sectionCamera = 2;
                
                if (lastCamera != sectionCamera)
                    cameraChanges.Add(new EventData { Time = i, Name = "Set Camera Focus", Arguments = new string[] { sectionCamera.ToString() } });
                
                lastCamera = sectionCamera;
                
                Godot.Collections.Array notes = curSection["sectionNotes"].AsGodotArray();
                for (int n = 0; n < notes.Count; n++)
                {
                    Godot.Collections.Array parsedNote = notes[n].AsGodotArray();
                    NoteData note = new NoteData()
                    {
                        Time = ((parsedNote[0].AsDouble() - measureTime) / (60 / measureBpm * 4) / 1000) + i,
                        Lane = parsedNote[1].AsInt32() % 4,
                        Length = parsedNote[2].AsDouble() / (60 / measureBpm * 4) / 1000,
                        Type = parsedNote.Count > 3 ? parsedNote[3].AsString() : "normal"
                    };

                    if (parsedNote[0].AsDouble() < measureTime)
                        GD.Print($"Measure {i}, note {n}, lane {parsedNote[1].AsUInt32()}: time of {parsedNote[0].AsDouble()} exceeds calculated measure start time of {measureTime}! Calculated milliseconds will be {parsedNote[0].AsDouble() - measureTime}, measure {note.MsTime}");

                    uint lane = parsedNote[1].AsUInt32();
                    if (lane <= 3)
                    {
                        if (playerSection)
                        {
                            playerNotes.Add(note);
                        }
                        else
                        {
                            if (gfSection)
                                speakerNotes.Add(note);
                            else
                                opponentNotes.Add(note);
                        }
                    }
                    else if (lane <= 7)
                    {
                        if (playerSection)
                            opponentNotes.Add(note);
                        else
                            playerNotes.Add(note);
                    }
                    else
                        speakerNotes.Add(note);
                }

                measureTime += ConductorUtil.MeasureToMs(1d, measureBpm, 4d);
            }

            chart.BpmInfo = bpmChanges.ToArray();
            chart.Charts = new IndividualChart[]
            {
                new IndividualChart() {
                    Visible = true,
                    Notes = opponentNotes.ToArray(),
                    Lanes = 4
                },
                new IndividualChart() {
                    Visible = true,
                    Notes = playerNotes.ToArray(),
                    Lanes = 4
                },
                new IndividualChart() {
                    Visible = true,
                    Notes = speakerNotes.ToArray(),
                    Lanes = 4
                }
            };
            chart.Format();

            ChartEvents events = new ChartEvents();
            events.Events = cameraChanges.ToArray();

            SongMetadata meta = new SongMetadata();
            meta.OpponentChartIndex = 0;
            meta.PlayerChartIndex = 1;
            meta.SpeakerChartIndex = 2;
            meta.Characters = new CharacterChart[]
            {
                new CharacterChart()
                {
                    Characters = new string[]
                        { swagSong.ContainsKey("player2") ? swagSong["player2"].AsString() : "bf-pixel" },
                    SpawnPointIndex = 0
                },
                new CharacterChart()
                {
                    Characters = new string[]
                        { swagSong.ContainsKey("player1") ? swagSong["player1"].AsString() : "bf" },
                    SpawnPointIndex = 1
                },
                new CharacterChart()
                {
                    Characters = new string[]
                        { swagSong.ContainsKey("gfVersion") ? swagSong["gfVersion"].AsString() : "gf" },
                    SpawnPointIndex = 0
                }
            };

            swagSong.Dispose();

            SongTriplet triplet = new SongTriplet()
            {
                Chart = chart,
                Events = events,
                Metadata = meta
            };

            return triplet;
        }
        
        /// <summary>
        /// Converts data from a StepMania file (SM)
        /// into SongTriplet form, for use with Rubicon.
        /// </summary>
        /// <param name="smText">The StepMania chart, in string form</param>
        /// <param name="specialNoteType">The special note type, should any mines or special notes be detected.</param>
        /// <returns></returns>
        public static SongTriplet FromStepMania(string smText, string specialNoteType = "normal")
        {
            HoloChart convChart = new HoloChart()
            {
                Charts = new IndividualChart[]
                {
                    new IndividualChart() { Visible = true },
                    new IndividualChart() { Visible = true },
                    new IndividualChart() { Visible = false }
                }
            };

            SongMetadata meta = new SongMetadata()
            {
                Characters = new CharacterChart[]
                {
                    new CharacterChart()
                    {
                        Characters = new string[1],
                        SpawnPointIndex = 0
                    },
                    new CharacterChart()
                    {
                        Characters = new string[1],
                        SpawnPointIndex = 1
                    },
                    new CharacterChart()
                    {
                        Characters = new string[1],
                        SpawnPointIndex = 2
                    }
                }
            };
            
            List<NoteData> opponentNotes = new List<NoteData>();
            List<NoteData> playerNotes = new List<NoteData>();
            List<NoteData> speakerNotes = new List<NoteData>();

            List<string[]> measureData = new List<string[]>();

            bool isSingle = false;
            bool isPump = false; // 5-key
            List<string> data = new List<string>();

            string[] smLines = smText.Split("\n");
            for (int i = 0; i < smLines.Length; i++)
            {
                if (!smLines[i].StartsWith("#"))
                    continue;

                string line = smLines[i];
                switch (string.Join("", line[1..].TakeWhile(x => x != ':')))
                {
                    case "BPMS":
                    {
                        List<BpmInfo> bpms = new List<BpmInfo>();
                        string beat = "";
                        string bpm = "";
                        bool readBPM = false;
                        line = line.Substring(6);
                        while (true)
                        {
                            for (int j = 0; j < line.Length; j++)
                            {
                                switch (line[j])
                                {
                                    default:
                                        if (readBPM)
                                            bpm += line[j];
                                        else
                                            beat += line[j];
                                        break;
                                    case '=':
                                        readBPM = true;
                                        break;
                                    case ';':
                                    case ',':
                                        bpms.Add(new BpmInfo { Time = float.Parse(beat) / 4f, Bpm = float.Parse(bpm) });
                                        beat = "";
                                        bpm = "";
                                        readBPM = false;
                                        break;
                                }
                            }

                            if (line.EndsWith(';'))
                                break;

                            i++;
                            line = smLines[i];
                        }

                        convChart.BpmInfo = bpms.ToArray();
                        break;
                    }
                    case "OFFSET":
                    {
                        string offset = "";
                        line = line.Substring(8);
                        while (true)
                        {
                            for (int j = 0; j < line.Length; j++)
                            {
                                switch (line[j])
                                {
                                    default:
                                        offset += line[j];
                                        break;
                                    case ';':
                                        convChart.Offset = double.Parse(offset) * 1000d;
                                        break;
                                }
                            }
                            if (line.EndsWith(';'))
                                break;

                            i++;
                            line = smLines[i];
                        }

                        break;
                    }
                    case "CHARACTERS": //Custom header tag for opponent character
                    {
                        int index = 0;
                        string character = "";
                        line = line.Substring(12);
                        while (true)
                        {
                            for (int j = 0; j < line.Length; j++)
                            {
                                switch (line[j])
                                {
                                    default:
                                        character += line[j];
                                        break;
                                    case ';':
                                    case ',':
                                        meta.Characters[index].Characters[0] = character;
                                        character = "";
                                        index++;
                                        break;
                                }
                            }
                            if (line.EndsWith(';'))
                                break;
                            
                            i++;
                            line = smLines[i];
                        }
                        break;
                    }
                    case "STAGE":
                    {
                        string stage = "";
                        line = line.Substring(7);
                        while (true)
                        {
                            for (int j = 0; j < line.Length; j++)
                            {
                                switch (line[j])
                                {
                                    default:
                                        stage += line[j];
                                        break;
                                    case ';':
                                        meta.Stage = stage;
                                        break;
                                }
                            }
                            if (line.EndsWith(';'))
                                break;

                            i++;
                            line = smLines[i];
                        }
                        break;
                    }
                    case "NOTES":
                    {
                        i++;
                        line = smLines[i];
                        
                        string ct = string.Join("", line.TrimStart(' ').TakeWhile(x => x != ':'));
                        isPump = ct.StartsWith("pump");
                        isSingle = ct.EndsWith("single");
                        
                        i += 4;
                        
                        while (line != ";" && !line.EndsWith(';'))
                        {
                            i++;
                            line = smLines[i];
                                
                            if (line == "," || line == ";")
                            {
                                measureData.Add(data.ToArray());
                                data = new List<string>();
                                continue;
                            }

                            data.Add(line);
                        }
                        break;
                    }
                }
            }
            
            GD.Print($"Measures: {measureData.Count}");

            int laneCount = isPump ? 5 : 4;
            NoteData[] holdNotes = new NoteData[laneCount * 2];
            for (int i = 0; i < measureData.Count; i++)
            {
                string[] measure = measureData[i];
                for (int t = 0; t < measure.Length; t++)
                {
                    string curData = measure[t];
                    for (int n = 0; n < curData.Length; n++)
                    {
                        switch (curData[n])
                        {
                            case '1': // Single
                            {
                                NoteData note = new NoteData()
                                {
                                    Lane = n % laneCount,
                                    Time = i + (t / (double)measure.Length)
                                };
                                if (n < laneCount)
                                    opponentNotes.Add(note);
                                else
                                    playerNotes.Add(note);
                                break;
                            }
                            case 'M': // Mine
                            {
                                NoteData note = new NoteData()
                                {
                                    Lane = n % laneCount,
                                    Time = i + ((float)t / (float)measure.Length),
                                    Type = specialNoteType
                                };
                                if (n < laneCount)
                                    opponentNotes.Add(note);
                                else
                                    playerNotes.Add(note);
                                break;
                            }
                            case '2': // Start of hold
                            {
                                NoteData note = new NoteData()
                                {
                                    Lane = n % laneCount,
                                    Time = i + ((float)t / (float)measure.Length)
                                };
                                holdNotes[n] = note;
                                if (n < laneCount)
                                    opponentNotes.Add(note);
                                else
                                    playerNotes.Add(note);
                                break;
                            }
                            case '4': // Special
                            {
                                NoteData note = new NoteData()
                                {
                                    Lane = n % laneCount,
                                    Time = i + ((float)t / (float)measure.Length),
                                    Type = specialNoteType
                                };
                                holdNotes[n] = note;
                                if (n < laneCount)
                                    opponentNotes.Add(note);
                                else
                                    playerNotes.Add(note);
                                break;
                            }
                            case '3': // End of hold
                            {
                                if (holdNotes[n] == null)
                                    break;
                                holdNotes[n].Length = i + (t / (double)measure.Length) - holdNotes[n].Time;
                                holdNotes[n] = null;
                                break;
                            }
                        }
                    }
                }
            }
            
            convChart.Charts[0].Notes = opponentNotes.ToArray();
            convChart.Charts[1].Notes = playerNotes.ToArray();
            convChart.Charts[2].Notes = speakerNotes.ToArray();

            convChart.Charts[0].Lanes = convChart.Charts[1].Lanes = convChart.Charts[2].Lanes = laneCount;
            
            if (isSingle)
            {
                meta.PlayerChartIndex = 0;
                meta.OpponentChartIndex = meta.SpeakerChartIndex = -1;
                convChart.Charts = convChart.Charts.Take(1).ToArray();
                meta.Characters = meta.Characters.Take(1).ToArray();
            }
            
            convChart.Format();

            SongTriplet triplet = new SongTriplet();
            triplet.Chart = convChart;
            triplet.Metadata = meta;

            return triplet;
        }
    }
}