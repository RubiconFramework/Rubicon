namespace Konkon.Data
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
    
    public static class GameData
    {
        public static string[] NoteSpriteSkins = { "funkin" };
        public static NoteHitType[] HitTypes = { NoteHitType.Perfect, NoteHitType.Great, NoteHitType.Good, NoteHitType.Bad };
        public static double[] HitWindows = { 25d, 50d, 90d, 135d };
        public static int MashThreshold = 6;
        public static float MinHoldLength = 90f;
        public static float LosingThreshold = 0.30f;
        public static float WinningThreshold = 0.70f;

        public static string AssetsFolder = "assets";

        public static float MaxScore = 1000000f;
    }
}