
namespace FNFGodot.Gameplay.Classes.Elements;
public partial class Healthbar : ProgressBar
{
    [Export] public bool ColorableBar = true;
    [Export] public Vector2 BarDownscrollPos = new();
    [Export] public Vector2 ScoreLabelDownscrollPos = new();
    [Export] public Node IconGroup;
    [Export] public Icon PlayerIcon;
    [Export] public Icon OpponentIcon;
    [Export] public float PlayerXOffset = 50;
    [Export] public float OpponentXOffset = -50;
    [Export] public RichTextLabel ScoreLabel;

}