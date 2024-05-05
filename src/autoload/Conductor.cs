using Godot.Collections;
using Rubicon.scenes.gameplay.objects.classes;
using Chart = Rubicon.scenes.gameplay.objects.classes.Chart;

namespace Rubicon.autoload;

[Icon("res://assets/miscicons/autoload.png")]
public partial class Conductor : Node
{
    public float rate = 1.0f;
    private float _bpm = 100f;

    public float bpm
    {
        get => _bpm;
        set
        {
            _bpm = value;
            crochet = ((60.0f / value) * 1000.0f);
            stepCrochet = crochet / 4.0f;
        }
    }

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

    public static Conductor ConductorInstance { get; private set; }
    
    public override void _EnterTree()
    {
        base._EnterTree();
        ConductorInstance = this;
        BeatHitEvent += OnBeatHit;
        StepHitEvent += OnStepHit;
        SectionHitEvent += OnSectionHit;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        ConductorInstance = null;
        BeatHitEvent -= OnBeatHit;
        StepHitEvent -= OnStepHit;
        SectionHitEvent -= OnSectionHit;
    }

    public override void _Ready()
    {
        bpm = _bpm;
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

    private BPMChangeEvent lastChange;
    public override void _Process(double delta)
    {
        base._Process(delta);
        
        int oldStep = curStep;
        int oldBeat = curBeat;
        int oldSection = curSection;

        //this is going to be changed later, rn is ugly af
        //CHANGE IT NOW!!!!!!
        position = AudioManager.Instance.music == null ? 0 :AudioManager.Instance.music.GetPlaybackPosition();

        foreach (BPMChangeEvent evt in bpmChangeMap)
        {
            if (position >= evt.songTime) lastChange = evt;
            else break;
        }

        if (lastChange != null && !_bpm.Equals(lastChange.bpm)) _bpm = lastChange.bpm;
        
        updateCurStep();
        updateBeat();
        updateSection();

        if (curStep != oldStep && curStep > 0)
        {
            StepHitEvent?.Invoke(curStep);
            OnStepHit(curStep);
        }
    }

    //the functions here are the math to calculate the current step, beat and section
    private void updateCurStep()
    {
        curDecStep = getBPMFromSeconds((float)position).stepTime + position - getBPMFromSeconds((float)position).songTime / stepCrochet;
        curStep = getBPMFromSeconds((float)position).stepTime + Mathf.FloorToInt(position - getBPMFromSeconds((float)position).songTime / stepCrochet);
    }

    private void updateBeat()
    {
        curBeat = Mathf.FloorToInt(curStep/4);
        curDecBeat = curDecStep/4;
    }
    
    //idk if making the section thingy only update when you are in gameplay
    private void updateSection()
    {
        curSection = Mathf.FloorToInt(curBeat / 4.0f);
        curDecSection = curDecBeat / 4.0f;
    }

    //this leaks memory?...
    //this is in process.
    BPMChangeEvent getBPMFromSeconds(float time)
    {
        BPMChangeEvent changeEvent = new BPMChangeEvent(0, 0.0f, bpm);
        foreach (var t in bpmChangeMap)
        {
            if (time >= t.songTime) changeEvent = t;
        }
        
        return changeEvent;
    }

    //those are the signals
    protected virtual void OnBeatHit(int beat)
    {
        if (beat % 4 == 0)
        {
            SectionHitEvent?.Invoke(curSection);
            OnSectionHit(curSection);
        }
    }

    protected virtual void OnStepHit(int step)
    {
        if (step % 4 == 0)
        {
            BeatHitEvent?.Invoke(curBeat);
            OnBeatHit(curBeat);
        }
    }

    protected virtual void OnSectionHit(int section)
    {
    }
    
    //well... it explains itself
    public bool IsSoundSynced(AudioStreamPlayer sound)
    {
        float msAllowed = (OS.GetName() == "Windows") ? (30 * sound.PitchScale) : (20 * sound.PitchScale);
        float soundTime = sound.GetPlaybackPosition() * 1000.0f;
        return !(Mathf.Abs(soundTime - position) > msAllowed);
    }
}
