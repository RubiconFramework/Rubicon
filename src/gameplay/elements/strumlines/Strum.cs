namespace Rubicon.gameplay.elements.strumlines;

public partial class Strum : AnimatedSprite2D
{
    [Export] public int direction { get; set; }
    string[] directionList = { "left", "down", "up", "right" };

    public override void _Ready() => playAnim("static");

    public void playAnim(string animName)
    {
        Frame = 0;

        switch (animName)
        {
            case "confirm" or "glow" or "hit":
                Play(directionList[direction] + " confirm");
                Modulate = new(1, 1, 1);
                break;
            case "press" or "pressed":
                Play(directionList[direction] + " pressed");
                Modulate = new(1, 1, 1, 0.75f);
                break;
            default:
                Play(directionList[direction] + " static");
                Modulate = new(1, 1, 1, 0.75f);
                break;
        }
    }
}
