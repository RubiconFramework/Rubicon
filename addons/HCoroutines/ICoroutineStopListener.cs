namespace HCoroutines;

public interface ICoroutineStopListener {
    void OnChildStopped(CoroutineBase child);
}
