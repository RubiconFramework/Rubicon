
namespace Rubicon.gameplay.classes.elements;
public partial class Healthbar : ProgressBar
{
    [Export] public bool ColorableBar = true;
    [Export] public Vector2 BarDownscrollPos;
    [Export] public Vector2 ScoreLabelDownscrollPos;
    [Export] public Node IconGroup;
    [Export] public Icon PlayerIcon;
    [Export] public Icon OpponentIcon;
    [Export] public float PlayerXOffset = 50;
    [Export] public float OpponentXOffset = -50;
    [Export] public RichTextLabel ScoreLabel;

}