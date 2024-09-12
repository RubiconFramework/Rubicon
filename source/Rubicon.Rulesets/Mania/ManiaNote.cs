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
		Position = new Vector2(5000, 0);
		Info = noteData;
		Info.HitObject = this;
		SvChange = svChange;
		ParentManager = parentManager;
		ChangeNoteSkin(noteSkin);
		
		HoldContainer.Visible = Info.MsLength > 0;
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
		Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, defaultAlpha);
		
		// Updating position and all that, whatever the base class does.
		base._Process(delta);

		double songPos = Conductor.Time * 1000d;
		bool isHeld = ParentManager.NoteHeld == Info;
		if (Info.MsLength > 0)
		{
			HoldContainer.Rotation = ParentManager.DirectionAngle;
			
			Vector2 posMult = new Vector2(Mathf.Cos(ParentManager.DirectionAngle), Mathf.Sin(ParentManager.DirectionAngle));
			HoldContainer.Position = -(Vector2.One * HoldContainer.PivotOffset.Y * posMult) * NoteSkin.Scale;
			
			if (isHeld)
			{
				AdjustTailLength(Info.MsTime + Info.MsLength - songPos);
				Note.Visible = false;
			}
		}

		if (Info.MsTime + Info.MsLength - songPos <= -1000f && !isHeld)
		{
			Active = false;
			Visible = false;
		}
	}
	
	public override void UpdatePosition()
	{
		float startingPos = ParentManager.ParentBarLine.DistanceOffset * ParentManager.ScrollSpeed;
		float distance = (float)(Info.MsTime - SvChange.MsTime - _tailOffset) * ParentManager.ScrollSpeed;
		Vector2 posMult = new Vector2(Mathf.Cos(ParentManager.DirectionAngle), Mathf.Sin(ParentManager.DirectionAngle));
		Position = ParentManager.NoteHeld != Info ? (startingPos + distance) * posMult : Vector2.Zero; // TODO: Do holding.
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
			HoldContainer.ClipContents = true;
			AddChild(HoldContainer);
			
			// Test
			/*
			ColorRect thing = new ColorRect();
			HoldContainer.AddChild(thing);
			thing.SetAnchorsPreset(LayoutPreset.FullRect);
			thing.Color = Colors.Lavender;*/
		}
		
		// Tiled hold
		if (noteSkin.UseTiledHold && TiledHold == null)
		{
			TiledHold = new TextureRect();
			TiledHold.Name = "Tiled Hold Graphic";
			TiledHold.StretchMode = TextureRect.StretchModeEnum.Tile;
			HoldContainer.AddChild(TiledHold);
			HoldContainer.MoveChild(TiledHold, 0);
		}
		else if (Hold == null) // normal hold
		{
			Hold = new AnimatedSprite2D();
			Hold.Name = "Hold Graphic";
			HoldContainer.AddChild(Hold);
			HoldContainer.MoveChild(Hold, 0);
		}
		
		// The tail
		if (Tail == null)
		{
			Tail = new AnimatedSprite2D();
			Tail.Name = "Tail Graphic";
			HoldContainer.AddChild(Tail);
			Tail.MoveToFront();
		}
		
		// Do actual note skin graphic setting
		Note.SpriteFrames = noteSkin.NoteAtlas;
		Note.Scale = Vector2.One * NoteSkin.Scale;
		Note.Play($"{direction}NoteNeutral");
		Note.Visible = true;

		HoldContainer.Modulate = new Color(1f, 1f, 1f, 0.5f);
		HoldContainer.Size = new Vector2(0f, NoteSkin.HoldAtlas.GetFrameTexture($"{direction}NoteHold", 0).GetHeight());
		HoldContainer.Scale = NoteSkin.Scale;
		HoldContainer.PivotOffset = new Vector2(0f, HoldContainer.Size.Y / 2f);
		if (noteSkin.UseTiledHold)
		{
			TiledHold.Texture = noteSkin.GetTiledHold(lane, laneCount);
			TiledHold.Visible = true;
			
			if (Hold != null)
				Hold.Visible = false;
		}
		else
		{
			Hold.Centered = false;
			Hold.SpriteFrames = noteSkin.HoldAtlas;
			Hold.Play($"{direction}NoteHold");
			Hold.Visible = true;
			
			if (TiledHold != null)
				TiledHold.Visible = false;
		}

		Tail.Centered = false;
		Tail.SpriteFrames = noteSkin.HoldAtlas;
		Tail.Play($"{direction}NoteTail");
		Tail.Visible = true;
	}
	
	public void AdjustTailSize()
	{
		// Rough code, might clean up later if possible
		string direction = ParentManager.Direction;
		bool isTiled = NoteSkin.UseTiledHold && TiledHold != null;
		int tailTexWidth = Tail.SpriteFrames.GetFrameTexture($"{direction}NoteTail", Tail.GetFrame()).GetWidth();
		int holdTexWidth = isTiled
			? TiledHold.Texture.GetWidth()
			: Hold.SpriteFrames.GetFrameTexture($"{direction}NoteHold", Hold.GetFrame()).GetWidth();

		float holdWidth = (float)(Info.MsLength * ParentManager.ScrollSpeed * SvChange.Multiplier);
		if (isTiled)
			TiledHold.Size = new Vector2((holdWidth - tailTexWidth) / HoldContainer.Scale.X, TiledHold.Size.Y);
		else
			Hold.Scale = new Vector2(Mathf.Max(((holdWidth / holdTexWidth) - ((float)tailTexWidth / holdTexWidth)) / HoldContainer.Scale.X, 0f), Hold.Scale.Y);
		
		if (ParentManager.NoteHeld != Info)
			AdjustTailLength(Info.MsLength);
	}

	public void AdjustTailLength(double length)
	{
		// Rough code, might clean up later if possible
		string direction = ParentManager.Direction;
		bool isTiled = NoteSkin.UseTiledHold && TiledHold != null;
		float initialHoldWidth = (float)(Info.MsLength * ParentManager.ScrollSpeed * SvChange.Multiplier);
		float holdWidth = (float)(length * ParentManager.ScrollSpeed * SvChange.Multiplier);
		
		HoldContainer.Size = new Vector2(holdWidth / HoldContainer.Scale.X, HoldContainer.Size.Y);
		float holdPos = HoldContainer.Size.X - (initialHoldWidth / HoldContainer.Scale.X);
		float holdHeight = 0f;
		if (isTiled)
		{
			TiledHold.Position = new Vector2(holdPos, TiledHold.Position.Y);
			holdHeight = TiledHold.Texture.GetHeight();
		}
		else
		{
			Hold.Position = new Vector2(holdPos, Hold.Position.Y);
			holdHeight = Hold.SpriteFrames.GetFrameTexture($"{direction}NoteHold", Hold.GetFrame()).GetHeight();
		}

		Texture2D tailFrame = Tail.SpriteFrames.GetFrameTexture($"{direction}NoteTail", Tail.GetFrame());
		Vector2 tailTexSize = tailFrame.GetSize();
		Tail.Position = new Vector2((initialHoldWidth - tailTexSize.X) / HoldContainer.Scale.X + holdPos, holdHeight - tailTexSize.Y);
	}
	
	/// <summary>
	/// Usually called when the note was let go too early.
	/// </summary>
	public void UnsetHold()
	{
		// Should be based on time, NOT note Y position
		_tailOffset = (Info.MsTime - Conductor.Time * 1000d);
	}

	public override void Reset()
	{
		base.Reset();
		Note.Visible = true;
		_tailOffset = 0d;
	}
}
