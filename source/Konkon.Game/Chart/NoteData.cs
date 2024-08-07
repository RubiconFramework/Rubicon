using System.Linq;
using Godot;

namespace Konkon.Game.Chart
{
    /// <summary>
    /// Used for storing a note's data values. Is not the actual Note object.
    /// </summary>
    [GlobalClass]
    public partial class NoteData : RefCounted
    {
        /// <summary>
        /// The note's lane
        /// </summary>
        public int Lane = 0;
        
        /// <summary>
        /// The note's type.
        /// </summary>
        public string Type = "normal";

        /// <summary>
        /// Starting point of the note. Stored on disk in measures.
        /// </summary>
        public double Time = 0d;

        /// <summary>
        /// Ending point of the note. Stored on disk in measures.
        /// </summary>
        public double Length = 0d;
        
        /// <summary>
        /// Length of the note converted to milliseconds. Should be ignored when serialized.
        /// </summary>
        public double MsTime = 0d;
        
        /// <summary>
        /// Length of the note converted to milliseconds. Should be ignored when serialized.
        /// </summary>
        public double MsLength = 0d;

        /// <summary>
        /// Basically tells the autoplay whether to miss this note or not. Should be ignored when serialized.
        /// </summary>
        public bool ShouldMiss = false;

        /// <summary>
        /// Converts the pre-existing Time and Length variables to milliseconds and stores them in MsTime and MsLength, using the provided BpmInfo.
        /// </summary>
        /// <param name="bpmInfo">An Array of BpmInfos</param>
        public void ConvertData(BpmInfo[] bpmInfo)
        {
            BpmInfo bpm = bpmInfo.Last();
            for (int i = 0; i < bpmInfo.Length; i++)
            {
                if (bpmInfo[i].Time > Time)
                {
                    bpm = bpmInfo[i - 1];
                    break;
                }
            }

            MsTime = Util.MeasureToMs(Time - bpm.Time, bpm.Bpm, bpm.TimeSignatureNumerator) + bpm.MsTime;
            MsLength = Util.MeasureToMs(Length, bpm.Bpm, bpm.TimeSignatureNumerator);
        }

        /// <summary>
        /// Converts its values into a Godot Dictionary to be used as a Variant.
        /// </summary>
        /// <returns>The NoteData, in Godot.Collections.Dictionary form.</returns>
        public Godot.Collections.Dictionary ToDictionary()
        {
            Godot.Collections.Dictionary returnDict = new Godot.Collections.Dictionary
            {
                { "Lane", Lane },
                { "Type", Type },
                { "Time", Time },
                { "Length", Length }
            };
            
            return returnDict;
        }
    }   
}