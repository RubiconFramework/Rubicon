namespace Rubicon.backend.classes;

[Tool]
public partial class PlayerSprite2D : AnimatedSprite2D
{
	bool isPlaying = true;
	
	[Export] public bool playing
	{
		get => isPlaying;
		set
		{
			if (value) Play(Animation);
			else
			{
				int curFrame = Frame;
				Stop();
				Frame = curFrame;
			}
		}
	}
}
