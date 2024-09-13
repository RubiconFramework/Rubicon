using Rubicon.Core;
using Rubicon.Core.Chart;

namespace Rubicon.Rulesets.Mania;

public partial class ManiaNote : Note
{
	/// <summary>
	/// The parent <see cref="ManiaNoteManager"/>.
	/// </summary>
	[Export] public ManiaNoteManager ParentManager;

	/// <summary>
	/// The note skin associated with this note.
	/// </summary>
	[Export] public ManiaNoteSkin NoteSkin;

	/// <summary>
	/// The Note graphic for this note.
	/// </summary>
	public AnimatedSprite2D Note; // Perhaps it'd be a good idea to make an AnimatedTextureRect?

	/// <summary>
	/// The hold control that contains everything related to the hold graphics.
	/// </summary>
	public Control HoldContainer;
	
	/// <summary>
	/// The Hold graphic. Will not be used if <see cref="ManiaNoteSkin.UseTiledHold"/> is active.
	/// </summary>
	public AnimatedSprite2D Hold;
	
	/// <summary>
	/// The Hold graphic, for tiling. Will be used if <see cref="ManiaNoteSkin.UseTiledHold"/> is active.
	/// </summary>
	public TextureRect TiledHold;

	/// <summary>
	/// The Tail graphic for this note.
	/// </summary>
	public AnimatedSprite2D Tail;

	private double _tailOffset = 0d;
	
	/// <summary>
	/// Sets up this hit object for usage alongside a <see cref="ManiaNoteManager"/>.
	/// </summary>
	/// <param name="noteData">The note data</param>
	/// <param name="svChange">The scroll velocity change associated</param>
	/// <param name="parentManager">The parent manager</param>
	/// <param name="noteSkin">The note skin</param>
	public void Setup(NoteData noteData, ManiaNoteManager parentManager, ManiaNoteSkin noteSkin)
	{
		Position = new Vector2(5000, 0);
		Info = noteData;
		Info.HitObject = this;
		ParentManager = parentManager;
		ChangeNoteSkin(noteSkin);
		
		HoldContainer.Visible = Info.MsLength > 0;
		if (Info.MsLength <= 0)
			return;
		
		AdjustInitialTailSize();
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
	
	/// <inheritdoc/>
	public override void UpdatePosition()
	{
		float startingPos = ParentManager.ParentBarLine.DistanceOffset * ParentManager.ScrollSpeed;
		SvChange svChange = ParentManager.ParentBarLine.Chart.SvChanges[Info.StartingScrollVelocity];
		float distance = (float)(svChange.Position + Info.MsTime - svChange.MsTime - _tailOffset) * ParentManager.ScrollSpeed;
		Vector2 posMult = new Vector2(Mathf.Cos(ParentManager.DirectionAngle), Mathf.Sin(ParentManager.DirectionAngle));
		Position = ParentManager.NoteHeld != Info ? (startingPos + distance) * posMult : Vector2.Zero;
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
		HoldContainer.Position = new Vector2(0f, -HoldContainer.Size.Y / 2f);
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
	
	/// <summary>
	/// Resizes the hold's initial size to match the scroll speed and scroll velocities.
	/// </summary>
	public void AdjustInitialTailSize()
	{
		// Rough code, might clean up later if possible
		string direction = ParentManager.Direction;
		bool isTiled = NoteSkin.UseTiledHold && TiledHold != null;
		int tailTexWidth = Tail.SpriteFrames.GetFrameTexture($"{direction}NoteTail", Tail.GetFrame()).GetWidth();
		int holdTexWidth = isTiled
			? TiledHold.Texture.GetWidth()
			: Hold.SpriteFrames.GetFrameTexture($"{direction}NoteHold", Hold.GetFrame()).GetWidth();

		float holdWidth = GetOnScreenHoldLength(Info.MsLength) * ParentManager.ScrollSpeed;
		if (isTiled)
			TiledHold.Size = new Vector2((holdWidth - tailTexWidth) / HoldContainer.Scale.X, TiledHold.Size.Y);
		else
			Hold.Scale = new Vector2(Mathf.Max(((holdWidth / holdTexWidth) - ((float)tailTexWidth / holdTexWidth)) / HoldContainer.Scale.X, 0f), Hold.Scale.Y);
		
		if (ParentManager.NoteHeld != Info)
			AdjustTailLength(Info.MsLength);
	}

	/// <summary>
	/// Resizes the entire hold in general according to the length provided.
	/// </summary>
	public void AdjustTailLength(double length)
	{
		// Rough code, might clean up later if possible
		string direction = ParentManager.Direction;
		bool isTiled = NoteSkin.UseTiledHold && TiledHold != null;
		float initialHoldWidth = GetOnScreenHoldLength(Info.MsLength) * ParentManager.ScrollSpeed;
		float holdWidth = GetOnScreenHoldLength(length) * ParentManager.ScrollSpeed;
		
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

	/// <inheritdoc/>
	public override void Reset()
	{
		base.Reset();
		Note.Visible = true;
		_tailOffset = 0d;
	}
	
	/// <summary>
	/// Gets the on-screen length of the hold note
	/// </summary>
	/// <param name="length">The current length of the note</param>
	/// <returns>The on-screen length</returns>
	private float GetOnScreenHoldLength(double length)
	{
		SvChange[] svChangeList = ParentManager.ParentBarLine.Chart.SvChanges;
		double startTime = Info.MsTime + (Info.MsLength - length);
		int startIndex = Info.StartingScrollVelocity;
		for (int i = startIndex; i <= Info.EndingScrollVelocity; i++)
		{
			if (svChangeList[i].MsTime > startTime)
				break;
			
			startIndex = i;
		}
		
		SvChange startingSvChange = svChangeList[startIndex];
		double startingPosition = startingSvChange.Position + ((startTime - startingSvChange.MsTime) * startingSvChange.Multiplier);
		double endingPosition = svChangeList[Info.EndingScrollVelocity].Position +
			((Info.MsTime + Info.MsLength - svChangeList[Info.EndingScrollVelocity].MsTime) * svChangeList[Info.EndingScrollVelocity].Multiplier);
		/*
		for (int i = startIndex; i < Info.EndingScrollVelocity; i++)
		{
			endingPosition += svChangeList[i].Position - endingPosition;
			GD.Print("does this even run");
		}
		*/

		//endingPosition = svChangeList[Info.EndingScrollVelocity].MsTime + ((Info.MsTime + Info.MsLength) * svChangeList[Info.EndingScrollVelocity].Multiplier);

		return (float)(endingPosition - startingPosition);
	}
}
