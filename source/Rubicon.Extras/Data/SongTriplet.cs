using Promise.Framework.Chart;
using Rubicon.Data.Events;
using Rubicon.Data.Meta;

namespace Rubicon.Extras.Data;

/// <summary>
/// A class that contains all three objects needed for a song.
/// </summary>
public class SongTriplet
{
    /// <summary>
    /// The HoloChart instance. This contains the notes, BPM changes, and the time signature changes.
    /// </summary>
    public HoloChart Chart;

    /// <summary>
    /// The ChartEvents instance. This contains the events.
    /// </summary>
    public ChartEvents Events;

    /// <summary>
    /// The SongMetadata instance. This contains the other information.
    /// </summary>
    public SongMetadata Metadata;
}