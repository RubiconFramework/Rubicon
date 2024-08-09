namespace HCoroutines;

/// <summary>
/// Runs multiple coroutines in parallel and exits once all have completed.
/// </summary>
public class ParallelCoroutine : CoroutineBase {
    private readonly CoroutineBase[] _coroutines;

    public ParallelCoroutine(params CoroutineBase[] coroutines) {
        _coroutines = coroutines;
    }

    public override void OnEnter() {
        if (_coroutines.Length == 0) {
            Kill();
            return;
        }

        foreach (CoroutineBase coroutine in _coroutines) {
            StartCoroutine(coroutine);
        }
    }

    public override void OnChildStopped(CoroutineBase child) {
        base.OnChildStopped(child);

        // If there are no more actively running coroutines, stop.
        if (_firstChild == null) {
            Kill();
        }
    }
}