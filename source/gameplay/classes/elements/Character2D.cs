using Rubicon.backend.autoload;

namespace Rubicon.gameplay.classes.elements;

public partial class Character2D : Node2D
{
	/*	FNF character class.
		The main difference between Character2D and Character3D is the inheritor,
		as you obviously couldn't inherit Node3D on a 2D class and vice versa.	*/

	[ExportGroup("HealthBar Info")]
	[Export] public SpriteFrames CharacterIcon { get; set; } = GD.Load<SpriteFrames>("res://assets/gameplay/characters/assets/placeholder/icon.tres");
	[Export] public Color HealthColor = new("#A1A1A1");
	[Export] public Vector2 IconOffset = new Vector2(0,10);
	
	[ExportGroup("Animation Info")]
	[Export] public string StaticPrefix;
	[Export] public string StaticSuffix;
	[Export] public string[] DanceList = {"idle"};
	[Export] public float SingDuration = 4;
	[Export] public bool StaticSustain;
	public CharacterAnimation CurrentAnim = new();
	public CharacterAnimation LastAnim = new();
	public bool ShouldDance = true;

	[ExportGroup("Character Info")]
	[Export] public bool MirrorCharacter;
	public bool FlipAnimations;
	public bool IsPlayer = false;
	public float SingTimer;
	public float HoldAnimTimer = 0;

	[ExportGroup("Path Info")]
	[Export] public Node MainObject;
	[Export] public AnimationPlayer AnimPlayer;
	
	public override void _Ready()
	{
		base._Ready();
		this.OnReady();

		AnimPlayer.AnimationFinished += AnimationFinished;
		if (IsPlayer != MirrorCharacter) Scale *= new Vector2(-1,1);

		FlipAnimations = IsPlayer != MirrorCharacter;
	}	

	private void AnimationFinished(StringName AnimName) => CurrentAnim.AnimFinished = true;

    public override void _PhysicsProcess(double delta)
    {
		if(CurrentAnim != null && CurrentAnim.AnimName.StartsWith("sing"))
		{
			SingTimer += (float)delta;
			if(!IsPlayer && SingTimer >= Conductor.StepDuration * (SingDuration * 0.0011)) {
				SingTimer = 0;
				Dance(true);
			}
		}
    }

	private int DanceStep;
	public void Dance(bool Force = false)
	{
		if(!Force && !CurrentAnim.AnimFinished || CurrentAnim.OverrideDance && !Force) return;
		if(CurrentAnim.AnimName.StartsWith("sing") && !Force) return; 

		PlayAnimByString(DanceList[DanceStep], true);

		DanceStep++;
		if (DanceStep > DanceList.Length-1) DanceStep = 0;
	}

	public void PlayAnimByString(string anim, bool force = false)
	{
		CharacterAnimation newAnim = new()
		{
			AnimName = anim,
			Force = force
		};

		PlayAnim(newAnim);
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

public class CharacterAnimation
{
	public string AnimName = "idle";
	public bool Force;
	public bool OverrideDance = false;
	public bool OverrideAnim = false;
	public bool AnimFinished;
	
	// these override the static ones.
	public string Prefix;
	public string Suffix;
}