using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Rubicon.backend.paths;
using Rubicon.gameplay.objects.classes.chart.resources;
using FileAccess = Godot.FileAccess;

namespace Rubicon.gameplay.objects.classes.chart;

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
