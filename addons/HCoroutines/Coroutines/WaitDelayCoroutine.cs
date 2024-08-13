using HCoroutines.Util;

namespace HCoroutines.Coroutines;

/// <summary>
/// Waits until a certain delay has passed.
/// </summary>
public class WaitDelayCoroutine : CoroutineBase {
    private readonly float _delay;
    private int _schedulerId;

    public WaitDelayCoroutine(float delay) {
        _delay = delay;
    }

    public override void OnEnter() {
        _schedulerId = TimeScheduler.Instance.Schedule(Kill, _delay);
    }

    public override void OnExit() {
        TimeScheduler.Instance.CancelSchedule(_schedulerId);
    }
}