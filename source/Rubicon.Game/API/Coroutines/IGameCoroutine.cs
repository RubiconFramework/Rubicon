using System.Collections;

namespace Rubicon.Game.API.Coroutines;

/// <summary>
/// An interface utilizing coroutines to be inherited by any C# classes to be used for songs or stages.
/// Used for when you want to create a sequence that can wait for certain actions until certain conditions are met.
/// </summary>
public interface IGameCoroutine
{
    /// <summary>
    /// The sequence to execute when the song is loaded.
    /// </summary>
    /// <returns>An IEnumerator</returns>
    public IEnumerator Execute();
}