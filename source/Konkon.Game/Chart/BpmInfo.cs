using Godot;

namespace Konkon.Game.Chart
{
    [GlobalClass]
    public partial class BpmInfo : RefCounted
    {
        public double MsTime = 0; // Gets ignored when getting serialized

        public double Time;
            
        public double Bpm;
    }
}