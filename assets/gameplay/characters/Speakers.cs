using Rubicon.gameplay.classes.elements;

namespace Rubicon.assets.gameplay.characters;

public partial class Speakers : Character2D
{
	[NodePath("Speaker")] private PlayerSprite2D speakerSprite;
	public new void Dance(bool Force = false)
	{
		base.Dance();

		speakerSprite.Frame = 0;
		speakerSprite.playCurrent = true;
	}
}