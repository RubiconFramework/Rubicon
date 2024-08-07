using Konkon.Data;
using Konkon.Core.Chart;
using Konkon.Core.Objects;

namespace Konkon.Game.API
{
    /// <summary>
    /// An interface to be inherited by any C# note types.
    /// </summary>
    public interface INoteScript
    {
        /// <summary>
        /// Triggers when the ChartController is being initialized for every note. You can switch on and off certain settings the note's data from here.
        /// </summary>
        /// <param name="chartCtrl">The chart controller this data belongs to</param>
        /// <param name="noteData">The note data</param>
        public void BeforeNoteCreate(ChartController chartCtrl, NoteData noteData);

        /// <summary>
        /// Triggers when the note graphic is spawned.
        /// </summary>
        /// <param name="chartCtrl">The chart controller this note belongs to</param>
        /// <param name="note">The created note.</param>
        public void OnNoteCreate(ChartController chartCtrl, Note note);

        /// <summary>
        /// Triggers when a regular note is hit, or when a hold note is successfully completed.
        /// </summary>
        /// <param name="chartCtrl">The chart controller this data belongs to</param>
        /// <param name="noteData">The note data</param>
        /// <param name="hit">The rating</param>
        /// <returns></returns>
        public NoteEventResult OnNoteHit(ChartController chartCtrl, NoteData noteData, NoteHitType hit) => new NoteEventResult(hit);

        /// <summary>
        /// Triggers when a hold note is being held.
        /// </summary>
        /// <param name="chartCtrl">The chart controller this data belongs to</param>
        /// <param name="noteData">The note data</param>
        /// <param name="hit">The rating</param>
        /// <returns></returns>
        public NoteEventResult OnNoteHeld(ChartController chartCtrl, NoteData noteData, NoteHitType hit) => NoteEventResult.Nothing;

        /// <summary>
        /// Triggers when a note is missed.
        /// </summary>
        /// <param name="chartCtrl">The chart controller this data belongs to</param>
        /// <param name="noteData">The note data</param>
        /// <returns></returns>
        public NoteEventResult OnNoteMiss(ChartController chartCtrl, NoteData noteData) => NoteEventResult.NothingMiss;
    }   
}