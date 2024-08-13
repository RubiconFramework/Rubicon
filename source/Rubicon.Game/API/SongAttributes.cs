using System;

namespace Rubicon.Game.API;

/// <summary>
/// An attribute to put on an ISongCoroutine to allow usage on a specific song.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SongBind : Attribute
{
    public string Song { get; private set; }

    public SongBind(string song)
    {
        Song = song;
    }
}

/// <summary>
/// An attribute to put on an ISongCoroutine to allow usage on a specific stage.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class StageBind : Attribute
{
    public string Stage { get; private set; }

    public StageBind(string stage)
    {
        Stage = stage;
    }
}