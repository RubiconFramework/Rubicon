using System;
using System.Linq;
using Godot;
using Konkon.Data;
using Konkon.Game.Chart;
using Konkon.Game.UI.Noteskins;
using Konkon.Utilities;

namespace Konkon.Game.Objects
{
    /// <summary>
    /// The individual lane that controls scrolling notes and hitting notes! Also referred to some as a "strum" or a "receptor".
    /// </summary>
    [GlobalClass]
    public partial class NoteLaneController : Control
    {
        #region Exported Variables
        /// <summary>
        /// Indicates which lane this controller is.
        /// </summary>
        [ExportGroup("Status"), Export] public int Lane { get; private set; } = 0;

        /// <summary>
        /// True if either the player or the autoplay is pressing on this lane.
        /// </summary>
        [Export] public bool Pressed { get; private set; } = false;

        /// <summary>
        /// The note this lane is currently holding.
        /// </summary>
        [Export] public NoteData NoteHeld = null;

        /// <summary>
        /// The current index for spawning notes.
        /// </summary>
        [ExportSubgroup("Note Indexes"), Export] private long _noteSpawnIndex = 0;

        /// <summary>
        /// The current index for hitting notes.
        /// </summary>
        [Export] private long _noteHitIndex = 0;

        /// <summary>
        /// Contains all the data for the notes to spawn with.
        /// </summary>
        [ExportGroup("Settings"), Export] public NoteData[] Notes = Array.Empty<NoteData>();

        /// <summary>
        /// The rate at which the notes scroll towards the controller.
        /// </summary>
        [Export]
        public float ScrollSpeed
        {
            get => _scrollSpeed;
            set
            {
                _scrollSpeed = value;
                for (int i = 0; i < NoteContainer.GetChildCount(); i++)
                    NoteContainer.GetChild<Note>(i).AdjustScrollSpeed();
            }
        }

        /// <summary>
        /// The current note skin being used on this lane.
        /// </summary>
        [Export]
        public NoteSkin NoteSkin
        {
            get => _noteSkin;
            set => ChangeNoteSkin(value);
        }
        
        /// <summary>
        /// True to let the computer play, otherwise controllable by the player.
        /// </summary>
        [Export] public bool Autoplay = true;
        
        /// <summary>
        /// The angle which the notes come from in radians.
        /// </summary>
        [Export] public float DirectionAngle = 0f;

        /// <summary>
        /// A reference to the ChartController this lane belongs to.
        /// </summary>
        [ExportGroup("References"), Export] public ChartController ParentController { get; private set; }

        /// <summary>
        /// A reference to this controller's lane graphic.
        /// </summary>
        [Export] public Control LaneGraphic;

        /// <summary>
        /// A reference to the lane graphic's animation player.
        /// </summary>
        [Export] public AnimationPlayer AnimationPlayer { get; private set; }

        /// <summary>
        /// The Control node that contains all of this lane's notes.
        /// </summary>
        [Export] public Control NoteContainer;

        /// <summary>
        /// The lane's resource containing both the note and the tail.
        /// </summary>
        [ExportGroup("Debug"), Export] private NotePair _notePair;

        /// <summary>
        /// The lane's material, if the note skin requires the RGB shader.
        /// </summary>
        [Export] private Material _noteMaterial;
        #endregion

        #region Private Variables
        private NoteSkin _noteSkin;
        private float _scrollSpeed = 1f;
        #endregion

        #region Public Methods
        
        #region Godot Overrides
        
        /// <summary>
        /// Updates every frame. The graphical note spawner and autoplay relies on this!
        /// </summary>
        /// <param name="delta"></param>
        public override void _Process(double delta)
        {
            base._Process(delta);

            // Sorry, I'm still super paranoid from like Haxe PTSD, bear with me here...
            double songPos = Conductor.Instance.Time * 1000d;
            if (NoteHeld != null && NoteHeld.MsTime + NoteHeld.Length < songPos)
                OnNoteHit(NoteHeld, 0, false);
        }
        
        /// <summary>
        /// This function handles all the incoming input from pretty much all input devices.
        /// </summary>
        /// <param name="event">The InputEvent</param>
        public override void _Input(InputEvent @event)
        {
            base._Input(@event);

            // keyboard ctrls
            if (@event is InputEventKey eventKey)
            {
                if (eventKey.Keycode != SaveData.Data.Keybinds[ParentController.Lanes.Length][Lane])
                    return;
				
                // Key Down Event
                if (eventKey.Pressed && !Autoplay && !Pressed)
                {
                    Pressed = true;
                    OnInputDown();
                }

                // Key Up Event
                if (!eventKey.Pressed && !Autoplay && Pressed)
                {
                    Pressed = false;
                    OnInputUp();
                }

                ParentController.OnLanePress();
            }
        }
        
        #endregion

        #region Note Hitting and Missing
        /// <summary>
        /// Is activated when this lane hits a note.
        /// </summary>
        /// <param name="noteData">The note data</param>
        /// <param name="distanceFromTime">The distance from when the note was supposed to be hit.</param>
        /// <param name="isHolding">Is true when the note is a hold note and the lane is FIRST holding it. False if it's a regular note, or the hold note was let go/completed</param>
        public void OnNoteHit(NoteData noteData, double distanceFromTime, bool isHolding)
        {
            LaneGraphic.Material = _noteMaterial;
            AnimationPlayer.Play("confirm");
            AnimationPlayer.Seek(0d, true);

            if (isHolding)
            {
                NoteHeld = noteData;
                AnimationPlayer.Pause();
                AnimationPlayer.Seek(0, true); // for good measure :3
            }
            else
            {
                Note[] notes = NoteContainer.GetChildrenOfType<Note>();
                if (notes != null)
                { 
                    Note[] matchingNotes = notes.Where(x => x.Data == noteData).ToArray();
                    for (int i = 0; i >= matchingNotes.Length; i++)
                        matchingNotes[i].QueueFree();
                }

                NoteHeld = null;
            }
            
            int hit = GameData.HitWindows.Length - 1;
            for (int i = 0; i < GameData.HitWindows.Length; i++)
            {
                if (Mathf.Abs(distanceFromTime) <= GameData.HitWindows[i])
                {
                    hit = i;
                    break;
                }
            }
            
            ParentController.OnNoteHit(noteData, GameData.HitTypes[hit], distanceFromTime, isHolding);
        }
        
        /// <summary>
        /// Is activated when this lane misses a note.
        /// </summary>
        /// <param name="noteData">The note data</param>
        /// <param name="distanceFromTime">The distance from when the note was supposed to be hit.</param>
        public void OnNoteMiss(NoteData noteData, double distanceFromTime)
        {
            ParentController.OnNoteMiss(noteData, distanceFromTime);

            Note[] notes = NoteContainer.GetChildrenOfType<Note>();
            if (notes != null)
            { 
                Note[] matchingNotes = notes.Where(x => x.Data == noteData).ToArray();
                for (int i = 0; i >= matchingNotes.Length; i++)
                {
                    matchingNotes[i].Missed = true;
                    if (NoteHeld != null && NoteHeld == noteData)
                        matchingNotes[i].UnsetHold();
                }
            }
            
            NoteHeld = null;
        }
        #endregion
        
        /// <summary>
        /// Changes the general note skin on this specific lane.
        /// </summary>
        /// <param name="noteSkin">The note skin to switch to</param>
        public void ChangeNoteSkin(NoteSkin noteSkin)
        {
            int laneCount = ParentController.Lanes.Length;
            
            _noteSkin = noteSkin;
            _notePair = _noteSkin.GetNoteFromLaneGroup(laneCount, Lane);
            _noteMaterial = _noteSkin.UseRgbShader ? _noteSkin.GetMaterialFromLaneGroup(laneCount, Lane) : null;
            
            if (LaneGraphic != null && !LaneGraphic.IsQueuedForDeletion())
                LaneGraphic.QueueFree();

            LaneGraphic = _noteSkin.GetLaneFromLaneGroup(laneCount, Lane).Instantiate<Control>();
            LaneGraphic.Name = "Lane Graphic";
            
            AddChild(LaneGraphic);
            MoveChild(LaneGraphic, Math.Max(NoteContainer.GetIndex() - 1, 0));

            // Probably a better way to do this tbh
            AnimationPlayer = LaneGraphic.GetChildOfType<AnimationPlayer>();
            AnimationPlayer.AnimationFinished += OnAnimationFinish;
        }
        #endregion
        
        #region Private Methods

        #region Input Methods
        private void OnInputDown()
        {
            if (_noteHitIndex >= Notes.Length)
            {
                LaneGraphic.Set("material", _noteMaterial);
                AnimationPlayer.Play("pressed");
                AnimationPlayer.Seek(0, true);
                return;
            }

            double songPos = Conductor.Instance.Time * 1000d; // calling it once since this can lag the game HORRIBLY if used without caution
            while (Notes[_noteHitIndex].MsTime - songPos - SaveData.Data.Offset <= -GameData.HitWindows.Last())
            {
                // Miss every note thats too late first
                OnNoteMiss(Notes[_noteHitIndex], -GameData.HitWindows.Last() - 1);
                _noteHitIndex++;
            }

            double hitTime = Notes[_noteHitIndex].MsTime - songPos - SaveData.Data.Offset;
            if (Mathf.Abs(hitTime) <= GameData.HitWindows.Last()) // Literally any other rating
            {
                OnNoteHit(Notes[_noteHitIndex], hitTime, Notes[_noteHitIndex].Length > 0);
                _noteHitIndex++;
            }
            else if (hitTime < -GameData.HitWindows.Last()) // Your Miss / "SHIT" rating
            {
                LaneGraphic.Set("material", _noteMaterial);
                AnimationPlayer.Play("confirm"); // i guess it'd still be a good idea to give the satisfaction of hittin a note
                AnimationPlayer.Seek(0, true);

                OnNoteMiss(Notes[_noteHitIndex], hitTime);
                _noteHitIndex++;
            }
            else
            {
                LaneGraphic.Material = _noteMaterial;
                AnimationPlayer.Play("pressed");
                AnimationPlayer.Seek(0, true);
            }
        }
        
        private void OnInputUp()
        {
            if (NoteHeld != null)
            {
                double length = NoteHeld.MsTime + NoteHeld.Length - (Conductor.Instance.Time * 1000d) - SaveData.Data.Offset;
                if (length <= GameData.HitWindows.Last())
                    OnNoteHit(NoteHeld, length, false);
                else
                    OnNoteMiss(NoteHeld, length);
            }

            LaneGraphic.Material = null;
            AnimationPlayer.Play("idle");
            AnimationPlayer.Seek(0, true);
        }
        #endregion
        
        private void OnAnimationFinish(StringName animName)
        {
            if (!Autoplay || animName != "confirm")
                return;

            LaneGraphic.Material = null;
            AnimationPlayer.Play("idle");
            AnimationPlayer.Seek(0, true);
        }
        #endregion
    }
}