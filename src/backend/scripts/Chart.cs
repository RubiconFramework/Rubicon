using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;
using FileAccess = Godot.FileAccess;

namespace Rubicon.backend.scripts;

public class Song
{
    public SongData song;
}

public class SongData
{
    public string song {get; set;}
    public int bpm {get; set;}
    public string player1 {get; set;}
    public string player2 {get; set;}
    public string player3 {get; set;}
    public string gfVersion {get; set;}
    public string stage {get; set;}
    [DefaultValue("default")]
    public string uiStyle {get; set;}
    public List<SongSection> notes {get; set;}
    public int keyCount {get; set;}
    public bool is3D {get; set;}
}

public partial class Section : Resource
{
    public float Bpm { get; set; }
    public bool ChangeBpm { get; set; }
    public bool IsPlayer { get; set; }
    public List<SectionNote> SectionNotes { get; set; }
    public int LengthInSteps { get; set; } = 16;
    public bool AltAnimation {get; set;} = false;
}

public class SongSection
{
    public int lengthInSteps {get; set;}
    public List<List<dynamic>> sectionNotes {get; set;}
    public bool mustHitSection {get; set;}
    public bool changeBPM {get; set;}
    public int bpm {get; set;}
    public bool altAnim {get; set;}
}

public partial class SectionNote : Resource
{
    public float Time { get; set; }
    public int Direction { get; set; }
    public string Type { get; set; } = "default";
    public float Length { get; set; }
    public bool AltAnim { get; set; }
    public bool PlayerSection { get; set; }
}

public partial class BPMChangeEvent : Resource
{
    public int stepTime;
    public float songTime;
    public float bpm;

    public BPMChangeEvent(int stepTime, float songTime, float bpm)
    {
        this.stepTime = stepTime;
        this.songTime = songTime;
        this.bpm = bpm;
    }

    public static BPMChangeEvent Create(int stepTime, float songTime, float bpm) =>  new(stepTime, songTime, bpm);
}

[Serializable]
public partial class Chart : Resource
{
    public string SongName { get; set; } = "Test";
    public string NameRaw { get; set; } = "test";
    public float Bpm { get; set; } = 150;
    public List<Section> Sections { get; set; } = new();
    public int KeyCount { get; set; } = 4;
    public float ScrollSpeed { get; set; } = 1;
    public bool Is3D { get; set; }
    public string Player { get; set; } = "bf";
    public string Opponent { get; set; } = "bf";
    public string Spectator { get; set; } = "bf";
    public string Stage { get; set; } = "stage";
    public string UiStyle { get; set; } = "default";

    public static Chart LoadChart(string songName, string difficulty)
    {
        string baseFilePath = $"res://common/songs/{ExternalPaths.FormatToSongPath(songName)}/{difficulty}.json";
        GD.Print($"Loading chart: {baseFilePath}");

        var file = FileAccess.Open(baseFilePath, FileAccess.ModeFlags.Read);
        if (file == null) throw new FileNotFoundException($"Unable to open file: {baseFilePath}");

        string fileContents = file.GetAsText();
        file.Close();

        SongData Data = JsonConvert.DeserializeObject<Song>(fileContents).song;
        Chart chart = new()
        {
            SongName = Data.song,
            NameRaw = songName,
            Bpm = Data.bpm,
            Player = Data.player1,
            Opponent = Data.player2,
            Stage = Data.stage
        };
        
        chart.Spectator = Data.gfVersion ?? Data.player3;
        chart.KeyCount = Data.keyCount > 0 ? Data.keyCount : 4;
        chart.Is3D = Data.is3D;
        chart.UiStyle = Data.uiStyle ?? "default";

        foreach (SongSection Section in Data.notes)
        {
            Section NewSection = new();
            NewSection.Bpm = Section.bpm;
            NewSection.ChangeBpm = Section.changeBPM;
            NewSection.IsPlayer = Section.mustHitSection;
            NewSection.AltAnimation = Section.altAnim;
            NewSection.SectionNotes = new();
            
            foreach (List<dynamic> Note in Section.sectionNotes)
            {
                SectionNote NewNote = new()
                {
                    Time = (float)Note[0],
                    Direction = (int)Note[1],
                    Length = (float)Note[2],
                };

                //dont mind whatever i did here okay thanks
                //this code exists
                if(Note.Count >= 4) NewNote.Type = Note[3] is string ? Note[3] : "default";
                else NewNote.Type = "default";
                if(NewNote.Type == "Alt Animation"){
                    NewNote.Type = "default";
                    NewNote.AltAnim = true;
                }
                NewSection.SectionNotes.Add(NewNote);
            }
            chart.Sections.Add(NewSection);
        }
        return chart;
    }
}
