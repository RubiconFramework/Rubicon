using Godot.Sharp.Extras;

namespace Rubicon.gameplay.objects.classes.scripts;

public partial class Character2D : Node2D
{
    [ExportGroup("General")] 
    [Export] private bool mirrorCharacter;
    [Export] public string[] danceAnim = { "idle" };

    [ExportGroup("HealthBar Icon")]
    [Export] public Texture2D iconTexture { get; set; } = GD.Load<Texture2D>("res://assets/gameplay/characters/icons/iconFace.png");
    [Export] public int iconAnimations = 2;
    [Export] public Color healthColor = new("#A1A1A1");

    [ExportGroup("Advanced")] [Export] public string prefix = "";
    [Export] public bool staticSustains;
    [Export] public float singDuration = 4;
    [Export] public bool danceOnBeat = true;
    [Export] private NodePath spritePath = "AnimatedSprite";
    [Export] private NodePath playerPath = "AnimatedSprite/AnimationPlayer";

    public dynamic sprite;
    public AnimationPlayer animPlayer;

    [NodePath("CameraPos")] public Node2D camPos;

    public bool isPlayer = false;
    public bool specialAnim;
    public float animTimer;
    public string lastAnim = "_";
    private int danceStep;
    public float holdTimer;
    public bool animFinished;

    public override void _Ready()
    {
        sprite = GetNode(spritePath);
        animPlayer = GetNode<AnimationPlayer>(playerPath);
        
        animPlayer.AnimationFinished += name => animFinished = true;
        dance(true);

        if (mirrorCharacter != isPlayer) Scale *= new Vector2(-1,1);
    }
    public Vector2 getCamPos() => camPos.GlobalPosition;

    public override void _Process(double delta)
    {
        if(animTimer > 0)
        {
            animTimer -= (float)delta;
            if (animTimer <= 0 && (specialAnim && (lastAnim == "cheer" || lastAnim == "hey")))
            {
                specialAnim = false;
                dance();
            }
        }
        else if (specialAnim && animFinished)
        {
            specialAnim = false;
            dance();
        }

        if (lastAnim.StartsWith("sing"))
        {
            holdTimer += (float)delta * Conductor.Instance.rate;
            if(!isPlayer && holdTimer >= Conductor.Instance.stepCrochet * singDuration * 0.0011)
            {
                holdTimer = 0;
                dance();
            }
        }
    }

    public void playAnim(string anim, bool force = false)
    {
        specialAnim = false;

        if (isPlayer != mirrorCharacter) anim = flipAnim(anim);
        if (anim.EndsWith("-alt") && (!animPlayer.HasAnimation(anim) || prefix != "")) anim.Replace("-alt", "");

        if (!animPlayer.HasAnimation(anim))
        {
            GD.PushWarning("Animation " + anim + " not found.");
            return;
        }

        if (force || animFinished)
        {
            if (lastAnim == anim) animPlayer.Seek(0.0);
            lastAnim = anim;
            animFinished = false;

            string finalAnim = anim;
            if (animPlayer.HasAnimation(anim + prefix)) finalAnim += prefix;

            animPlayer.Play(finalAnim);
        }
    }

    private static string flipAnim(string anim)
    {
        if (anim.Contains("singLEFT")) anim = "singRIGHT";
        else if (anim.Contains("singRIGHT")) anim = "singLEFT";

        return anim;
    }

    public void dance(bool force = false)
    {
        if (!force && specialAnim) return;

        playAnim(danceAnim[danceStep]);

        danceStep++;
        if (danceStep > danceAnim.Length - 1) danceStep = 0;
    }
}
