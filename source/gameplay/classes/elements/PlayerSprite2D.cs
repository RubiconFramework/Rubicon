namespace FNFGodot.Gameplay.Classes.Elements;

[Tool]
[GlobalClass]
public partial class PlayerSprite2D : AnimatedSprite2D
{
	/*	This serves as a tool for using AnimatedCharacter with
		AnimationPlayer more comfortably, by not having to add
		every frame manually to an animation track.	*/

	[Export] public bool playCurrent
	{
		get => true;
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
