
namespace FNFGodot.Gameplay.Classes.Elements;
[GlobalClass]
public partial class Icon : AnimatedSprite2D
{
    public bool IsPlayer = false;
    public float IconBopSpeed = 15;
    public Vector2 DefaultScale = new(1,1);

    public override void _Ready() => DefaultScale = Scale;

    public override void _Process(double delta)
	{
		float FinalSpeed = (float)Mathf.Clamp(delta * IconBopSpeed, 0, 1);
		Scale = new Vector2(Mathf.Lerp(Scale.X, DefaultScale.X, FinalSpeed),Mathf.Lerp(Scale.Y, DefaultScale.Y, FinalSpeed));
	}
}
