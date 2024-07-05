    using System.Collections.Generic;

namespace Rubicon.Backend.Classes;

[Serializable]
public partial class Chart : Resource
{
    /*  This file has all the processed classes needed to handle charts.
        They're processed on ChartHandler.   */

    public string SongName = "Test";
    public string NameRaw = "test";
    public float Bpm = 150;
    public List<Section> Sections = new();
    public List<RawNote> RawNotes = new();
    public List<RawEvent> RawEvents = new();
    public int KeyCount = 4;
    public float ScrollSpeed = 1;
    public bool Is3D = false;
    public string Player = "bf";
    public string Opponent = "bf";
    public string Watcher = "bf";
    public string Stage = "stage";
    public string UiStyle = "default";
    public ChartTypeEnum chartType;
}

public enum ChartTypeEnum
{
    Default,
    VSlice
}

// unhandled stuff
// this is briefly explained, all the chart types mashed into a nicely formatted one for later digestion
public class RawNote
{
    public float Time;
    public int Direction;
    public string Type = "default";
    public float Length;
    public bool AltAnim;
    public bool PlayerSection;
}

public class RawEvent
{
    public float Time;
    public string EventName;
    public Dictionary<string,dynamic> Values;
}

//handled stuff
public class Section
{
    public float Bpm;
    public bool ChangeBpm;
    public bool IsPlayer = false;
    public int LengthInSteps = 16;
    public bool AltAnimation = false;
}