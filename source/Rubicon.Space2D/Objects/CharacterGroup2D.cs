using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace Rubicon.Space2D.Objects;

/// <summary>
/// A class that contains characters, as well as give extra functions.
/// </summary>
public partial class CharacterGroup2D : Node2D
{
    /// <summary>
    /// Either allows or denies all the characters in this CharacterGroup to idle.
    /// </summary>
    public bool CanIdle 
    {
        set
        { 
            foreach (Character2D chara in Characters) 
                chara.CanIdle = value;
        }
    }
    
    /// <summary>
    /// Sets how many beats to wait until the next idle animation plays for every character in this character group.
    /// </summary>
    public uint IdleBeat
    {
        set
        {
            foreach (Character2D chara in Characters)
                chara.Data.IdleBeat = value;
        }
    }

    /// <summary>
    /// All characters contained in this character group.
    /// </summary>
    public Array<Character2D> Characters { get; set; } = new Array<Character2D>();
    
    /// <summary>
    /// Grabs a character from this group that matches the name provided. (case-sensitive)
    /// </summary>
    /// <param name="name">The character's name.</param>
    /// <returns>Returns a character that matches the name provided, null if not.</returns>
    public Character2D GetCharacterByName(string name)
    {
        for (int i = 0; i < Characters.Count; i++)
            if ((Characters[i] as Character2D).Name == name)
                return Characters[i];

        return null;
    }

    /// <summary>
    /// Makes all characters in this CharacterGroup play their idle dance.
    /// </summary>
    public void PlayIdleAnim()
    {
        for (int i = 0; i < Characters.Count; i++)
            Characters[i].PlayIdleAnim();
    }
    
    /// <summary>
    /// Makes all characters in this CharacterGroup play an animation based on lane number.
    /// </summary>
    /// <param name="note">The lane</param>
    /// <param name="miss">Whether the character missed</param>
    public void PlayNoteAnim(int note, bool miss = false)
    {
        for (int i = 0; i < Characters.Count; i++)
            if (Characters[i].Active)
                Characters[i].PlayNoteAnim(note, miss);
    }

    /// <summary>
    /// Makes all characters in this CharacterGroup play an animation.
    /// </summary>
    /// <param name="state">The animation name</param>
    /// <param name="miss">Whether the character missed</param>
    /// <param name="time">The start time, in seconds</param>
    public void PrepareAndPlay(string state, bool miss = false, float time = 0f)
    {
        for (int i = 0; i < Characters.Count; i++)
            Characters[i].PrepareAndPlay(state, miss, time);
    }
    
    /// <summary>
    /// Makes all characters in this CharacterGroup play an animation, without any modification.
    /// </summary>
    /// <param name="state">The animation name</param>
    /// <param name="time">The start time, in seconds</param>
    public void PlayAnimation(string state, float time = 0f)
    {
        for (int i = 0; i < Characters.Count; i++)
            Characters[i].PlayAnimation(state, time);
    }

    /// <summary>
    /// Passing "true" locks the characters in their note animations so they can't idle.
    /// </summary>
    /// <param name="lockChar">Whether to note lock the characters or not</param>
    public void LockNote(bool lockChar = false)
    {
        for (int i = 0; i < Characters.Count; i++)
            if (Characters[i].Active)
                Characters[i].NoteLocked = lockChar;
    }
}