using System.Threading.Tasks;

namespace HCoroutines.Coroutines;

/// <summary>
/// A coroutine that waits until an asynchronous task has been completed.
/// If the coroutine is killed before completion, the async task
/// will currently *not* be canceled.
/// </summary>
/// <typeparam name="T"></typeparam>
public class AwaitCoroutine<T> : CoroutineBase {
    public Task<T> Task { get; }

    public AwaitCoroutine(Task<T> task) {
        Task = task;
    }

    public override void OnEnter() {
        // As the CoroutineManager class is not thread safe, ensure that Kill()
        // is executed on the main Godot thread.
        TaskScheduler godotTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        Task.ContinueWith(_ => Kill(), godotTaskScheduler);
    }
}

/// <summary>
/// A coroutine that waits until an asynchronous task has been completed.
/// If the coroutine is killed before completion, the async task
/// will currently *not* be canceled.
/// </summary>
public class AwaitCoroutine : CoroutineBase {
    public Task Task { get; }

    public AwaitCoroutine(Task task) {
        Task = task;
    }

    public override void OnEnter() {
        // As the CoroutineManager class is not thread safe, ensure that Kill()
        // is executed on the main Godot thread.
        TaskScheduler godotTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        Task.ContinueWith(_ => Kill(), godotTaskScheduler);
    }
}