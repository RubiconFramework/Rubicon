namespace FNFGodot.Gameplay.Classes.Elements;

public partial class Character3D : Node3D
{
	/*	Character2D all over again	*/

	[ExportGroup("HealthBar Info")]
	[Export] public Texture2D CharacterIcon;
	[Export] public Color HealthColor = new("#A1A1A1");
	[Export] public string[] IconAnimations = {"normal","lose"};
	
	[ExportGroup("Animation Info")]
	[Export] public bool ShouldDance = true;
	[Export] public string Prefix;
	[Export] public string Suffix;
	[Export] private string[] DanceList = {"idle"};
	[Export] public float SingDuration = 4;
	[Export] public bool StaticSustain = false;
	private bool AnimFinished;
	public string LastAnim = "";
	public bool OverrideAnim = false;
	public float AnimTimer;

	[ExportGroup("Character Info")]
	[Export] public bool MirrorCharacter = false;
	public bool IsPlayer = false;
	public float HoldTimer;

	[ExportGroup("Path Info")]
	[Export] NodePath MainObjectPath = "Character";
	[Export] NodePath AnimPlayerPath = "AnimationPlayer";
	public dynamic MainObject;
	public AnimationPlayer AnimPlayer;
	[NodePath("Sprite/Camera")] public Node3D Camera;
	
	public override void _Ready()
	{
		this.OnReady();
		MainObject = GetNode<dynamic>(MainObjectPath);
		AnimPlayer = GetNode<AnimationPlayer>(AnimPlayerPath);

		AnimPlayer.AnimationFinished += AnimationFinished;
	}	

	private void AnimationFinished(StringName AnimName)
	{
		LastAnim = AnimName;
		AnimFinished = true;
		OverrideAnim = false;
	}

    public override void _Process(double delta)
    {
        if(AnimTimer > 0)
			AnimTimer -= (float)delta;
		if(LastAnim.StartsWith("sing"))
		{
			HoldTimer += (float)delta;
			if(!IsPlayer && HoldTimer >= SingDuration * 0.0011)
				HoldTimer = 0; Dance();
		}
    }

	private int DanceStep = 0;
	public void Dance(bool Force = false)
	{
		if(!Force && OverrideAnim) return;

		PlayAnim(DanceList[DanceStep]);

		DanceStep++;
		if (DanceStep > DanceList.Length-1) DanceStep = 0;
	}

	public void PlayAnim(string AnimName, bool Force = false)
	{
		if (IsPlayer != MirrorCharacter) AnimName = FlipAnim(AnimName);

		if (AnimPlayer.HasAnimation(Prefix+AnimName+Suffix))
			AnimName = Prefix+AnimName+Suffix;
		
		if(!AnimPlayer.HasAnimation(AnimName))
		{
			GD.PushWarning($"There is no animation called {AnimName}");
			return;
		}

		if(Force || !OverrideAnim || AnimFinished)
		{
			if (LastAnim == AnimName) AnimPlayer.Seek(0);
			LastAnim = AnimName;
			AnimFinished = false;

			AnimPlayer.Play(AnimName);
		}
	}

	private static string FlipAnim(string anim)
    {
        if (anim.Contains("singLEFT")) anim = "singRIGHT";
        else if (anim.Contains("singRIGHT")) anim = "singLEFT";

        return anim;
    }

}