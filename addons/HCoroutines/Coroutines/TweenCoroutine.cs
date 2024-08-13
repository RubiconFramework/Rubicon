using System;
using Godot;
using HCoroutines.Util;

namespace HCoroutines.Coroutines;

/// <summary>
/// A coroutine that manages a tween instance.
/// When the tween is finished, the coroutine also finishes.
/// If the coroutine is killed before that, it also kills the tween instance.
/// </summary>
public class TweenCoroutine : CoroutineBase {
    private readonly Action<Tween> _setupTween;
    private Tween _tween;
    private int _schedulerId;

    public TweenCoroutine(Action<Tween> setupTween) {
        _setupTween = setupTween;
    }

    public override void OnEnter() {
        _tween = Manager.CreateTween();
        _setupTween(_tween);
        _schedulerId = TimeScheduler.Instance.ScheduleOnSignal(Kill, _tween, "finished");
    }

    public override void OnExit() {
        _tween.Kill();
        TimeScheduler.Instance.CancelSchedule(_schedulerId);
    }
}