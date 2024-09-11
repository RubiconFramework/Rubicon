using System;
using System.Linq;
using Rubicon.Core;
using Rubicon.Core.Chart;
using Rubicon.Core.Data;

namespace Rubicon.Rulesets.Mania;

public partial class ManiaNoteManager : NoteManager
{
    [Export] public int Lane = 0;

    [Export] public string Direction = "";
     
    [Export] public NoteData NoteHeld;
    
    [Export] public float DirectionAngle = 90f;

    [Export] public ManiaNoteSkin NoteSkin;

    public void Setup(ManiaBarLine parent, int lane, ManiaNoteSkin noteSkin)
    {
        ParentBarLine = parent;
        Lane = lane;
        Direction = noteSkin.GetDirection(lane, parent.Chart.Lanes);
        ChangeNoteSkin(noteSkin);

        Notes = parent.Chart.Notes.Where(x => x.Lane == Lane).ToArray();
        Array.Sort(Notes, (a, b) =>
        {
            if (a.Time < b.Time)
                return -1;
            if (a.Time > b.Time)
                return 1;
            
            return 0;
        });
    }

    public void ChangeNoteSkin(ManiaNoteSkin noteSkin)
    {
        NoteSkin = noteSkin;
        if (!NoteSkin.UseTiledHold)
            return;
        
        NoteSkin.InitializeTileHolds();
    }
    
    protected override Note CreateNote() => new ManiaNote();

    protected override void SetupNote(Note note, NoteData data, SvChange svChange)
    {
        if (note is not ManiaNote maniaNote)
            return;
        
        maniaNote.Setup(data, svChange, this, NoteSkin);
    }

    protected override void OnNoteHit(NoteData note, double distance, bool holding)
    {
        base.OnNoteHit(note, distance, holding);
    }

    protected override void OnNoteMiss(NoteData note, double distance, bool holding)
    {
        base.OnNoteMiss(note, distance, holding);
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        string actionName = $"MANIA_{ParentBarLine.Managers}K_{Lane}";
        if (!InputMap.HasAction(actionName) || !@event.IsAction(actionName) || @event.IsEcho())
            return;

        if (@event.IsPressed())
        {
            NoteData[] notes = Notes;
            if (NoteHitIndex >= notes.Length)
            {
                // Play pressed animation
                return;
            }

            double songPos = Conductor.Time * 1000d; // calling it once since this can lag the game HORRIBLY if used without caution
            while (notes[NoteHitIndex].MsTime - songPos <= -(float)ProjectSettings.GetSetting("rubicon/judgments/bad_hit_window"))
            {
                // Miss every note thats too late first
                OnNoteMiss(notes[NoteHitIndex], -(float)ProjectSettings.GetSetting("rubicon/judgments/bad_hit_window") - 1, false);
                NoteHitIndex++;
            }

            double hitTime = notes[NoteHitIndex].MsTime - songPos;
            if (Mathf.Abs(hitTime) <= (float)ProjectSettings.GetSetting("rubicon/judgments/bad_hit_window")) // Literally any other rating
            {
                OnNoteHit(notes[NoteHitIndex], hitTime, notes[NoteHitIndex].Length > 0);
                NoteHitIndex++;
            }
            else if (hitTime < -(float)ProjectSettings.GetSetting("rubicon/judgments/bad_hit_window")) // Your Miss / "SHIT" rating
            {
                // Do confirm thing
                OnNoteMiss(notes[NoteHitIndex], hitTime, true);
                NoteHitIndex++;
            }
            else
            {
                // Do pressed anim
            }
        }
        else if (@event.IsReleased())
        {
            if (NoteHeld != null)
            {
                double length = NoteHeld.MsTime + NoteHeld.MsLength - (Conductor.Time * 1000d);
                if (length <= (float)ProjectSettings.GetSetting("rubicon/judgments/bad_hit_window"))
                    OnNoteHit(NoteHeld, length, false);
                else
                    OnNoteMiss(NoteHeld, length, true);
            }

            // Go back to idle
        }
    }
}