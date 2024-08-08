namespace Promise.Framework
{
    public enum NoteHitType
    {
        None = -1,
        Perfect,
        Great,
        Good,
        Bad,
        Miss
    }
    
    public static class PromiseData
    {
        /// <summary>
        /// An array of ratings for notes, ordered from greatest to least.
        /// </summary>
        public static NoteHitType[] HitTypes = { NoteHitType.Perfect, NoteHitType.Great, NoteHitType.Good, NoteHitType.Bad };
        
        /// <summary>
        /// An array of hit windows for hitting notes. Indexes should match with the hit types.
        /// </summary>
        public static double[] HitWindows = { 25d, 50d, 90d, 135d };
        
        /// <summary>
        /// Helps with syncing the music and the inputs together.
        /// </summary>
        public static double Offset = 0d;
        
        /// <summary>
        /// Helps with syncing the music and chart visually.
        /// </summary>
        public static float VisualOffset = 0f;

        /// <summary>
        /// The max score if the player gets all perfect hits, provided the score isn't arbitrary.
        /// </summary>
        public static float MaxScore = 1000000f;
    }
}