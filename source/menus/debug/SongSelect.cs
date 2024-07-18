using Rubicon.Backend.Autoload;
using ChartHandler = Rubicon.backend.autoload.ChartHandler;

namespace Rubicon.menus.debug;

public partial class SongSelect : Control
{
    [NodePath("song")] LineEdit songName;
    [NodePath("diff")] LineEdit difficulty;

    public override void _Ready() => this.OnReady();

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey && Input.IsActionJustPressed("menu_accept")) OnSongSelect();
    }

    public void OnSongSelect()
    {
        if (songName != null)
        {
            if (difficulty.Text is null or "") difficulty.Text = "normal";
            ChartHandler.NewChart(songName.Text, difficulty.Text);
            LoadingHandler.ChangeScene("res://source/gameplay/Gameplay2D.tscn");
        }
        else GD.PrintErr("No chart name entered.");
    }
}
