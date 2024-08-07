using Godot;
using Konkon.Data;

namespace Konkon.Game
{
    /// <summary>
    /// A class to track statistics for each chart controller.
    /// </summary>
    [GlobalClass]
    public partial class ChartStatistics : GodotObject
    {
        #region Health
        /// <summary>
        /// This chart's current health.
        /// </summary>
        public float Health = 1.0f;
        
        /// <summary>
        /// This chart's maximum health.
        /// </summary>
        public float MaxHealth = 2.0f;
        #endregion

        #region Hits
        /// <summary>
        /// Counter for perfect hits.
        /// </summary>
        public uint PerfectHits = 0;
        
        /// <summary>
        /// Counter for great hits.
        /// </summary>
        public uint GreatHits = 0;
        
        /// <summary>
        /// Counter for good hits.
        /// </summary>
        public uint GoodHits = 0;
        
        /// <summary>
        /// Counter for bad hits.
        /// </summary>
        public uint BadHits = 0;
        
        /// <summary>
        /// Counter for misses.
        /// </summary>
        public uint Misses = 0;
        
        /// <summary>
        /// Counter for how many notes were hit in total.
        /// </summary>
        public uint HitNotes => PerfectHits + GreatHits + GoodHits + BadHits;
        #endregion

        #region Combo
        /// <summary>
        /// Counter for this chart controller's current combo.
        /// </summary>
        public uint Combo = 0;
        
        /// <summary>
        /// Counter for this chart controller's highest combo.
        /// </summary>
        public uint HighestCombo = 0;
        
        /// <summary>
        /// Counter for this chart controller's current miss streak.
        /// </summary>
        public uint MissStreak = 0;
        #endregion

        #region Scoring
        /// <summary>
        /// The max score for this chart controller. Does not affect score if ArbitraryScore is turned on.
        /// </summary>
        public float MaxScore = GameData.MaxScore;
        
        /// <summary>
        /// If turned on, you can add your own scoring system with custom note scripts.
        /// </summary>
        public bool ArbitraryScore = false;
        
        /// <summary>
        /// The current score.
        /// </summary>
        public float Score
        {
            get
            {
                if (ArbitraryScore)
                    return _arbScore;

                if (PerfectHits == NoteCount)
                    return MaxScore;

                return (float)((NoteValue * PerfectHits) + (NoteValue * (GreatHits * 0.95)) + (NoteValue * (GoodHits * 0.75)) + (NoteValue * (BadHits * 0.5)));
            }
        }
        #endregion

        #region Miscellaneous
        /// <summary>
        /// How many notes are hittable in the current chart.
        /// </summary>
        public uint NoteCount = 0;
        
        /// <summary>
        /// Tells how much each note is worth.
        /// </summary>
        public float NoteValue => MaxScore / NoteCount;
        #endregion

        #region Private Variables
        private float _arbScore = 0f;
        #endregion
    }
}