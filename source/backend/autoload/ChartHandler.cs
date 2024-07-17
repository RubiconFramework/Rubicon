using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Rubicon.backend.classes;
using FileAccess = Godot.FileAccess;
using static Rubicon.backend.classes.chartType.DefaultChartData;
using static Rubicon.backend.classes.chartType.VSliceChartData;
using Chart = Rubicon.backend.classes.Chart;

namespace Rubicon.Backend.Autoload;
public partial class ChartHandler : Node
{
    public static Chart CurrentChart;
    public static string CurrentDifficulty = "normal";
    
    public static void NewChart(string songName, string difficulty)
    {
        CurrentDifficulty = difficulty;
        CurrentChart = LoadChart(songName);
    }

	public static Chart LoadChart(string songName)
    {
        string basePath = $"res://assets/songs/{songName.ToLower()}/data/";
        string chartPath = $"{basePath+CurrentDifficulty.ToLower()}.json";

        ChartTypeEnum chartType = ChartTypeEnum.Default;
        if(!ResourceLoader.Exists(chartPath) && ResourceLoader.Exists($"{basePath}chart.json")){
            chartType = ChartTypeEnum.VSlice;
            chartPath = $"{basePath}chart.json";
        }
        
        GD.Print($"Attempting to load {chartType} chart: {chartPath}");

        var chartFile = FileAccess.Open(chartPath, FileAccess.ModeFlags.Read);
        if (chartFile == null) 
        {
            GD.PrintErr($"Unable to load chart: {chartPath}");
            return null;
        }
        string chartString = chartFile.GetAsText();
        chartFile.Close();

        string metadataString = "";
        if(chartType == ChartTypeEnum.VSlice)
        {
            var metadataFile = FileAccess.Open($"{basePath}metadata.json", FileAccess.ModeFlags.Read);
            metadataString = metadataFile.GetAsText();
        }


        RawSongData defaultChart = JsonConvert.DeserializeObject<RawSong>(chartString).song;
        RawVSliceSong vsliceChart = new();
        SongMetadata metadata = new();
        if(chartType == ChartTypeEnum.VSlice) {
            vsliceChart = JsonConvert.DeserializeObject<RawVSliceSong>(chartString);
            metadata = JsonConvert.DeserializeObject<SongMetadata>(metadataString);
        }

        return chartType == ChartTypeEnum.Default ? GetDefaultChart(songName,defaultChart) : GetVSliceChart(songName, vsliceChart, metadata);
    }

    private static Chart GetDefaultChart(string rawName, RawSongData Data)
    {
        //basics chart convertion
        Chart chart = new()
        {
            SongName = Data.song,
            NameRaw = rawName,
            Bpm = (float)Data.bpm,
            ScrollSpeed = (float)Data.speed,
            Player = Data.player1,
            Opponent = Data.player2,
            Stage = Data.stage,
            chartType = ChartTypeEnum.Default
        };
        
        chart.Watcher = Data.gfVersion ?? Data.player3 ?? Data.gf ?? Data.watcher ?? Data.spectator;
        chart.KeyCount = Data.keyCount > 0 ? Data.keyCount : 4;
        chart.Is3D = Data.is3D;
        chart.UiStyle = Data.uiStyle ?? "default";

        chart.RawNotes = new();

        foreach (RawSection Section in Data.notes)
        {
            // cooking sections (get it? cause they're RawSections and im turning them into Sections :))
            Section NewSection = new() {
                Bpm = (float)Section.bpm,
                ChangeBpm = Section.changeBPM,
                IsPlayer = Section.mustHitSection,
                AltAnimation = Section.altAnim
            };
            
            foreach (List<dynamic> Note in Section.sectionNotes)
            {
                // note conversion from section notes to raw notes
                RawNote NewNote = new()
                {
                    Time = (float)Note[0],
                    Direction = (int)Note[1],
                    Length = (float)Note[2],
                };

                NewNote.PlayerSection = Note[1] >= chart.KeyCount;
                if(Section.mustHitSection) NewNote.PlayerSection = !NewNote.PlayerSection;

                // this chart system deorganized as FUCK bruh
                if(Note.Count >= 4) NewNote.Type = Note[3] is string ? Note[3] : "default";
                else NewNote.Type = "default";
                if(NewNote.Type == "Alt Animation"){
                    NewNote.Type = "default";
                    NewNote.AltAnim = true;
                }
                chart.RawNotes.Add(NewNote);
            }
            chart.Sections.Add(NewSection);
        }
        GD.Print("Succesfully loaded chart!");
        return chart;
    }

    private static Chart GetVSliceChart(string rawName, RawVSliceSong chartData, SongMetadata metadata)
    {
        Chart chart = new()
        {
            SongName = metadata.songName,
            NameRaw = rawName,
            Bpm = metadata.timeChanges[0].bpm,
            Stage = metadata.playData.stage,
            Player = metadata.playData.characters.player,
            Opponent = metadata.playData.characters.opponent,
            Watcher = metadata.playData.characters.girlfriend,
            ScrollSpeed = chartData.scrollSpeed[CurrentDifficulty.ToCamelCase()],
            chartType = ChartTypeEnum.VSlice
        };
        
        //WDYM SECTIONS DONT EXIST HERE???
        // ^ it has grown on me now tbh
        chart.Sections = new();
        foreach (RawNote newNote in chartData.notes[CurrentDifficulty.ToCamelCase()].Select(rawNote => new RawNote()
                 {
                     Time = rawNote.t,
                     Length = rawNote.l,
                     Direction = rawNote.d,
                     Type = rawNote.k ?? "default"
                 }))
        {
            newNote.PlayerSection = newNote.Direction < chart.KeyCount ? true : false;

            if(newNote.Type == "Alt Animation"){
                newNote.Type = "default";
                newNote.AltAnim = true;
            }

            chart.RawNotes.Add(newNote);
        }

        /*foreach (RawVSliceEvent rawEvent in chartData.events)
        {
            RawEvent newEvent = new()
            {
                Time = rawEvent.t,
                EventName = rawEvent.e,
                Values = rawEvent.v
            };
        }*/

        return chart;
    }
}
