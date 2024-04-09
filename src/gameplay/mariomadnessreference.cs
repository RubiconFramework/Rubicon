using Godot.Sharp.Extras;

namespace Rubicon.gameplay;

public partial class mariomadnessreference : Control
{
	[NodePath("AnimationPlayer")] private AnimationPlayer AnimPlayer;
	[NodePath("Artist")] private Label ArtistLabel;
	[NodePath("Song")] private Label SongLabel;

	private readonly string SongName;
	private readonly string Artist;
	private readonly float Duration;
	
	public mariomadnessreference() {} //godot needs an empty constructor idk why

	public mariomadnessreference(string songName, string artist, float duration = 1.0f)
	{
		SongName = songName;
		Artist = artist;
		Duration = duration;
	}

	public override void _Ready()
	{
		this.OnReady();
		SongLabel.Text = SongName;
		ArtistLabel.Text = Artist;
		
		AnimPlayer.Play("in");
		AnimPlayer.AnimationFinished += async name =>
		{
			if (name == "in")
			{
				await ToSignal(GetTree().CreateTimer(Duration), SceneTreeTimer.SignalName.Timeout);
				AnimPlayer.Play("out");
			}
			else this.QueueFree();
		};
	}
}
