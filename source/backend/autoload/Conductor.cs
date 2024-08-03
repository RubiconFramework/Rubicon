namespace Rubicon.backend.autoload;

public partial class Conductor : Node
{
    public static double SongPosition;
    public static double SongDuration = 0;
    public static bool UpdatePosition = false;

    public static float BPM;

    public static float BeatDuration;
    public static float StepDuration;
    public static float SectionDuration;

    public static int CurBeat;
    public static int CurStep;
    public static int CurSection;

    public delegate int StepHit(int step);
    public delegate int BeatHit(int beat);
    public delegate int SectionHit(int section);

    public static event StepHit OnStepHit;
    public static event BeatHit OnBeatHit;
    public static event SectionHit OnSectionHit;

    public static void UpdateBpm(float NewBpm)
    {
        BPM = NewBpm;
        BeatDuration = 60 / BPM * 1000;
        StepDuration = BeatDuration / 4;
        SectionDuration = StepDuration * 16;
    }

    private int PrevStep;

    public override void _Process(double delta)
    {
        PrevStep = CurStep;

        if (UpdatePosition)
        {
            SongPosition += delta * 1000;

            CurStep = (int)(Math.Floor(SongPosition) / StepDuration);
            CurBeat = CurStep / 4;
            CurSection = CurStep / 16;
        }

        if (PrevStep != CurStep) OnStepHit?.Invoke(CurStep);
        if (PrevStep / 4 != CurBeat) OnBeatHit?.Invoke(CurBeat);
        if (PrevStep / 16 != CurSection) OnSectionHit?.Invoke(CurSection);
        
        PrevStep = CurStep;
    }
}