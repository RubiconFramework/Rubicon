using Godot;
using Godot.Sharp.Extras;

public partial class Character2D : Node2D
{
    /// <summary>
    /// 
    /// This is the character class, which handles which animations the sprite should play
    /// among other utils
    /// 
    /// </summary>

    [ExportGroup("Character Info")]

    // This should be true if the character is looking to the left
    // It rotates the sprites and animations in case the character is a player
    [Export] public bool MirrorCharacter = false;

    // This two properties is automatically determined by the engine
    public bool FlipAnimations = false;
    public bool IsPlayer = false;


    [ExportGroup("Animation Info")]

    // This two properties are added to the back and front of the animation name
    // They are overriden by the ones in the CharacterAnimation being played
    // Useful for alt animations or similar
    [Export] public string StaticPrefix;
    [Export] public string StaticSuffix;
    
    // This array contains the "idle" or "dance" animation sequence
    // Every beat that is played goes to the next one on the array
    // Useful for left and right animations like Gf
    [Export] public string[] DanceList = {"idle"};
    public int DanceStep = 0;

    // Whether the characters should jitter when holding notes or be completely still
    [Export] public bool StaticSustain = false;

    // Currently playing animation and last played animation
    public CharacterAnimation CurrentAnim = new();
	public CharacterAnimation LastAnim = new();

    // Timers to determine when to repeat or finish sing animations
    [Export] public float SingDuration = 4;
    public float SingTimer;
    public float HoldTimer;
    

    [ExportGroup("Healthbar Info")]

    // A SpriteFrames resource that should contain an "idle" and "lose" animation
    [Export] public SpriteFrames CharacterIcon { get; set; } = GD.Load<SpriteFrames>("res://assets/characters/placeholder/icon.tres");
    [Export] public Vector2 IconOffset = new Vector2(0,10);
    [Export] public Color HealthColor = new("#A1A1A1");


    [ExportGroup("Path Info")]
    [Export] public Node MainNode;
    [Export] public AnimationPlayer AnimPlayer;
    [NodePath("CamPoint")] public Marker2D CameraPoint;

    public override void _Ready()
    {
        base._Ready();
        this.OnReady();

        AnimPlayer.AnimationFinished += (StringName name) => CurrentAnim.AnimFinished = true;
        FlipAnimations = IsPlayer != MirrorCharacter;
		if (IsPlayer != MirrorCharacter) Scale *= new Vector2(-1,1);
    }

    public void PlayAnim(CharacterAnimation anim)
	{
		if(!(anim.Force || !CurrentAnim.OverrideAnim || CurrentAnim.AnimFinished)) return;

		if (FlipAnimations) anim.AnimName = FlipAnim(anim.AnimName);

		string FinalPrefix = anim.Prefix != "" ? anim.Prefix : StaticPrefix;
		string FinalSuffix = anim.Suffix != "" ? anim.Suffix : StaticSuffix;

		if (AnimPlayer.HasAnimation(FinalPrefix+anim.AnimName+FinalSuffix))
			anim.AnimName = FinalPrefix+anim.AnimName+FinalSuffix;
		
		if(!AnimPlayer.HasAnimation(anim.AnimName) && anim.AnimName != "" && !anim.AnimName.EndsWith("miss"))
		{
			GD.PushWarning($"There is no animation called {anim.AnimName}");
			return;
		}

		if (CurrentAnim.AnimName == anim.AnimName) AnimPlayer.Seek(0);
		LastAnim = CurrentAnim;
		CurrentAnim = anim;
		AnimPlayer.Play(anim.AnimName);
	}

	private static string FlipAnim(string anim)
    {
		string newAnim = anim.Contains("LEFT")
		? anim.Replace("LEFT", "RIGHT")
		: anim.Replace("RIGHT", "LEFT");

        return newAnim;
    }
}
