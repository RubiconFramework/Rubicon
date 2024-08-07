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
        public static float[] HitWindows = { 25f, 50f, 90f, 135f };
        public static int MashThreshold = 6;
        public static float MinHoldLength = 90f;
        public static float LosingThreshold = 0.30f;
        public static float WinningThreshold = 0.70f;

        public static string AssetsFolder = "assets";

        public static float MaxScore = 1000000f;
    }
}