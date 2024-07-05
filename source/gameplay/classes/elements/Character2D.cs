using FNFGodot.Backend.Autoload;

namespace FNFGodot.Gameplay.Classes.Elements;

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
	[Export] public string Prefix;
	[Export] public string Suffix;
	[Export] public string[] DanceList = {"idle"};
	[Export] public float SingDuration = 4;
	[Export] public bool StaticSustain = false;
	private bool AnimFinished;
	public string LastAnim = "";
	public bool OverrideAnim = false;
	public bool ShouldDance = true;

	[ExportGroup("Character Info")]
	[Export] public bool MirrorCharacter = false;
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

	private void AnimationFinished(StringName AnimName)
	{
		AnimFinished = true;
		OverrideAnim = false;
	}

    public override void _PhysicsProcess(double delta)
    {
		if(LastAnim.StartsWith("sing"))
		{
			SingTimer += (float)delta;
			if(!IsPlayer && SingTimer >= Conductor.StepDuration * (SingDuration * 0.0011)) {
				SingTimer = 0;
				Dance(true);
			}
		}
    }

	private int DanceStep = 0;
	public void Dance(bool Force = false)
	{
		if(!Force && !AnimFinished || OverrideAnim) return;
		if(LastAnim.StartsWith("sing") && !Force) return; 

		PlayAnim(DanceList[DanceStep]);

		DanceStep++;
		if (DanceStep > DanceList.Length-1) DanceStep = 0;
	}

	public void PlayAnim(string AnimName, bool Force = false)
	{
		if(!(Force || !OverrideAnim || AnimFinished)) return;

		if (FlipAnimations) AnimName = FlipAnim(AnimName);

		if (AnimPlayer.HasAnimation(Prefix+AnimName+Suffix))
			AnimName = Prefix+AnimName+Suffix;
		
		if(!AnimPlayer.HasAnimation(AnimName) && AnimName != "" && !AnimName.EndsWith("miss"))
		{
			GD.PushWarning($"There is no animation called {AnimName}");
			return;
		}

		if (LastAnim == AnimName) AnimPlayer.Seek(0);
		AnimFinished = false;
		LastAnim = AnimName;
		AnimPlayer.Play(AnimName);
	}

	private static string FlipAnim(string anim)
    {
        if (anim.Contains("singLEFT")) anim = "singRIGHT";
        else if (anim.Contains("singRIGHT")) anim = "singLEFT";

        return anim;
    }
}