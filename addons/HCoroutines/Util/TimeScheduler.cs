namespace HCoroutines.Util;

using System;
using System.Collections.Generic;
using Godot;

public partial class TimeScheduler : Node {
    public static TimeScheduler Instance { get; private set; }

    private readonly Dictionary<int, Action> _actionsById = new();
    private int _idCounter;

    public override void _EnterTree() {
        Instance = this;
    }

    private int GetNextScheduleId() {
        int id = _idCounter;
        // Allow for integer overflow to wrap around to the beginning.
        _idCounter = unchecked(_idCounter + 1);
        return id;
    }

    public int Schedule(Action action, float delay) {
        SceneTreeTimer timer = GetTree().CreateTimer(delay, processAlways: false);
        return ScheduleOnSignal(action, timer, "timeout");
    }

    public int ScheduleOnSignal(Action action, GodotObject obj, string signal) {
        int id = GetNextScheduleId();
        _actionsById[id] = action;

        obj.Connect(
            signal,
            Callable.From(() => CallCallback(id)),
            (int)ConnectFlags.OneShot
        );

        return id;
    }

    public void CancelSchedule(int scheduleId) {
        _actionsById.Remove(scheduleId);
    }

    private void CallCallback(int id) {
        if (_actionsById.TryGetValue(id, out Action action)) {
            action.Invoke();
            _actionsById.Remove(id);
        }
    }
}