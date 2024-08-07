using Godot;
using Konkon.Data;
using Konkon.Game.Chart;
using Konkon.UI.Noteskins;

namespace Konkon.Game.Objects
{
    [GlobalClass]
    public partial class Note : Control
    {
        [ExportGroup("Status"), Export] public bool Missed = false;
        
        [ExportGroup("Settings"), Export] public NoteData Data { get; private set; }

        [Export] public float Alpha = 1f;

        [Export] public float ScrollMultiplier = 1f;

        [Export] private NotePair _notePair;

        [ExportGroup("References"), Export] public NoteLaneController ParentLane { get; private set; }

        [ExportSubgroup("Note"), Export] public Control NoteGraphic;

        [Export] public Control TailGraphic;

        [ExportGroup("Debug"), Export] private float _tailOffset = 0f;

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
            float defaultAngle = (!SaveData.Data.Downscroll ? 1f : 3f) * (Mathf.Pi / 2f);
            float angleOffset = ParentLane.DirectionAngle;
            float noteAngle = defaultAngle + angleOffset;

            float scrollSpeed = ParentLane.ScrollSpeed * ScrollMultiplier;
            float distance = isHeld ? 0f : (float)(Data.MsTime - songPos - _tailOffset + SaveData.Data.VisualOffset) * scrollSpeed;
            Vector2 posMult = new Vector2(Mathf.Cos(noteAngle), Mathf.Sin(noteAngle));
            Position = new Vector2(distance, distance) * posMult;

            if (Data.Length > 0)
            {
                TailGraphic.Rotation = noteAngle;
                
                if (isHeld)
                {
                    AdjustTailLength(Data.MsTime + Data.Length - songPos + SaveData.Data.VisualOffset);
                    NoteGraphic.Visible = false;
                }
            }

            if (Data.MsTime + Data.Length - songPos <= -1000f && !isHeld)
                QueueFree();
        }

        public void ChangeNotePair(NotePair notePair, Material mat = null)
        {
            _notePair = notePair;
            
            NoteGraphic?.QueueFree();
            TailGraphic.QueueFree();

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

        public void UnsetHold()
        {
            // Should be based on time, NOT note Y position
            _tailOffset = (float)(Data.MsTime - Conductor.Instance.Time * 1000d);
        }
    }
}