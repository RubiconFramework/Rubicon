namespace Rubicon.gameplay.objects.classes.chart.resources;

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


