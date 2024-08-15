using Rubicon.Data.Events;

namespace Rubicon.API.Events;

/// <summary>
/// An interface to be inherited by any C# song events.
/// </summary>
public interface ISongEvent 
{
    /// <summary>
    /// The name of the song event.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// This function is executed when any chart events matches the name specified in the Name field.
    /// Will execute multiple times for every matching chart event found.
    /// </summary>
    /// <param name="eventData">The event data passed through</param>
    public void OnReady(EventData eventData) { }

    /// <summary>
    /// This function is executed upon the global Conductor passing the event data's time.
    /// </summary>
    /// <param name="args">The arguments specified</param>
    public void OnTrigger(string[] args);

    /// <summary>
    /// This function is executed upon the event controller being freed from memory.
    /// </summary>
    public void OnFree() { }
}