using System.Collections.Generic;
using System.ComponentModel;

namespace Rubicon.gameplay.elements.classes.song;

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
