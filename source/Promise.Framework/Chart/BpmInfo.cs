using Godot;

namespace Promise.Framework.Chart
{
    [GlobalClass]
    public partial class BpmInfo : RefCounted
    {
        public double MsTime = 0d; // Gets ignored when getting serialized

        public double Time;
            
        public double Bpm;
        
        /// <summary>
        /// The number of beats in a measure with this BPM info.
        /// </summary>
        public int TimeSignatureNumerator = 4;
        
        /// <summary>
        /// The type of note which equals one beat with this BPM info.
        /// </summary>
        public int TimeSignatureDenominator = 4;
    }
}