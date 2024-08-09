namespace HCoroutines;

using System;
using Godot;

/// <summary>
/// Base class of all coroutines that allows for pausing / resuming / killing / ... the coroutine.
/// It is also responsible for managing the hierarchical structure and organization
/// of coroutine nodes.
/// The coroutines themselves act like a doubly linked list, so that
/// the list of children can be efficiently managed and even modified during iteration.
/// </summary>
public class CoroutineBase : ICoroutineStopListener {
    protected ICoroutineStopListener _stopListener;
    internal CoroutineManager Manager { get; set; }

    protected CoroutineBase _firstChild, _lastChild;
    protected CoroutineBase _previousSibling, _nextSibling;

    public bool IsAlive { get; private set; } = true;
    public bool IsPlaying { get; private set; }

    public void StartCoroutine(CoroutineBase coroutine) {
        coroutine._stopListener = this;
        coroutine.Manager = Manager;

        AddChild(coroutine);
        coroutine.OnEnter();
    }

    /// <summary>
    /// Called when the coroutine starts.
    /// </summary>
    public virtual void OnEnter() {
        ResumeUpdates();
    }

    /// <summary>
    /// Called every frame if the coroutine is playing.
    /// </summary>
    public virtual void Update() { }

    /// <summary>
    /// Called when the coroutine is killed.
    /// </summary>
    public virtual void OnExit() { }

    /// <summary>
    /// Starts playing this coroutine, meaning that it will receive Update() calls
    /// each frame. This is independent of the child coroutines.
    /// This method only works if the coroutine is still alive.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public void ResumeUpdates() {
        if (!IsAlive) {
            throw new InvalidOperationException("Cannot resume updates on dead coroutine.");
        }

        IsPlaying = true;
        Manager.ActivateCoroutine(this);
    }

    /// <summary>
    /// Stops giving the coroutine Update() calls each frame.
    /// This is independent of the child coroutines.
    /// </summary>
    public void PauseUpdates() {
        IsPlaying = false;
        Manager.DeactivateCoroutine(this);
    }

    public virtual void OnChildStopped(CoroutineBase child) {
        // If the parent coroutine is dead, then there is no reason to
        // manually remove the child coroutines
        if (IsAlive) {
            RemoveChild(child);
        }
    }

    /// <summary>
    /// Kills this coroutine and all child coroutines that were started using
    /// StartCoroutine(...) on this coroutine.
    /// </summary>
    public void Kill() {
        if (!IsAlive) {
            return;
        }

        try {
            OnExit();
        } catch (Exception e) {
            GD.PrintErr(e.ToString());
        }

        IsAlive = false;
        Manager.DeactivateCoroutine(this);

        CoroutineBase child = _firstChild;
        while (child != null) {
            child.Kill();
            child = child._nextSibling;
        }

        _stopListener?.OnChildStopped(this);
    }

    /// <summary>
    /// Adds a coroutine as a child.
    /// </summary>
    /// <param name="coroutine"></param>
    protected void AddChild(CoroutineBase coroutine) {
        if (_firstChild == null) {
            _firstChild = coroutine;
            _lastChild = coroutine;
        } else {
            _lastChild._nextSibling = coroutine;
            coroutine._previousSibling = _lastChild;
            _lastChild = coroutine;
        }
    }

    /// <summary>
    /// Removes a child from the list of child coroutines.
    /// </summary>
    /// <param name="coroutine"></param>
    protected void RemoveChild(CoroutineBase coroutine) {
        if (coroutine._previousSibling != null) {
            coroutine._previousSibling._nextSibling = coroutine._nextSibling;
        }

        if (coroutine._nextSibling != null) {
            coroutine._nextSibling._previousSibling = coroutine._previousSibling;
        }

        if (_firstChild == coroutine) {
            _firstChild = coroutine._nextSibling;
        }

        if (_lastChild == coroutine) {
            _lastChild = coroutine._previousSibling;
        }
    }
}
