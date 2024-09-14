
namespace Rubicon.Space;

public class CharacterAnimation
{
	/// <summary>
	/// The name of the animation played by the AnimationPlayer. It has to match exactly.
	/// </summary>
	public string AnimName = "idle";

	/// <summary>
	/// Overrides any animation currently playing on the character, no matter what.
	/// </summary>
	public bool Force = false;

	/// <summary>
	/// The animation cannot be overriden by any dance/idle animation.
	/// </summary>
	public bool OverrideDance = true;

	/// <summary>
	/// Represents whether its an idle/dance animation or not.
	/// </summary>
	public bool IsDanceAnimation = false;

	/// <summary>
	/// The animation cannot be overriden unless Force is true.
	/// </summary>
	public bool OverrideAnim = false;

	/// <summary>
	/// Represents whether the animation is finished or not.
	/// </summary>
	public bool AnimFinished = false;
	
	/// <summary>
	/// A prefix for the animation played by the AnimationPlayer. Overrides the StaticPrefix found in the Character class.
	/// </summary>
	public string Prefix;

    /// <summary>
    /// A suffix for the animation played by the AnimationPlayer. Overrides the StaticSuffix found in the Character class.
    /// </summary>
    public string Suffix;

	/// <summary>
	/// This animation will play at the end of the current one.
	/// </summary>
	public CharacterAnimation PostAnimation;
}