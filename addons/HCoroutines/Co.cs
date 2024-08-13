// Hierarchical Coroutines v2.0.2 for Godot
// by @Inspiaaa, @BangL

using HCoroutines.Coroutines;

namespace HCoroutines;

using System;
using System.Collections;
using System.Threading.Tasks;
using Godot;

/// <summary>
/// Class that allows for easy access to the standard coroutine types.
/// </summary>
public static class Co {
    private static Coroutine[] GetCoroutines(IEnumerator[] enumerators) {
        Coroutine[] coroutines = new Coroutine[enumerators.Length];

        for (int i = 0; i < enumerators.Length; i++) {
            coroutines[i] = new Coroutine(enumerators[i]);
        }

        return coroutines;
    }

    public static float DeltaTime => CoroutineManager.Instance.DeltaTime;

    public static double DeltaTimeDouble => CoroutineManager.Instance.DeltaTimeDouble;

    public static void Run(CoroutineBase coroutine) {
        CoroutineManager.Instance.StartCoroutine(coroutine);
    }

    public static Coroutine Run(IEnumerator coroutine) {
        Coroutine co = new(coroutine);
        CoroutineManager.Instance.StartCoroutine(co);
        return co;
    }

    public static Coroutine Run(Func<IEnumerator> creator) {
        return Run(creator());
    }

    public static Coroutine Run(Func<Coroutine, IEnumerator> creator) {
        Coroutine coroutine = new(creator);
        CoroutineManager.Instance.StartCoroutine(coroutine);
        return coroutine;
    }

    public static Coroutine Coroutine(IEnumerator enumerator) {
        return new(enumerator);
    }

    public static Coroutine Coroutine(Func<Coroutine, IEnumerator> creator) {
        return new(creator);
    }

    public static ParallelCoroutine Parallel(params IEnumerator[] enumerators) {
        return new(GetCoroutines(enumerators));
    }

    public static ParallelCoroutine Parallel(params CoroutineBase[] coroutines) {
        return new(coroutines);
    }

    public static SequentialCoroutine Sequence(params IEnumerator[] enumerators) {
        return new(GetCoroutines(enumerators));
    }

    public static SequentialCoroutine Sequence(params CoroutineBase[] coroutines) {
        return new(coroutines);
    }

    public static WaitDelayCoroutine Wait(float delay) {
        return new(delay);
    }

    public static WaitWhileCoroutine WaitWhile(Func<bool> condition) {
        return new(condition);
    }

    public static WaitUntilCoroutine WaitUntil(Func<bool> condition) {
        return new(condition);
    }

    public static WaitForSignalCoroutine WaitForSignal(GodotObject obj, string signal) {
        return new(obj, signal);
    }

    public static RepeatCoroutine Repeat(int times, Func<RepeatCoroutine, CoroutineBase> creator) {
        return new(times, creator);
    }

    public static RepeatCoroutine Repeat(int times, Func<CoroutineBase> creator) {
        return new(times, _ => creator());
    }

    public static RepeatCoroutine Repeat(int times, Func<RepeatCoroutine, IEnumerator> creator) {
        return new(times, coroutine => new Coroutine(creator(coroutine)));
    }

    public static RepeatCoroutine Repeat(int times, Func<IEnumerator> creator) {
        return new(times, _ => new Coroutine(creator()));
    }

    public static RepeatCoroutine RepeatInfinitely(Func<RepeatCoroutine, CoroutineBase> creator) {
        return new(-1, creator);
    }

    public static RepeatCoroutine RepeatInfinitely(Func<CoroutineBase> creator) {
        return new(-1, _ => creator());
    }

    public static RepeatCoroutine RepeatInfinitely(Func<RepeatCoroutine, IEnumerator> creator) {
        return new(-1, coroutine => new Coroutine(creator(coroutine)));
    }

    public static RepeatCoroutine RepeatInfinitely(Func<IEnumerator> creator) {
        return new(-1, _ => new Coroutine(creator()));
    }

    public static TweenCoroutine Tween(Action<Tween> setupTween) {
        return new(setupTween);
    }

    public static AwaitCoroutine<T> Await<T>(Task<T> task) {
        return new(task);
    }

    public static AwaitCoroutine Await(Task task) {
        return new(task);
    }
}