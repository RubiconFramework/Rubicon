using System;
using Godot;

namespace Promise.Framework.API
{
    /// <summary>
    /// Flags for NoteEventResult. Will prevent the action from being activated.
    /// </summary>
    [Flags]
    public enum NoteEventProcessFlags : uint
    {
        None = 0x0,
        Animation = 0x1,
        Health = 0x2,
        Score = 0x4,
        Splash = 0x8
    }

    /// <summary>
    /// An object one can modify to prevent actions from passing through in ChartController or modify the rating of.
    /// </summary>
    public partial class NoteEventResult : GodotObject
    {
        /// <summary>
        /// An easily accessible NoteEventResult that does nothing.
        /// </summary>
        public static NoteEventResult Nothing { get; private set; } = new NoteEventResult(NoteEventProcessFlags.None, NoteHitType.None);
        
        /// <summary>
        /// An easily accessible NoteEventResult that does nothing BUT will grant you a miss.
        /// </summary>
        public static NoteEventResult NothingMiss { get; private set; } = new NoteEventResult(NoteEventProcessFlags.None, NoteHitType.Miss);

        /// <summary>
        /// Essentially the rating of the note hit. You can modify what type of rating this is before the note goes through ChartController!
        /// </summary>
        public NoteHitType Hit;
        
        /// <summary>
        /// Flags for the ChartController to handle. Adding flags onto this will PREVENT the action from happening!
        /// </summary>
        public NoteEventProcessFlags ProcessFlags;

        /// <summary>
        /// Empty initializer, will essentially create NoteEventResult.Nothing.
        /// </summary>
        public NoteEventResult() : this(NoteEventProcessFlags.None, NoteHitType.None) { }
        
        /// <summary>
        /// Creates a new NoteEventResult that does nothing, but will pass the hit type / rating on.
        /// </summary>
        /// <param name="hitType"></param>
        public NoteEventResult(NoteHitType hitType) : this(NoteEventProcessFlags.None, hitType) { }
        
        /// <summary>
        /// Creates a new NoteEventResult to be used on ChartControllers.
        /// </summary>
        /// <param name="flags">Flags for the ChartController to handle.</param>
        /// <param name="hitType">The type of note hit, or the ratinf</param>
        public NoteEventResult(NoteEventProcessFlags flags, NoteHitType hitType = NoteHitType.None)
        {
            Hit = hitType;
            ProcessFlags = flags;
        }
        
        /// <summary>
        /// Takes the higher rating and merges the process flags.
        /// </summary>
        /// <param name="a">The first NoteEventResult</param>
        /// <param name="b">The second NoteEventResult</param>
        /// <returns>The combined NoteEventResult</returns>
        public static NoteEventResult operator+ (NoteEventResult a, NoteEventResult b)
        {
            if (b.Hit > a.Hit || b.Hit == NoteHitType.None)
                a.Hit = b.Hit;

            a.ProcessFlags |= b.ProcessFlags;
            return a;
        }
    }
}