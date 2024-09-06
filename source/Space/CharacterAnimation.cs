using Godot;

public class CharacterAnimation
{
	public string AnimName = "idle"; // self explanatory
	public bool Force = false; // overrides any animation playing right now, no matter what
	public bool OverrideDance = true; // cant be overridden by idle or dance animations (good for sing anims)
	public bool OverrideAnim = false; // cant be overridden by any animation unless Force is true
	public bool AnimFinished = false; // self explanatory
	
	// these override the static ones.
	public string Prefix;
	public string Suffix;
}