using System.Collections;

namespace HCoroutines.Coroutines;

/// <summary>
/// Runs an IEnumerator as a coroutine (like Unity).
/// If it returns null, it waits until the next frame before continuing
/// the IEnumerator. If it returns another CoroutineBase, it will start it
/// and wait until this new coroutine has finished before continuing the
/// IEnumerator.
/// </summary>
public class Coroutine : CoroutineBase {
    private readonly IEnumerator _routine;

    public Coroutine(IEnumerator routine) {
        _routine = routine;
    }

    public Coroutine(Func<Coroutine, IEnumerator> creator) {
        _routine = creator(this);
    }

    public override void OnEnter() {
        if (_routine == null) {
            Kill();
            return;
        }

        ResumeUpdates();
    }

    public override void Update() {
        if (!_routine.MoveNext()) {
            Kill();
            return;
        }

        object obj = _routine.Current;

        // yield return null; => do nothing.
        if (obj is null) {
            return;
        }

        // yield return some coroutine; => Pause until the returned
        // coroutine is finished.
        if (obj is CoroutineBase childCoroutine) {
            // It's important to pause before starting the child coroutine.
            // Otherwise, if the child coroutine instantly terminates, which would
            // lead to this coroutine resuming, it would pause this coroutine.
            // That would not be correct.
            PauseUpdates();
            StartCoroutine(childCoroutine);
            return;
        }

        // yield return some other enumerator; => Create new Coroutine
        // and pause until it is finished
        if (obj is IEnumerator childEnumerator) {
            PauseUpdates();
            StartCoroutine(new Coroutine(childEnumerator));
        }
    }

    public override void OnChildStopped(CoroutineBase child) {
        base.OnChildStopped(child);
        ResumeUpdates();
    }
}