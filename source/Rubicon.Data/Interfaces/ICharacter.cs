using System.Collections.Generic;
using System.Linq;
using Godot;
using Promise.Framework;
using Rubicon.Data.Stage;

namespace Rubicon.Data.Interfaces;

/// <summary>
/// An interface to be implemented in character classes.
/// </summary>
public interface ICharacter
{
    #region Exported Variables

    /// <summary>
    /// The data for this character to be functional.
    /// </summary>
    public CharacterData Data { get; set; }

    /// <summary>
    /// The current animation being played by this character.
    /// </summary>
    public string CurrentAnimation { get; set; }

    /// <summary>
    /// If the character is note locked (aka currently pressing a key), the character will not idle.
    /// </summary>
    public bool NoteLocked { get; set; }

    /// <summary>
    /// Whether this character is losing or not.
    /// </summary>
    public bool Losing { get; set; }

    /// <summary>
    /// Whether this character is winning or not.
    /// </summary>
    public bool Winning { get; set; }
    
    /// <summary>
    /// Whether this character has missed a note.
    /// </summary>
    public bool Missed { get; set; }

    /// <summary>
    /// Whether this character is dead.
    /// </summary>
    public bool Dead { get; set; }

    /// <summary>
    /// The current animation being played without any overrides. (example: CurrentAnimation = "singRIGHTmiss" would be just "singRIGHT" here).
    /// </summary>
    public string CurrentState { get; set; }

    /// <summary>
    /// A suffix to place after every animation name.
    /// </summary>
    public string AnimationSuffix { get; set; }

    /// <summary>
    /// Determines whether this character is able to sing.
    /// </summary>
    public bool Active { get; set; }
    
    /// <summary>
    /// Whether this character can play its idle animation or not.
    /// </summary>
    public bool CanIdle { get; set; }

    /// <summary>
    /// The playback speed for this character's animation player.
    /// </summary>
    public float SpeedScale { get; set; }

    /// <summary>
    /// The lane count so characters can sing properly. (i need a better description)
    /// </summary>
    public int LaneCount { get; set; }

    /// <summary>
    /// A reference to this character's animation player.
    /// </summary>
    public AnimationPlayer AnimationPlayer { get; set; }
    #endregion

    /// <summary>
    /// Plays the current idle animation. Which idle animation it plays is dependent on the current beat.
    /// </summary>
    public void PlayIdleAnim();

    /// <summary>
    /// Plays the note animation based on the given parameters.
    /// </summary>
    /// <param name="note">The lane</param>
    /// <param name="miss">Whether the character missed</param>
    public void PlayNoteAnim(int note, bool miss = false);

    /// <summary>
    /// Checks for any valid overrides before passing the final animation name onto PlayAnimation.
    /// </summary>
    /// <param name="state">The animation state</param>
    /// <param name="miss">Whether the character missed</param>
    /// <param name="time">The time to play at (in seconds)</param>
    /// <returns>Whether playing the animation was successful or not.</returns>
    public bool PrepareAndPlay(string state, bool miss = false, double time = 0d);

    /// <summary>
    /// Plays an animation directly.
    /// </summary>
    /// <param name="state">The animation name</param>
    /// <param name="time">The time to play at (in seconds)</param>
    /// <returns>Whether playing the animation was successful or not.</returns>
    public bool PlayAnimation(string state, double time = 0d);

    /// <summary>
    /// Returns the death character or itself, for use in game overs.
    /// </summary>
    /// <returns>The death character.</returns>
    public ICharacter GetDeathCharacter();
}