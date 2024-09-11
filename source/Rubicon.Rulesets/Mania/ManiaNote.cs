using Rubicon.Core;
using Rubicon.Core.Chart;

namespace Rubicon.Rulesets.Mania;

public partial class ManiaNote : Note
{
    [Export] public SvChange SvChange;

    [Export] public ManiaNoteManager ParentManager;

    [Export] public ManiaNoteSkin NoteSkin;

    [ExportGroup("Objects"), Export] public AnimatedSprite2D Note; // Perhaps it'd be a good idea to make an AnimatedTextureRect?

    [Export] public Control HoldContainer;
    
    [Export] public AnimatedSprite2D Hold;
    
    [Export] public TextureRect TiledHold;

    [Export] public AnimatedSprite2D Tail;

    private double _tailOffset = 0d;
    
    public void Setup(NoteData noteData, SvChange svChange, ManiaNoteManager parentManager, ManiaNoteSkin noteSkin)
    {
        Info = noteData;
        SvChange = svChange;
        ParentManager = parentManager;
        ChangeNoteSkin(noteSkin);
        
        // Reset note
        Active = true;
        Missed = false;
        _tailOffset = 0d;
        
        if (Info.MsLength <= 0)
            return;
        
        AdjustTailSize();
        AdjustTailLength(Info.MsLength);
    }

    public override void _Process(double delta)
    {
        if (!Active || ParentManager == null || !Visible || Info == null)
            return;

        float defaultAlpha = Missed ? 0.5f : 1f;
        Note.SelfModulate = new Color(Note.SelfModulate.R, Note.SelfModulate.G, Note.SelfModulate.B, defaultAlpha);
        
        // Updating position and all that, whatever the base class does.
        base._Process(delta);

        double songPos = Conductor.Time * 1000d;
        bool isHeld = ParentManager.NoteHeld == Info;
        if (Info.MsLength > 0)
        {
            HoldContainer.Rotation = ParentManager.DirectionAngle;

            if (isHeld)
            {
                AdjustTailLength(Info.MsTime + Info.MsLength - songPos);
                Note.Visible = false;
            }
        }

        if (Info.MsTime + Info.MsLength - songPos <= -1000f && !isHeld)
            Active = false;
    }
    
    public override void UpdatePosition()
    {
        float startingPos = ParentManager.ParentBarLine.DistanceOffset;
        float distance = (float)(Info.MsTime - SvChange.MsTime - _tailOffset) * ParentManager.ScrollSpeed;
        Vector2 posMult = new Vector2(Mathf.Cos(ParentManager.DirectionAngle), Mathf.Sin(ParentManager.DirectionAngle));
        Position = ParentManager.NoteHeld == Info ? Vector2.One * (startingPos + distance) * posMult : Vector2.Zero; // TODO: Do holding.
    }

    /// <summary>
    /// Changes the note skin of this note.
    /// </summary>
    /// <param name="noteSkin">The provided note skin.</param>
    public void ChangeNoteSkin(ManiaNoteSkin noteSkin)
    {
        if (NoteSkin == noteSkin)
            return;
        
        NoteSkin = noteSkin;

        int lane = ParentManager.Lane;
        int laneCount = ParentManager.ParentBarLine.Chart.Lanes;
        string direction = noteSkin.GetDirection(lane, laneCount).ToLower();
        
        // Initialize graphic object here, if they're null.
        if (Note == null)
        {
            Note = new AnimatedSprite2D();
            Note.Name = "Note Graphic";
            AddChild(Note);
        }

        if (HoldContainer == null)
        {
            HoldContainer = new Control();
            HoldContainer.Name = "Hold Container";
            AddChild(HoldContainer);
        }
        
        // Tiled hold
        if (noteSkin.UseTiledHold && TiledHold == null)
        {
            TiledHold = new TextureRect();
            TiledHold.Name = "Tiled Hold Graphic";
            TiledHold.StretchMode = TextureRect.StretchModeEnum.Tile;
            HoldContainer.AddChild(TiledHold);
            MoveChild(TiledHold, 0);
        }
        else if (Hold == null) // normal hold
        {
            Hold = new AnimatedSprite2D();
            Hold.Name = "Hold Graphic";
            HoldContainer.AddChild(Hold);
            MoveChild(Hold, 0);
        }
        
        // The tail
        if (Tail == null)
        {
            Tail = new AnimatedSprite2D();
            Tail.Name = "Tail Graphic";
            HoldContainer.AddChild(Tail);
            MoveToFront();
        }
        
        // Do actual note skin graphic setting
        Note.SpriteFrames = noteSkin.NoteAtlas;
        Note.Play($"{direction}NoteNeutral");
        Note.Visible = true;

        HoldContainer.Modulate = new Color(1f, 1f, 1f, 0.5f);
        if (noteSkin.UseTiledHold)
        {
            TiledHold.Texture = noteSkin.GetTiledHold(lane, laneCount);
            TiledHold.Visible = true;

            float holdOffset = TiledHold.Texture.GetHeight() / 2f;
            TiledHold.PivotOffset = new Vector2(Hold.Offset.X, -holdOffset);
            
            if (Hold != null)
                Hold.Visible = false;
        }
        else
        {
            Hold.Centered = false;
            Hold.SpriteFrames = noteSkin.HoldAtlas;
            Hold.Play($"{direction}NoteHold");
            Hold.Visible = true;
            
            // Set offset too
            float holdOffset = Hold.SpriteFrames.GetFrameTexture($"{direction}NoteHold", 0).GetHeight() / 2f;
            Hold.Offset = new Vector2(Hold.Offset.X, -holdOffset);
            
            if (TiledHold != null)
                TiledHold.Visible = false;
        }

        Tail.Centered = false;
        Tail.SpriteFrames = noteSkin.HoldAtlas;
        Tail.Play($"{direction}NoteTail");
        Tail.Visible = true;
        
        float tailOffset = Tail.SpriteFrames.GetFrameTexture($"{direction}NoteTail", 0).GetHeight() / 2f;
        Hold.Offset = new Vector2(Hold.Offset.X, -tailOffset);
    }
    
    public void AdjustTailSize()
    {
        // Rough code, might clean up later if possible
        string direction = NoteSkin.GetDirection(ParentManager.Lane, ParentManager.ParentBarLine.Chart.Lanes).ToLower();
        bool isTiled = NoteSkin.UseTiledHold && TiledHold != null;
        int tailTexWidth = Tail.SpriteFrames.GetFrameTexture($"{direction}NoteTail", Tail.GetFrame()).GetWidth();
        int holdTexWidth = isTiled
            ? TiledHold.Texture.GetWidth()
            : Hold.SpriteFrames.GetFrameTexture($"{direction}NoteHold", Hold.GetFrame()).GetWidth();

        float holdScale = (float)Info.MsLength / holdTexWidth * (ParentManager.ScrollSpeed * SvChange.Multiplier);
        float tailEndSub = tailTexWidth * Tail.Scale.X / (holdTexWidth * Hold.Scale.X);
        float holdWidth = holdTexWidth * Mathf.Max(holdScale - tailEndSub, 0f);

        if (isTiled)
            TiledHold.Size = new Vector2(holdWidth, TiledHold.Size.Y);
        else
            Hold.Scale = new Vector2(Mathf.Max(holdScale - tailEndSub, 0f), Hold.Scale.Y);
    }

    private void AdjustTailLength(double length)
    {
        // Rough code, might clean up later if possible
        string direction = NoteSkin.GetDirection(ParentManager.Lane, ParentManager.ParentBarLine.Chart.Lanes).ToLower();
        bool isTiled = NoteSkin.UseTiledHold && TiledHold != null;
        int tailTexWidth = Tail.SpriteFrames.GetFrameTexture($"{direction}NoteTail", Tail.GetFrame()).GetWidth();
        int holdTexWidth = isTiled
            ? TiledHold.Texture.GetWidth()
            : Hold.SpriteFrames.GetFrameTexture($"{direction}NoteHold", Hold.GetFrame()).GetWidth();

        float holdScale = (float)length / holdTexWidth * (ParentManager.ScrollSpeed * SvChange.Multiplier);
        float tailEndSub = tailTexWidth * Tail.Scale.X / (holdTexWidth * Hold.Scale.X);
        float holdWidth = holdTexWidth * Mathf.Max(holdScale - tailEndSub, 0f);
        float totalHoldWidth = holdTexWidth * Mathf.Max(holdScale, 0f);
            
        HoldContainer.Size = new Vector2(totalHoldWidth, HoldContainer.Size.Y);

        if (isTiled)
        {
            TiledHold.Position = new Vector2(holdWidth - (TiledHold.Scale.X * TiledHold.Size.X), TiledHold.Position.Y);
        }
        else
        {
            float initialHeight = (float)Info.MsLength / holdTexWidth * (ParentManager.ScrollSpeed * SvChange.Multiplier);
            Hold.Position = new Vector2(holdWidth - (Hold.Scale.X * initialHeight), Hold.Position.Y);
        }
        
        Tail.Position = new Vector2(holdWidth, Tail.Position.Y);
    }
    
    /// <summary>
    /// Usually called when the note was let go too early.
    /// </summary>
    public void UnsetHold()
    {
        // Should be based on time, NOT note Y position
        _tailOffset = (Info.MsTime - Conductor.Time * 1000d);
    }
}