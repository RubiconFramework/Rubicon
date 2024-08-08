using System;
using Godot;
using Rubicon.Game.Data;

namespace Rubicon.Game.Chart
{
    /// <summary>
    /// Used to hold important information about a song.
    /// </summary>
    [GlobalClass]
    public partial class SongMetadata : Resource
    {
        /// <summary>
        /// The name of the song.
        /// </summary>
        [Export] public string Name = "";

        /// <summary>
        /// The artist who made the song.
        /// </summary>
        [Export] public string Artist = "";

        /// <summary>
        /// The week associated with this song.
        /// </summary>
        [Export] public string Week = "";

        /// <summary>
        /// The icon that's associated with this song.
        /// </summary>
        [Export] public Texture2D Icon;

        /// <summary>
        /// Tells the game whether to look for vocals or not.
        /// </summary>
        [Export] public bool UseVocals = false;

        /// <summary>
        /// Locks Opponent and Speaker mode out.
        /// </summary>
        [Export] public bool OnlyPlayerMode = false;
        
        /// <summary>
        /// The stage to spawn in for this chart.
        /// </summary>
        [Export] public string Stage = "stage";
        
        /// <summary>
        /// The UI style to use for this chart.
        /// </summary>
        [Export] public string UiStyle = "funkin";
        
        /// <summary>
        /// The initial difficulty to choose when in Free Play.
        /// </summary>
        [ExportGroup("Difficulties"), Export] public string InitialDifficulty = "normal";

        /// <summary>
        /// The difficulties available to play.
        /// </summary>
        [Export] public SongDifficulty[] Difficulties = Array.Empty<SongDifficulty>();

        /// <summary>
        /// Determines whether the song needs to be unlocked.
        /// </summary>
        [ExportGroup("Unlocking"), Export] public bool RequireUnlock = false;

        /// <summary>
        /// If RequireUnlock is true, these conditions unlock the song.
        /// </summary>
        [Export] public string[] UnlockConditions = Array.Empty<string>();
        
        /// <summary>
        /// The characters to spawn in the song.
        /// </summary>
        [ExportGroup("Characters"), Export] public CharacterChart[] Characters = Array.Empty<CharacterChart>();
        
        /// <summary>
        /// The index for which chart to select to be the opponent (sometimes referred as the Dad).
        /// </summary>
        [ExportSubgroup("Focused Indexes"), Export] public int OpponentChartIndex = 0;
        
        /// <summary>
        /// The index for which chart to select to be playable (sometimes referred as the BF).
        /// </summary>
        [Export] public int PlayerChartIndex = 1;
        
        /// <summary>
        /// The index for which chart acts as the speaker character (sometimes referred as the GF).
        /// </summary>
        [Export] public int SpeakerChartIndex = 2;
        
        /// <summary>
        /// Whether to enable the countdown or not.
        /// </summary>
        [ExportGroup("Miscellaneous"), Export] public bool Countdown = true;

        /// <summary>
        /// Turning this on will use the camera positions preset on the stage. If set to false, the camera will follow the individual character camera positions.
        /// </summary>
        [Export] public bool UsePresetPositions = false;

        /// <summary>
        /// The audio file's extension.
        /// </summary>
        [Export] public string AudioExtension = ".ogg";
    }
}