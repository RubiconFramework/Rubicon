using Godot.Sharp.Extras;

namespace Rubicon.Space;

/// <summary>
/// This is the character class, which handles which animations the sprite should play
/// among other utils
/// </summary>
public partial class Character2D : Node2D
{
    /// <summary>
    /// Determines whether the character should be rotated or not.
    /// Recommended when a character's sprite is facing left.
    /// </summary>
    [ExportGroup("Character Info"), Export] public bool MirrorCharacter = false;

    /// <summary>
    /// This property is automatically determined by the character class.
    /// It flips the left and right animations depending if MirrorCharacter does not equal IsPlayer.
    /// </summary>
    public bool FlipAnimations = false;

    /// <summary>
    /// Determines whether the character should be rotated or not depending on MirrorCharacter.
    /// </summary>
    public bool IsPlayer = false;

    /// <summary>
    /// This property gets added to the start of an animation's name.
    /// It is overriden by the Prefix property inside CurrentAnimation.
    /// Useful for alt animations or similar.
    /// </summary>
    [ExportGroup("Animation Info"), Export] public string StaticPrefix;

    /// <summary>
    /// This property gets added to the end of an animation's name.
    /// It is overriden by the Suffix property inside CurrentAnimation.
    /// Useful for alt animations or similar.
    /// </summary>
    [Export] public string StaticSuffix;
    
    // This array contains the "idle" or "dance" animation sequence
    // Every beat that is played goes to the next one on the array
    // Useful for left and right animations like Gf
    /// <summary>
    /// A string array containing the sequence of idle/dance animations to be played.
    /// Useful for 
    /// </summary>
    [Export] public string[] DanceList = {"idle"};
    private int DanceIndex = 0;

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

    // A SpriteFrames resource that should contain an "idle" animation and optionally, "lose" and "win"
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
		if(!(anim.Force || !CurrentAnim.OverrideAnim || CurrentAnim.AnimFinished) || (!anim.Force && (!anim.OverrideDance && CurrentAnim.IsDanceAnimation))) return;

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

    public void Dance(bool Force = false)
    {
        if (!Force && !CurrentAnim.AnimFinished || !CurrentAnim.AnimFinished && CurrentAnim.OverrideDance) return;
        if (CurrentAnim.AnimName.StartsWith("sing") && !Force) return;

        PlayAnimByString(DanceList[DanceIndex], true);

        DanceIndex++;
        DanceIndex = Mathf.Wrap(DanceIndex, 0, DanceList.Length-1);
    }

    public void PlayAnimByString(string anim, bool force = false)
    {
        CharacterAnimation newAnim = new()
        {
            AnimName = anim,
            Force = force,
            IsDanceAnimation = true
        };

        PlayAnim(newAnim);
    }

    private static string FlipAnim(string anim)
    {
		string newAnim = anim.Contains("LEFT") ? anim.Replace("LEFT", "RIGHT") : anim.Replace("RIGHT", "LEFT");
        return newAnim;
    }
}
