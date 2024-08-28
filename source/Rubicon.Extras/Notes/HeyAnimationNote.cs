using Promise.Framework;
using Promise.Framework.API;
using Promise.Framework.Chart;
using Promise.Framework.Objects;
using Rubicon.Data;
using Rubicon.Game;

namespace Rubicon.Extras.Notes;

/// <summary>
/// A note type that makes characters play the "hey" animation on hit.
/// </summary>
[NoteTypeBind("hey")]
public class HeyAnimationNote : INoteScript
{
    public void OnNoteCreate(ChartController chartCtrl, NoteData noteData) { }

    public void OnNoteSpawn(ChartController chartCtrl, Note note) { }

    /// <summary>
    /// Makes the assigned characters play the hey animation if the note was hit! (and NOT the end of a hold note!)
    /// </summary>
    public NoteEventResult OnNoteHit(ChartController chartCtrl, NoteData noteData, NoteHitType hit)
    {
        // If it's not the end of a hold note, don't want to spam it!
        if (noteData.Length == 0d)
        {
            if (RubiconGame.Instance.Stage2D != null)
            {
                // Play hey animation for all characters in the assigned character group!
                RubiconGame.Instance.Stage2D.CharacterGroups[chartCtrl.Index].PlayAnimation("hey");
            }
        }

        return new NoteEventResult(RubiconNoteFlags.Animation, hit);
    }

    /// <summary>
    /// Makes the assigned characters play the hey animation if the note was hit AND it is a hold note!
    /// </summary>
    public NoteEventResult OnNoteHeld(ChartController chartCtrl, NoteData noteData, NoteHitType hit)
    {
        if (RubiconGame.Instance.Stage2D != null)
        {
            // Play hey animation for all characters in the assigned character group!
            RubiconGame.Instance.Stage2D.CharacterGroups[chartCtrl.Index].PlayAnimation("hey");
        }
        
        return new NoteEventResult(RubiconNoteFlags.Animation, hit);
    }

    /// <summary>
    /// Miss normally.
    /// </summary>
    public NoteEventResult OnNoteMiss(ChartController chartCtrl, NoteData noteData, bool held) => NoteEventResult.NothingMiss;
}