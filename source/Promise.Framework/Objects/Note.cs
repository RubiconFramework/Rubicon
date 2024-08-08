using Godot;
using Promise.Framework.Chart;
using Promise.Framework.UI.Noteskins;

namespace Promise.Framework.Objects
{
    /// <summary>
    /// The Note object displayed graphically!
    /// </summary>
    [GlobalClass]
    public partial class Note : Control
    {
        /// <summary>
        /// Whether the note was missed.
        /// </summary>
        [ExportGroup("Status"), Export] public bool Missed = false;
        
        /// <summary>
        /// The data containing the time and length of this note.
        /// </summary>
        [ExportGroup("Settings"), Export] public NoteData Data { get; private set; }

        /// <summary>
        /// The alpha value of the graphic's Modulate.
        /// </summary>
        [Export] public float Alpha = 1f;

        /// <summary>
        /// Multiplies the scroll speed by whatever this value's set to.
        /// </summary>
        [Export] public float ScrollMultiplier = 1f;
        
        /// <summary>
        /// The note pair containing the note skin's note and tail graphics.
        /// </summary>
        [Export] private NotePair _notePair;

        /// <summary>
        /// The parent NoteLaneController that this note belongs to.
        /// </summary>
        [ExportGroup("References"), Export] public NoteLaneController ParentLane { get; private set; }

        /// <summary>
        /// The graphic for this note.
        /// </summary>
        [ExportSubgroup("Note"), Export] public Control NoteGraphic;

        /// <summary>
        /// The graphic for this note's tail, provided the length of this note is longer than 0.
        /// </summary>
        [Export] public Control TailGraphic;

        [ExportGroup("Debug"), Export] private float _tailOffset = 0f;

        /// <summary>
        /// Usually called to set up the Note for the first time.
        /// </summary>
        /// <param name="noteData">The note data provided</param>
        /// <param name="parent">The parent NoteLaneController</param>
        /// <param name="notePair">The note pair that comes with the note skin</param>
        /// <param name="material">The material, if the note skin requires the RGB shader.</param>
        public void Initialize(NoteData noteData, NoteLaneController parent, NotePair notePair, Material material = null)
        {
            Data = noteData;
            ParentLane = parent;
            Position = new Vector2(0, -5000);
            
            ChangeNotePair(notePair, material);
        }
        
        public override void _Process(double delta)
        {
            base._Process(delta);

            if (ParentLane == null || Data == null || !Visible)
                return;

            float alphaMult = 1f;
            if (Missed)
                alphaMult = 0.5f;

            Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, Alpha * alphaMult);

            float songPos = (float)(Conductor.Instance.Time * 1000d);
            bool isHeld = Data.Length > 0 && ParentLane.NoteHeld != null && ParentLane.NoteHeld == Data;

            // in radians
            float noteAngle = ParentLane.DirectionAngle;

            float scrollSpeed = ParentLane.ScrollSpeed * ScrollMultiplier;
            float distance = isHeld ? 0f : (float)(Data.MsTime - songPos - _tailOffset + PromiseData.VisualOffset) * scrollSpeed;
            Vector2 posMult = new Vector2(Mathf.Cos(noteAngle), Mathf.Sin(noteAngle));
            Position = new Vector2(distance, distance) * posMult;

            if (Data.Length > 0)
            {
                TailGraphic.Rotation = noteAngle;
                
                if (isHeld)
                {
                    AdjustTailLength(Data.MsTime + Data.Length - songPos + PromiseData.VisualOffset);
                    NoteGraphic.Visible = false;
                }
            }

            if (Data.MsTime + Data.Length - songPos <= -1000f && !isHeld)
                QueueFree();
        }

        /// <summary>
        /// Changes the look of the note from the provided note pair.
        /// </summary>
        /// <param name="notePair">The note pair, usually comes from a note skin.</param>
        /// <param name="mat">The material, if the note skin needs the RGB shader.</param>
        public void ChangeNotePair(NotePair notePair, Material mat = null)
        {
            _notePair = notePair;
            
            NoteGraphic?.QueueFree();
            TailGraphic?.QueueFree();

            NoteGraphic = _notePair.Note.Instantiate<Control>();
            if (mat != null)
                NoteGraphic.Material = mat;
            
            AddChild(NoteGraphic);

            if (Data.Length <= 0d)
                return;
            
            TailGraphic = notePair.Tail.Instantiate<Control>();
                
            AdjustScrollSpeed();
            AdjustTailLength(Data.Length);
                
            TailGraphic.Modulate = new Color(TailGraphic.Modulate.R, TailGraphic.Modulate.G, TailGraphic.Modulate.B, 0.5f);

            if (mat != null)
                TailGraphic.GetNode<Control>("Tail").Material = TailGraphic.GetNode<Control>("End").Material = mat;

            AddChild(TailGraphic);
        }
        
        /// <summary>
        /// Called when the scroll speed is adjusted!
        /// </summary>
        public void AdjustScrollSpeed()
        {
            if (TailGraphic == null)
                return;
            
            Control tailContainer = TailGraphic.GetNode<Control>("Tail");
            Control tailEndContainer = TailGraphic.GetNode<Control>("End");

            TextureRect tailSpr = tailContainer.GetNode<TextureRect>("Texture");
            TextureRect tailEndSpr = tailEndContainer.GetNode<TextureRect>("Texture");
            
            float tailScale = (float)Data.Length / tailSpr.Texture.GetWidth() * (ParentLane.ScrollSpeed * ScrollMultiplier);
            float tailEndSub = tailEndSpr.Texture.GetWidth() * tailEndSpr.Scale.X / (tailSpr.Texture.GetWidth() * tailSpr.Scale.X);
            float tailWidth = tailSpr.Texture.GetWidth() * Mathf.Max(tailScale - tailEndSub, 0f);

            tailSpr.Size = new Vector2(tailWidth, tailSpr.Size.Y);
        }
        
        /// <summary>
        /// Called when initialized and when the note is being held.
        /// </summary>
        /// <param name="length">The length of the note</param>
        public void AdjustTailLength(double length)
        {
            if (TailGraphic == null)
                return;
            
            Control tailContainer = TailGraphic.GetNode<Control>("Tail");
            Control tailEndContainer = TailGraphic.GetNode<Control>("End");

            TextureRect tailSpr = tailContainer.GetNode<TextureRect>("Texture");
            TextureRect tailEndSpr = tailEndContainer.GetNode<TextureRect>("Texture");

            float tailScale = (float)length / tailSpr.Texture.GetWidth() * (ParentLane.ScrollSpeed * ScrollMultiplier);
            float tailEndSub = tailEndSpr.Texture.GetWidth() * tailEndSpr.Scale.X / (tailSpr.Texture.GetWidth() * tailSpr.Scale.X);
            float tailWidth = tailSpr.Texture.GetWidth() * Mathf.Max(tailScale - tailEndSub, 0f);

            tailContainer.Size = new Vector2(tailWidth, tailContainer.Size.Y);
            tailSpr.Position = new Vector2(tailWidth - (tailSpr.Scale.X * tailSpr.Size.X), tailSpr.Position.Y);
            tailEndContainer.Position = new Vector2(tailWidth, tailEndContainer.Position.Y);
            
            // If it's ever too short for some reason???
            if (tailScale - tailEndSub <= 0)
            {
                float tailEndWidth = (tailEndSpr.Texture.GetWidth() * tailEndSpr.Scale.X) * (1f + tailScale - tailEndSub);

                tailEndContainer.Size = new Vector2(tailEndWidth, tailEndContainer.Size.Y);
                tailEndSpr.Position = new Vector2(tailEndWidth - (tailEndSpr.Scale.X * tailEndSpr.Size.X), tailEndSpr.Position.Y);
            }
        }

        /// <summary>
        /// Usually called when the note was let go too early.
        /// </summary>
        public void UnsetHold()
        {
            // Should be based on time, NOT note Y position
            _tailOffset = (float)(Data.MsTime - Conductor.Instance.Time * 1000d);
        }
    }
}