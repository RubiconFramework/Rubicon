namespace HCoroutines;

using System;

/// <summary>
/// Waits while a certain condition is true.
/// </summary>
public class WaitWhileCoroutine : CoroutineBase {
    private readonly Func<bool> _condition;

    public WaitWhileCoroutine(Func<bool> condition) {
        _condition = condition;
    }

    public override void OnEnter() {
        CheckCondition();
        if (IsAlive) {
            ResumeUpdates();
        }
    }

    public override void Update() {
        CheckCondition();
    }

    private void CheckCondition() {
        if (!_condition()) {
            Kill();
        }
    }
}