namespace HCoroutines;

using Godot;
using Util;

/// <summary>
/// Waits until a certain signal has been fired.
/// </summary>
public class WaitForSignalCoroutine : CoroutineBase {
    private readonly GodotObject _targetObject;
    private readonly string _targetSignal;
    private int _schedulerId;

    public WaitForSignalCoroutine(GodotObject obj, string signal) {
        _targetObject = obj;
        _targetSignal = signal;
    }

    public override void OnEnter() {
        _schedulerId = TimeScheduler.Instance.ScheduleOnSignal(Kill, _targetObject, _targetSignal);
    }

    public override void OnExit() {
        TimeScheduler.Instance.CancelSchedule(_schedulerId);
    }
}