using System.Collections.Generic;
using Godot;
using Konkon.Game.API;
using Konkon.Data;
using Konkon.Game;
using Konkon.Core.Chart;
using Konkon.Core.UI.Noteskins;

namespace Konkon.Core.Objects
{
    [GlobalClass]
    public partial class ChartController : Control
    {
        /// <summary>
        /// The current stats on this ChartController.
        /// </summary>
        [ExportGroup("Status"), Export] public ChartStatistics Statistics { get; private set; } = new();

        /// <summary>
        /// The current note skin assigned to this ChartController.
        /// </summary>
        [ExportGroup("Settings"), Export] public NoteSkin NoteSkin { get; private set; }

        /// <summary>
        /// The lanes assigned to this ChartController.
        /// </summary>
        [ExportGroup("References"), Export] public NoteLaneController[] Lanes { get; private set; }

        /// <summary>
        /// The chart data for this ChartController.
        /// </summary>
        [Export] public CharacterChart Chart { get; private set; }

        /// <summary>
        /// The note scripts currently running on this chart controller.
        /// </summary>
        public Dictionary<string, INoteScript> NoteScripts { get; private set; } = new();

        [Signal] public delegate void NoteHitEventHandler(ChartController chartCtrl, NoteData noteData, int hitType, float hitMs, bool held, NoteEventResult result);
        [Signal] public delegate void NoteMissEventHandler(ChartController chartCtrl, NoteData noteData, float hitMs, NoteEventResult result);
        [Signal] public delegate void PressedEventHandler(ChartController chartCtrl);

        public void Initialize(int laneCount, CharacterChart data, bool autoplay = true, float scrollSpeed = 1.0f, string uiStyle = "funkin", string noteSkin = "funkin")
        {
            NoteSkin = GD.Load<NoteSkin>($"res://assets/ui/noteskins/{noteSkin}/noteskin.tres");
        }
        
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