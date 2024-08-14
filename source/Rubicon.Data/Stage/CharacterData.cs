using Godot;
using Godot.Collections;
using Array = System.Array;

namespace Rubicon.Data.Stage;

/// <summary>
/// A resource that contains everything needed for a character to function.
/// </summary>
public partial class CharacterData : Resource
{
    /// <summary>
    /// Contains animation data for this character. Contains additional information required for animations to work.
    /// </summary>
    [ExportGroup("Settings"), Export] public CharacterAnimation[] Animations = Array.Empty<CharacterAnimation>();

    /// <summary>
    /// A collection of sprites to be used as a health bar icon.
    /// </summary>
    [Export] public SpriteFrames Icon;
    
    /// <summary>
    /// An array of animations played in order when the character is idling.
    /// </summary>
    [Export] public string[] IdleAnimation = { "idle" };
    
    /// <summary>
    /// How many beats to wait until the next idle animation plays.
    /// </summary>
    [Export] public uint IdleBeat = 2;
    
    /// <summary>
    /// The animations to play when a note is hit, miss or not.
    /// </summary>
    [Export] public Dictionary<int, string[]> NoteAnimations = new() { { 4, new[] {"singLEFT", "singDOWN", "singUP", "singRIGHT"} } };
    
    /// <summary>
    /// When enabled, brings back the original functionality of characters repeating their singing animation every step when a note is held down.
    /// </summary>
    [Export] public bool StepRepeat = false;
    
    /// <summary>
    /// Tells whether the character is normally facing left or not, without any modifications.
    /// </summary>
    [Export] public bool LeftFacing = false;
    
    /// <summary>
    /// The color to be used on the health bar.
    /// </summary>
    [Export] public Color[] HealthColors = { Colors.White };

    /// <summary>
    /// If not null, the game will switch to this character upon death.
    /// </summary>
    [ExportSubgroup("Death"), Export] public PackedScene DeathCharacter = null;
    
    /// <summary>
    /// The sound to play immediately after the player dies.
    /// </summary>
    [Export] public AudioStream DeathSoundSfx = null;
    
    /// <summary>
    /// The BPM of the music played on death.
    /// </summary>
    [Export] public double DeathMusicBpm = 100d;
    
    /// <summary>
    /// The music played upon dying.
    /// </summary>
    [Export] public AudioStream DeathMusic = null;
    
    /// <summary>
    /// The sound to play when the player restarts the song after death.
    /// </summary>
    [Export] public AudioStream DeathConfirmSfx = null;
}