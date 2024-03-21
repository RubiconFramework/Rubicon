using BaseRubicon.Gameplay.Elements.Resources;
using Godot.Collections;
using BPMChangeEvent = BaseRubicon.Gameplay.Elements.Classes.Song.BPMChangeEvent;
using Section = BaseRubicon.Gameplay.Elements.Classes.Song.Section;

namespace BaseRubicon.Backend.Autoload;

[Icon("res://assets/miscicons/autoload.png")]
public partial class Conductor : Node
{
    public float rate = 1.0f;
    public float bpm = 100.0f;
 
    public float crochet;
    public float stepCrochet;
 
    public double position;
 
    public int safeFrames = 10;
    public float safeZoneOffset;
 
    public int curBeat;
    public int curStep;
    public int curSection;
 
    public double curDecBeat;
    public double curDecStep;
    public double curDecSection;

    public Array<BPMChangeEvent> bpmChangeMap = new();

    private event Action<int> BeatHitEvent;
    private event Action<int> StepHitEvent;
    private event Action<int> SectionHitEvent;

    public static Conductor Instance { get; private set; }
    
    public override void _EnterTree()
    {
        base._EnterTree();
        Instance = this;
        BeatHitEvent += OnBeatHit;
        StepHitEvent += OnStepHit;
        SectionHitEvent += OnSectionHit;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Instance = null;
        BeatHitEvent -= OnBeatHit;
        StepHitEvent -= OnStepHit;
        SectionHitEvent -= OnSectionHit;
    }

    public override void _Ready()
    {
        crochet = ((60.0f / bpm) * 1000.0f);
        stepCrochet = crochet / 4.0f;
        safeZoneOffset = (safeFrames / 60.0f) * 1000.0f;
    }

    public void MapBPMChanges(Chart song)
    {
        bpmChangeMap.Clear();
        float curBPM = song.Bpm;
        int totalSteps = 0;
        float totalPos = 0.0f;

        foreach (Section section in song.Sections)
        {
            if (section.ChangeBpm && !section.Bpm.Equals(curBPM))
            {
                curBPM = section.Bpm;
                BPMChangeEvent newEvent = new(totalSteps, totalPos, curBPM);
                bpmChangeMap.Add(newEvent);
            }

            int deltaSteps = section.LengthInSteps;
            totalSteps += deltaSteps;
            totalPos += (((60.0f / curBPM) * 1000.0f) / 4.0f) * deltaSteps;
        }
    }

    public void ChangeBPM(float newBpm)
    {
        bpm = newBpm;
        crochet = ((60.0f / newBpm) * 1000.0f);
        stepCrochet = crochet / 4.0f;
    }

    public override void _Process(double delta)
    {
        int oldStep = curStep;
        int oldBeat = curBeat;
        int oldSection = curSection;

        position += delta;

        BPMChangeEvent lastChange = null;
        foreach (BPMChangeEvent evt in bpmChangeMap)
        {
            if (position >= evt.songTime) lastChange = evt;
            else break;
        }

        if (lastChange != null && !bpm.Equals(lastChange.bpm)) ChangeBPM(lastChange.bpm);
        
        curStep = lastChange != null ? lastChange.stepTime + Mathf.FloorToInt((position - lastChange.songTime) / stepCrochet) : 0;
        curBeat = Mathf.FloorToInt(curStep / 4.0f);
        curSection = Mathf.FloorToInt(curStep / 16.0f);

        curDecStep = lastChange != null ? lastChange.stepTime + ((position - lastChange.songTime) / stepCrochet) : 0;
        curDecBeat = curDecStep / 4.0f;
        curDecSection = curDecStep / 16.0f;

        if (curStep != oldStep && curStep > 0)
        {
            StepHitEvent?.Invoke(curStep);
            OnStepHit(curStep);
        }
        if (curBeat != oldBeat && curBeat > 0)
        {
            BeatHitEvent?.Invoke(curBeat);
            OnBeatHit(curBeat);
        }
        if (curSection != oldSection && curSection > 0)
        {
            SectionHitEvent?.Invoke(curSection);
            OnSectionHit(curSection);
        }
    }

    protected virtual void OnBeatHit(int beat)
    {
        //this thing cant even receive its own signals?? is it five stupid??
    }

    protected virtual void OnStepHit(int step)
    {
        //this thing cant even receive its own signals?? is it five stupid??
    }
    protected virtual void OnSectionHit(int section) 
    { 
        //this thing cant even receive its own signals?? is it five stupid
    }
    
    public bool IsSoundSynced(AudioStreamPlayer sound)
    {
        float msAllowed = (OS.GetName() == "Windows") ? (30 * sound.PitchScale) : (20 * sound.PitchScale);
        float soundTime = sound.GetPlaybackPosition() * 1000.0f;
        return !(Mathf.Abs(soundTime - position) > msAllowed);
    }
}
