namespace HCoroutines;

using System;

/// <summary>
/// Waits until a certain condition is true.
/// </summary>
public class WaitUntilCoroutine : CoroutineBase {
    private readonly Func<bool> _condition;

    public WaitUntilCoroutine(Func<bool> condition) {
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
        if (_condition()) {
            Kill();
        }
    }
}