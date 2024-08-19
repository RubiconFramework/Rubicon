using Godot;
using Promise.Framework;
using Promise.Framework.API;
using Promise.Framework.Chart;
using Promise.Framework.Objects;
using Rubicon.Data;
using Rubicon.Game;

namespace Rubicon.Extras.Notes;

/// <summary>
/// A note type that hurts the target controller upon touching it.
/// </summary>
[NoteTypeBind("hurt")]
public class BasicHurtNote : INoteScript
{
    private float _damage = 0.2f;
    private ShaderMaterial _noteMaterial;
    
    /// <summary>
    /// Lets the game know that the chart should miss this note.
    /// </summary>
    public void BeforeNoteCreate(ChartController chartCtrl, NoteData noteData)
    {
        // I just set it for all chart controllers to miss this note, but if you want the opponent to miss,
        // remove true from this and erase the comment.
        noteData.ShouldMiss = true; // RubiconGame.Instance.Metadata.OpponentChartIndex != chartCtrl.Index;
    }

    /// <summary>
    /// Changes the note graphic to the hurt note graphic.
    /// </summary>
    public void OnNoteCreate(ChartController chartCtrl, Note note)
    {
        if (_noteMaterial == null)
            _noteMaterial = GD.Load<ShaderMaterial>("res://assets/ui/notetypes/hurt/HurtNoteMaterial.tres");

        note.NoteGraphic.Material = _noteMaterial;
        if (note.TailGraphic != null)
            note.TailGraphic.Material = _noteMaterial;
    }

    /// <summary>
    /// Hurts the chart controller if the note was hit! (and NOT the end of a hold note!)
    /// </summary>
    public NoteEventResult OnNoteHit(ChartController chartCtrl, NoteData noteData, NoteHitType hit)
    {
        // If it's not the end of a hold note, players have probably suffered enough!
        if (noteData.Length == 0d)
        {
            chartCtrl.Statistics.Health -= _damage;
            
            // Flag health so the controller doesn't take off by itself, we're in control of the health loss!
            return new NoteEventResult(RubiconNoteFlags.Health, NoteHitType.Miss);
        }
        
        return NoteEventResult.Nothing;
    }

    /// <summary>
    /// Hurts the chart controller if the note was hit AND it is a hold note!
    /// </summary>
    public NoteEventResult OnNoteHeld(ChartController chartCtrl, NoteData noteData, NoteHitType hit)
    {
        chartCtrl.Statistics.Health -= _damage;
            
        // Flag health so the controller doesn't take off by itself, we're in control of the health loss!
        return new NoteEventResult(RubiconNoteFlags.Health, NoteHitType.Miss);
    }

    /// <summary>
    /// Do nothing.
    /// </summary>
    public NoteEventResult OnNoteMiss(ChartController chartCtrl, NoteData noteData, bool held) => NoteEventResult.Nothing;
}