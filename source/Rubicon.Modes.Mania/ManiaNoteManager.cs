using System;
using System.Linq;
using Rubicon.Core;
using Rubicon.Core.Chart;
using Rubicon.Core.Data;

namespace Rubicon.Modes.Mania;

public partial class ManiaNoteManager : NoteManager
{
    [Export] public int Lane = 0;

    [Export] public string Direction = "";
     
    [Export] public NoteData NoteHeld;
    
    [Export] public float DirectionAngle = 90f;

    [Export] public ManiaNoteSkin NoteSkin;

    private Texture2D _tiledHoldGraphic;
    private Image _tiledHoldImage;

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
        _tiledHoldGraphic = null;
        _tiledHoldImage = null;

        if (!NoteSkin.UseTiledHold)
            return;

        _tiledHoldGraphic = noteSkin.HoldAtlas.GetFrameTexture($"{Direction}NoteHold", 0);
        if (_tiledHoldGraphic is not AtlasTexture atlasTexture)
            return;

        int xPos = (int)atlasTexture.Region.Position.X;
        int yPos = (int)atlasTexture.Region.Position.Y;
        int width = (int)atlasTexture.Region.Size.X;
        int height = (int)atlasTexture.Region.Size.Y;
        _tiledHoldImage = Image.CreateEmpty(width, height, false, Image.Format.Rgba8);
        _tiledHoldImage.BlitRect(atlasTexture.GetImage(), new Rect2I(xPos, yPos, width, height), Vector2I.Zero);
        _tiledHoldGraphic = ImageTexture.CreateFromImage(_tiledHoldImage);
    }
    
    protected override Note SpawnNote(NoteData data, SvChange svChange)
    {
        ManiaNote note = new ManiaNote();
        //note.Setup(data, svChange);
        
        return base.SpawnNote(data, svChange);
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
            while (notes[NoteHitIndex].MsTime - songPos <= -EngineSettings.BadHitWindow)
            {
                // Miss every note thats too late first
                OnNoteMiss(notes[NoteHitIndex], -EngineSettings.BadHitWindow - 1, false);
                NoteHitIndex++;
            }

            double hitTime = notes[NoteHitIndex].MsTime - songPos;
            if (Mathf.Abs(hitTime) <= EngineSettings.BadHitWindow) // Literally any other rating
            {
                OnNoteHit(notes[NoteHitIndex], hitTime, notes[NoteHitIndex].Length > 0);
                NoteHitIndex++;
            }
            else if (hitTime < -EngineSettings.BadHitWindow) // Your Miss / "SHIT" rating
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
                if (length <= EngineSettings.BadHitWindow)
                    OnNoteHit(NoteHeld, length, false);
                else
                    OnNoteMiss(NoteHeld, length, true);
            }

            // Go back to idle
        }
    }
}