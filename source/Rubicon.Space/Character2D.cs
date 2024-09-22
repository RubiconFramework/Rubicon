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
    /// It flips the left and right animations depending if <paramref name="MirrorCharacter"/> does not equal <paramref name="IsPlayer"/>.
    /// </summary>
    public bool FlipAnimations = false;

    /// <summary>
    /// Determines whether the character should be rotated or not depending on <paramref name="MirrorCharacter"/>.
    /// </summary>
    public bool IsPlayer = false;

    /// <summary>
    /// This property gets added to the start of an animation's name.
    /// It is overriden by the <paramref name="Prefix"/> parameter inside <paramref name="CurrentAnimation"/>.
    /// Useful for alt animations or similar.
    /// </summary>
    [ExportGroup("Animation Info"), Export] public string StaticPrefix;

    /// <summary>
    /// This property gets added to the end of an animation's name.
    /// It is overriden by the <paramref name="Suffix"/> parameter inside <paramref name="CurrentAnimation"/>.
    /// Useful for alt animations or similar.
    /// </summary>
    [Export] public string StaticSuffix;
    
    /// <summary>
    /// A string array containing the sequence of idle/dance animations to be played.
    /// Useful for left to right dance animations.
    /// </summary>
    [Export] public string[] DanceList = {"idle"};

    /// <summary>
    /// The index for which dance animation in the <paramref name="DanceList"/> should play.
    /// </summary>
    private int DanceIndex = 0;

    /// <summary>
    /// If <see langword="true"/>, the character will jitter when holding a note. If <see langword="false"/>, it will stay completely static.
    /// </summary>
    [Export] public bool StaticSustain = false;

    /// <summary>
    /// Currently playing <see cref="CharacterAnimation"/>.
    /// </summary>
    public CharacterAnimation CurrentAnim = new();

    /// <summary>
    /// Last played <paramref name="CharacterAnimation"/>.
    /// </summary>
	public CharacterAnimation LastAnim = new();

    /// <summary>
    /// The duration of the sing animations before going back to idle.
    /// </summary>
    [Export] public float SingDuration = 4;

    /// <summary>
    /// A timer that determines if the animation should be finished or not.
    /// </summary>
    public float SingTimer;

    /// <summary>
    /// A timer that determines if the hold animation should be repeated or not.
    /// It will not be used in case <paramref name="StaticSustain"/> is <see langword="true"/>.
    /// </summary>
    public float HoldTimer;

    /// <summary>
    /// The <paramref name="SpriteFrames"/> used for the healthbar icons.
    /// It has to contain an idle animation.
    /// It can contain "lose" and "win" optionally.
    /// </summary>
    [ExportGroup("Healthbar Info"), Export] public SpriteFrames CharacterIcon { get; set; } = GD.Load<SpriteFrames>("res://Assets/Characters/Placeholder/Icon.tres");

    /// <summary>
    /// The offset of the healthbar icon.
    /// </summary>
    [Export] public Vector2 IconOffset = new Vector2(0,10);

    /// <summary>
    /// The healthbar color of this character.
    /// </summary>
    [Export] public Color HealthColor = new("#A1A1A1");

    /// <summary>
    /// The main <paramref name="Node2D"/> used for positioning and scaling.
    /// Usually its the main <paramref name="PlayerSprite2D"/> node.
    /// </summary>
    [ExportGroup("Path Info"), Export] public Node2D MainNode;

    /// <summary>
    /// The main <see cref="AnimationPlayer"> node used for playing animations.
    /// </summary>
    [Export] public AnimationPlayer AnimPlayer;

    /// <summary>
    /// A <see cref="Marker2D"/> from which the camera takes its position.
    /// </summary>
    [NodePath("CamPoint")] public Marker2D CameraPoint;

    public override void _Ready()
    {
        base._Ready();
        this.OnReady();

        AnimPlayer.AnimationFinished += AnimationFinished;
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

    public void AnimationFinished(StringName anim) 
    {
        CurrentAnim.AnimFinished = true;

        if(CurrentAnim.PostAnimation != null)
            PlayAnim(CurrentAnim.PostAnimation);
    }

    private static string FlipAnim(string anim)
    {
		string newAnim = anim.Contains("LEFT") ? anim.Replace("LEFT", "RIGHT") : anim.Replace("RIGHT", "LEFT");
        return newAnim;
    }
}
