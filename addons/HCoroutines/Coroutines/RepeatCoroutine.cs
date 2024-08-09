namespace HCoroutines;

using System;

/// <summary>
/// Runs a coroutine multiple times. Each time the coroutine is finished,
/// it is restarted.
/// </summary>
public class RepeatCoroutine : CoroutineBase {
    private readonly int _repeatTimes;
    private int _currentRepeatCount;
    private readonly Func<RepeatCoroutine, CoroutineBase> _coroutineCreator;

    private bool IsInfinite => _repeatTimes == -1;

    public RepeatCoroutine(int repeatTimes, Func<RepeatCoroutine, CoroutineBase> coroutineCreator) {
        _repeatTimes = repeatTimes;
        _coroutineCreator = coroutineCreator;
    }

    public override void OnEnter() {
        if (_repeatTimes == 0) {
            Kill();
            return;
        }

        Repeat();
    }

    private void Repeat() {
        _currentRepeatCount++;
        CoroutineBase coroutine = _coroutineCreator.Invoke(this);
        StartCoroutine(coroutine);
    }

    public override void OnChildStopped(CoroutineBase child) {
        base.OnChildStopped(child);

        if (!IsInfinite && _currentRepeatCount > _repeatTimes) {
            Kill();
            return;
        }

        Repeat();
    }
}