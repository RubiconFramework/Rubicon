using Rubicon.gameplay.objects.scripts;

namespace Rubicon.gameplay.objects.strumlines;

public partial class Strum : AnimatedSprite2D
{
    [Export] public int direction { get; set; }

    public override void _Ready() => playAnim("static");

    public void playAnim(string animName)
    {
        Frame = 0;

        switch (animName)
        {
            case "confirm" or "glow" or "hit":
                Play(Note.DefaultNoteDirections[direction] + " confirm");
                Modulate = new(1, 1, 1);
                break;
            case "press" or "pressed":
                Play(Note.DefaultNoteDirections[direction] + " pressed");
                Modulate = new(1, 1, 1, 0.75f);
                break;
            default:
                Play(Note.DefaultNoteDirections[direction] + " static");
                Modulate = new(1, 1, 1, 0.75f);
                break;
        }
    }
}
