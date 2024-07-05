#region Imports 
using FNFGodot.Backend.Autoload;
#endregion

namespace FNFGodot.Backend.Debug;
public partial class SongSelect : Control
{
	[NodePath("song")] LineEdit songName;
	[NodePath("diff")] LineEdit difficulty;
	public override void _Ready() => this.OnReady();

    public override void _Input(InputEvent @event)
    {
		if(@event is InputEventKey && Input.IsActionJustPressed("menu_accept"))
			OnSongSelect();
    }

    public void OnSongSelect() 
	{
		if(songName != null) {
			if(difficulty.Text is null || difficulty.Text == "") difficulty.Text = "normal";
			ChartHandler.NewChart(songName.Text, difficulty.Text);
			LoadingHandler.ChangeScene("res://source/gameplay/Gameplay2D.tscn");
		}
		else
			GD.PrintErr("No chart name entered.");
	}
}
