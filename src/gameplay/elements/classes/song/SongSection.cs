using System.Collections.Generic;

namespace BaseRubicon.Gameplay.Elements.Classes.Song;

public class SongSection
{
    public int lengthInSteps {get; set;}
    public List<List<dynamic>> sectionNotes {get; set;}
    public bool mustHitSection {get; set;}
    public bool changeBPM {get; set;}
    public int bpm {get; set;}
    public bool altAnim {get; set;}
}
