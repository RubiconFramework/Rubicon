using Godot;
using Konkon.Data;
using Konkon.Game.Chart;

namespace Konkon.Game.Objects
{
    [GlobalClass]
    public partial class ChartController : Control
    {
        public NoteLaneController[] Lanes { get; private set; }

        //[Signal] public delegate void NoteHitEventHandler(ChartController chartCtrl, NoteData noteData, int hitType, float hitMs, bool held, NoteEventResult result);
        //[Signal] public delegate void NoteMissEventHandler(ChartController chartCtrl, NoteData noteData, float hitMs, NoteEventResult result);
        //[Signal] public delegate void PressedEventHandler(ChartController chartCtrl);
        
        //public void Initialize()
        
        public void OnNoteHit(NoteData noteData, NoteHitType hitType, double distanceFromTime, bool held = false)
        {
            
        }

        public void OnNoteMiss(NoteData noteData, double distanceFromTime)
        {
            
        }
        
        public void OnLanePress()
        {
            //EmitSignal(SignalName.Pressed, this);
        }
    }
}