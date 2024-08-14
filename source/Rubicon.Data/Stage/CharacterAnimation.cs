using Godot;

namespace Rubicon.Data.Stage;

/// <summary>
/// Data for an animation played by a character.
/// </summary>
[GlobalClass]
public partial class CharacterAnimation : Resource
{
    /// <summary>
    /// The name of the animation.
    /// </summary>
    [Export] public string Name = "";

    /// <summary>
    /// Whether to interrupt the idle playing animation when this is played.
    /// </summary>
    [Export] public bool IdleInterrupt = false;
    
    /// <summary>
    /// Whether to interrupt the note playing animation when this is played.
    /// </summary>
    [Export] public bool NoteInterrupt = true;
    
    /// <summary>
    /// Whether this animation is played when losing.
    /// </summary>
    [Export] public bool Losing = false;
    
    /// <summary>
    /// Whether this animation is played when winning.
    /// </summary>
    [Export] public bool Winning = false;
    
    /// <summary>
    /// Whether this animation is played when missing.
    /// </summary>
    [Export] public bool Miss = false;
    
    /// <summary>
    /// Which animation to override, should this be replacing another. (Example: singRIGHTmiss would replace singRIGHT)
    /// </summary>
    [Export] public string OverrideAnimation = null;
    
    /// <summary>
    /// Which animation to go to, should the character be facing the other way. (Example: singRIGHT on the opposite side for singLEFT)
    /// </summary>
    [Export] public string FlipXOverride = null;
    
    /// <summary>
    /// The next animation to play after this one.
    /// </summary>
    [Export] public string NextAnimation = null;
}