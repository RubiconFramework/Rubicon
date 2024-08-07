using Godot;

namespace Konkon.Data
{
    /// <summary>
    /// Save data for the engine.
    /// </summary>
    public class SaveData
    {
        /// <summary>
        /// The current instance of SaveData being used.
        /// </summary>
        public static SaveData Data { get; private set; } = new SaveData(); // TODO: Actually load a file...

        #region Settings
        
        #region Appearance
        /// <summary>
        /// The note skin to use in-game. Can be overridden by other scripts.
        /// </summary>
        public string Noteskin = "funkin";
        #endregion
        
        #region Gameplay
        /// <summary>
        /// Controls whether the notes go down or up.
        /// </summary>
        public bool Downscroll = false;
        
        /// <summary>
        /// Controls whether the player's notes are in the middle.
        /// </summary>
        public bool Middlescroll = false;
        
        /// <summary>
        /// Controls whether the opponent's chart is hidden. Setting this to true will hide them.
        /// </summary>
        public bool HideOpponentChart = false;

        /// <summary>
        /// Controls the scroll speed of the notes globally. Setting this to 0 will default to the chart's scroll speed.
        /// </summary>
        public float ScrollSpeed = 0.0f;
        #endregion
        
        #region Audio
        /// <summary>
        /// Controls the overall volume of the game.
        /// </summary>
        public float MasterVolume = 50;
        
        /// <summary>
        /// Boolean for turning on and off stereo sound. Turning this false will have the game output mono audio.
        /// </summary>
        public bool StereoMode = true;
        
        /// <summary>
        /// Controls the volume for the music tracks played in-game. This includes the instrumental for songs.
        /// </summary>
        public float MusicVolume = 100;
        
        /// <summary>
        /// Controls the volume for the vocal tracks played with songs that have them in-game.
        /// </summary>
        public float VocalsVolume = 100;
        
        /// <summary>
        /// Controls the volume for the sound effects played in-game.
        /// </summary>
        public float SoundEffectsVolume = 100;
        #endregion
        
        #region Video
        /// <summary>
        /// The Max FPS the game will attempt to run at.
        /// </summary>
        public int MaxFps { get; set; } = 144;
        
        /// <summary>
        /// The Window mode this game is in.
        /// </summary>
        public DisplayServer.WindowMode WindowMode { get; set; } = DisplayServer.WindowMode.Windowed;
        
        /// <summary>
        /// The V-Sync mode this game is running with.
        /// </summary>
        public DisplayServer.VSyncMode VSync { get; set; } = DisplayServer.VSyncMode.Disabled;
        #endregion
        
        #region Miscellaneous
        /// <summary>
        /// Indicates whether Discord RPC should be on.
        /// </summary>
        public bool DiscordRichPresence = true;
        #endregion
        
        #endregion

        /// <summary>
        /// Updates the game when a setting in the save data requires it.
        /// </summary>
        public void Update()
        {
            Engine.MaxFps = MaxFps;
            DisplayServer.Singleton.WindowSetMode(WindowMode);
            DisplayServer.Singleton.WindowSetVsyncMode(VSync);
        }

        /// <summary>
        /// Saves the current settings to the disk.
        /// </summary>
        public void Save()
        {
            // TODO: Actually save the data...
        }
    }
}