namespace Rubicon.backend.misc.animatedfont;

[GlobalClass]
public partial class AnimatedFontCharacter : Resource
{
    [Export] public string AnimationName { get; set; } = "";
    [Export] public float Separation { get; set; } = 0f;
}
