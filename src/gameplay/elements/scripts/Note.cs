using BaseRubicon.Gameplay.Elements.Resources;
using BaseRubicon.Gameplay.Elements.StrumLines;
using Global = BaseRubicon.Backend.Autoload.Global;

namespace BaseRubicon.Gameplay.Elements.Scripts;

public partial class Note : Node2D
{
    [Export] public float HealthGain = 1f;
    [Export] public bool ShouldHit = true;
    [Export] public bool ApplyUIStyle = true;
    [Export] public float HitWindowMult = 0.75f;

    public float Time = 0f;
    public int Direction = 0;
    public float Length;

    public string Type = "Default";
    public bool AltAnim = false;

    public float OriginalLength;
    public bool IsSustainNote = false;

    public bool MustPress = false, CanBeHit = false, WasGoodHit = false, TooLate = false, Independent = false;
    private float StepCrochet;

    [NodePath("../../../")] private GameplayScene GameplayScene;
    [NodePath("Sprite")] private AnimatedSprite2D Sprite;
    [NodePath("SustainLine")] private Line2D Sustain;
    [NodePath("SustainLine/SustainEnd")] private Sprite2D SustainEnd;

    private Vector2 InitialScale;
    public StrumLine Strumline;

    public override void _Ready()
    {
        this.OnReady();
        
        if (Length < 50) Length = 0;
        if (Length <= 0) Sustain.Visible = false;

        StepCrochet = Conductor.Instance.stepCrochet;
        OriginalLength = Length;
        InitialScale = Scale;
        Sprite.Play(Global.defaultNoteDirections[Direction]);
    }

    public void loadUIStyle(UIStyle style)
    {
        Sprite.SpriteFrames = style.noteTexture;
        Sustain.Texture = style.sustainTexture.GetFrameTexture($"{Global.defaultNoteDirections[Direction]} hold piece",0);
        SustainEnd.Texture = style.sustainTexture.GetFrameTexture($"{Global.defaultNoteDirections[Direction]} hold end",0);

        Scale = Strumline.GetChild<Strum>(0).Scale;
        Sustain.Width /= (Scale.X + 0.3f);
    }    
}