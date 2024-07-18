using Godot.Collections;
using Rubicon.backend.autoload;
using Rubicon.gameplay.classes.strums;
using Rubicon.gameplay.resources;

namespace Rubicon.gameplay.classes.notes;
public partial class Note : Node2D
{
	[Export] public bool ShouldHit = true;
    public bool PlayerNote = false;
    public bool TimeToHit = false;
    public bool WasHit = false;
    public bool WasMissed = false;

    [Export] public bool AltAnim = false;
    public string NoteType = "default";
    public string RawType = "default";
    public UIStyle uiStyle;

    [Export] public float HealthGain = 1;
    [Export] public float HealthLoss = 1;
    [Export] public float HitWindowMult = 1;

    public float Time = 0;
    public float HitTime = 0;
    public int Direction = 0;
    public float ScrollSpeed = 1;
    public float SustainLength = 0;
    public float InitialLength = 0;
    public float HoldTime = 0;
    public bool IsSustainNote = false;
    public int DownscrollMultiplier = 1;

    public StrumLine CurrentStrumline;
    [Export] public bool AttachToStrumline = true;
    [Export] public bool ShouldSplash = true;
    public bool HasCustomSplash = false;
    [Export] public SpriteFrames SustainTexture { get; set; } = GD.Load<SpriteFrames>("res://assets/gameplay/noteTypes/assets/default/defaultSustain.tres");
    [Export] public SpriteFrames SustainSplashTexture { get; set; } = null;
    [Export] public Dictionary<string,Vector2> SustainSplashOffset { get; set; }
    [Export] public SpriteFrames SplashTexture { get; set; } = null;
    [Export] public int SplashAnimRange = 1;

    [NodePath("Sprite")] public AnimatedSprite2D Sprite;
    [NodePath("SustainMask")] public TextureRect SustainMask;
    [NodePath("SustainMask/SustainPiece")] public Line2D SustainPiece;
    [NodePath("SustainMask/SustainPiece/SustainEnd")] public Sprite2D SustainEnd;

    public override void _Ready() 
    {
        base._Ready();
        this.OnReady();

        string DirectionName = CurrentStrumline.GetChildren<Strum>()[Direction].Name;

        Sprite.Play(DirectionName);
        SustainPiece.Texture = SustainTexture.GetFrameTexture($"{DirectionName}",0);
        SustainEnd.Texture = SustainTexture.GetFrameTexture($"{DirectionName}",1);

        DownscrollMultiplier = RubiconSettings.Gameplay.Downscroll ? -1 : 1;
    
        if(SustainLength < 50) SustainMask.QueueFree();
        SustainMask.Size = new Vector2(SustainMask.Size.X, Main.WindowSize.Y / Scale.Y);
        InitialLength = SustainLength;
    }

    public override void _Process(double delta)
    {
        if(WasMissed)
            Modulate = new Color(0.5f,0.5f,0.5f,0.5f);

        if(InitialLength > 50 && SustainLength > 0) {
            SustainMask.Position = new Vector2(SustainMask.Position.X,0);
            SustainPiece.Position = new Vector2(SustainPiece.Position.X,0);
            if(DownscrollMultiplier < 0) {
                SustainMask.Position = new Vector2(SustainMask.Position.X,-SustainMask.Size.Y);
                SustainPiece.Position = new Vector2(SustainPiece.Position.X,SustainMask.Size.Y);
            }

            int ArrayLength = SustainPiece.Points.Length-1;
            SustainPiece.SetPointPosition(ArrayLength, new Vector2(SustainPiece.Points[ArrayLength].X, HitTime+SustainLength/2.5f*(ScrollSpeed/Scale.Y)*DownscrollMultiplier));

           /*foreach(Vector2 point in SustainPiece.Points) {
                int pointIdx = Array.IndexOf(SustainPiece.Points,point);
                if(pointIdx == 0 || point == SustainPiece.Points[ArrayLength])
                    SustainPiece.SetPointPosition(pointIdx, new Vector2(point.X,SustainPiece.Points[ArrayLength].Y * (1 / SustainPiece.Points.Length) * pointIdx));
            }*/

            SustainEnd.Position = new Vector2(SustainEnd.Position.X, SustainPiece.Points[ArrayLength].Y + SustainEnd.Texture.GetHeight() * SustainEnd.Scale.Y * 0.5f * DownscrollMultiplier);
            SustainEnd.FlipV = DownscrollMultiplier < 0;
        }
        if(IsSustainNote && SustainLength <= 0)
            Visible = false;
    }
}
