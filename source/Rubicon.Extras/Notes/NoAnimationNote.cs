using Promise.Framework;
using Promise.Framework.API;
using Promise.Framework.Chart;
using Promise.Framework.Objects;
using Rubicon.Data;

namespace Rubicon.Extras.Notes;

/// <summary>
/// A note type that prevents the character from doing their normal sing animation.
/// </summary>
[NoteTypeBind("no_anim")]
public class NoAnimationNote : INoteScript
{
    public void OnNoteCreate(ChartController chartCtrl, NoteData noteData) { }
    
    public void OnNoteSpawn(ChartController chartCtrl, Note note) { }
    
    /// <summary>
    /// Returns a result that contains the RubiconNoteFlags.Animation flag, disabling any animation.
    /// </summary>
    public NoteEventResult OnNoteHit(ChartController chartCtrl, NoteData noteData, NoteHitType hit) => new NoteEventResult(RubiconNoteFlags.Animation, hit);

    /// <summary>
    /// Returns a result that contains the RubiconNoteFlags.Animation flag, disabling any animation.
    /// </summary>
    public NoteEventResult OnNoteHeld(ChartController chartCtrl, NoteData noteData, NoteHitType hit) => new NoteEventResult(RubiconNoteFlags.Animation, hit);

    /// <summary>
    /// Returns a result that contains the RubiconNoteFlags.Animation flag, disabling any animation.
    /// </summary>
    public NoteEventResult OnNoteMiss(ChartController chartCtrl, NoteData noteData, bool held) => new NoteEventResult(RubiconNoteFlags.Animation, NoteHitType.Miss);
}