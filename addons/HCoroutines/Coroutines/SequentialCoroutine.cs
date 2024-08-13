namespace HCoroutines.Coroutines;

/// <summary>
/// Runs multiple coroutines one after another. Waits until the first
/// one has finished before starting the second one, ...
/// </summary>
public class SequentialCoroutine : CoroutineBase {
    private readonly CoroutineBase[] _coroutines;
    private int _idx;

    public SequentialCoroutine(params CoroutineBase[] coroutines) {
        _coroutines = coroutines;
    }

    public override void OnEnter() {
        if (_coroutines.Length == 0) {
            Kill();
            return;
        }

        StartCoroutine(_coroutines[0]);
    }

    public override void OnChildStopped(CoroutineBase child) {
        base.OnChildStopped(child);

        _idx++;
        if (_idx < _coroutines.Length) {
            StartCoroutine(_coroutines[_idx]);
        } else {
            Kill();
        }
    }
}