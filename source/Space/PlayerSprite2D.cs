using Godot;

[Tool]
public partial class PlayerSprite2D : AnimatedSprite2D
{
	[Export] public bool Playing
	{
		get => Playing;
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
