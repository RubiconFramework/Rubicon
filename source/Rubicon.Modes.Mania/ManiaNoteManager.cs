using Rubicon.Core;
using Rubicon.Core.Chart;
using Rubicon.Core.Data;

namespace Rubicon.Modes.Mania;

public partial class ManiaNoteManager : NoteManager
{
    [Export] public int Lane = 0;
     
    [Export] public NoteData NoteHeld;
    
    [Export] public float DirectionAngle = 90f;

    [ExportGroup("Graphics"), Export] public SpriteFrames NoteAtlas;

    [Export] public string NoteGraphicName = "green";

    [Export] public string HoldGraphicName = "green hold piece";

    [Export] public string TailGraphicName = "green hold end";

    [Export] public bool IsTiledHold = false;

    private Texture2D _noteGraphic;
    private Texture2D _holdGraphic;
    private Texture2D _tailGraphic;

    private Image _holdImage;

    public void Setup()
    {
        ChangeNoteGraphics(NoteAtlas, NoteGraphicName, HoldGraphicName, TailGraphicName);
    }

    public void ChangeNoteGraphics(SpriteFrames atlas, string noteName, string holdName, string tailName)
    {
        _noteGraphic = null;
        _tailGraphic = null;

        if (_holdImage != null)
        {
            _holdGraphic.Free();
            _holdImage.Free();
        }
        _holdGraphic = null;

        NoteAtlas = atlas;
        
        NoteGraphicName = noteName;
        HoldGraphicName = holdName;
        TailGraphicName = tailName;
        
        // Create textures to use
        _noteGraphic = NoteAtlas.GetFrameTexture(NoteGraphicName, 0);
        _tailGraphic = NoteAtlas.GetFrameTexture(TailGraphicName, 0);

        _holdGraphic = NoteAtlas.GetFrameTexture(HoldGraphicName, 0);
        if (_holdGraphic is AtlasTexture atlasTex)
        {
            _holdImage = Image.CreateEmpty((int)atlasTex.Region.Size.X, (int)atlasTex.Region.Size.Y, false, Image.Format.Rgba8);
            _holdImage.BlitRect(atlasTex.Atlas.GetImage(), (Rect2I)atlasTex.Region, Vector2I.Zero);
            ImageTexture tex = ImageTexture.CreateFromImage(_holdImage);
            _holdGraphic = tex;
        }
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
            NoteData[] notes = Chart.Notes;
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