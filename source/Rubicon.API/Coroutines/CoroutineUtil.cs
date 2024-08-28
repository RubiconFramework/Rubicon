using HCoroutines;
using Promise.Framework;

namespace Rubicon.API.Coroutines;

/// <summary>
/// A coroutine that waits until the global Conductor's measure passes the provided measure.
/// </summary>
public class WaitForMeasure : CoroutineBase
{
    protected double Measure;
        
    public WaitForMeasure(double measure)
    {
        Measure = measure;
    }

    public override void OnEnter() 
    {
        CheckTime();
        if (IsAlive)
            ResumeUpdates();
    }

    public override void Update() 
    {
        CheckTime();
    }

    private void CheckTime() 
    {
        if (Conductor.CurrentMeasure >= Measure)
            Kill();
    }
}

/// <summary>
/// A coroutine that waits for a set amount of measures.
/// </summary>
public class WaitMeasures : WaitForMeasure
{
    public WaitMeasures(double measure) : base(measure)
    {
        Measure = Conductor.CurrentMeasure + measure;
    }
}

/// <summary>
/// A coroutine that waits until the global Conductor's beat passes the provided beat.
/// </summary>
public class WaitForBeat : CoroutineBase
{
    protected double Beat;
        
    public WaitForBeat(double beat)
    {
        Beat = beat;
    }

    public override void OnEnter() 
    {
        CheckTime();
        if (IsAlive)
            ResumeUpdates();
    }

    public override void Update() 
    {
        CheckTime();
    }

    private void CheckTime() 
    {
        if (Conductor.CurrentBeat >= Beat)
            Kill();
    }
}

/// <summary>
/// A coroutine that waits for a set amount of beats.
/// </summary>
public class WaitBeats : WaitForBeat
{
    public WaitBeats(double beats) : base(beats)
    {
        Beat = Conductor.CurrentBeat + beats;
    }
}

/// <summary>
/// A coroutine that waits until the global Conductor's step passes the provided step.
/// </summary>
public class WaitForStep : CoroutineBase
{
    protected double Step;
        
    public WaitForStep(double step)
    {
        Step = step;
    }

    public override void OnEnter() 
    {
        CheckTime();
        if (IsAlive)
            ResumeUpdates();
    }

    public override void Update() 
    {
        CheckTime();
    }

    private void CheckTime() 
    {
        if (Conductor.CurrentStep >= Step)
            Kill();
    }
}

/// <summary>
/// A coroutine that waits for a set amount of steps.
/// </summary>
public class WaitSteps : WaitForStep
{
    public WaitSteps(double steps) : base(steps)
    {
        Step = Conductor.CurrentStep + steps;
    }
}

   
public class WaitForTime : CoroutineBase
{
    protected double Time;
        
    public WaitForTime(double time)
    {
        Time = time;
    }

    public override void OnEnter() 
    {
        CheckTime();
        if (IsAlive)
            ResumeUpdates();
    }

    public override void Update() 
    {
        CheckTime();
    }

    private void CheckTime() 
    {
        if (Conductor.Time >= Time)
            Kill();
    }
}

/// <summary>
/// A coroutine that waits for a set amount of seconds.
/// </summary>
public class WaitTime : WaitForTime
{
    public WaitTime(double seconds) : base(seconds)
    {
        Time = Conductor.Time + seconds;
    }
}