using Rubicon.backend.autoload;
using Rubicon.gameplay.resources;

namespace Rubicon.gameplay.classes.elements;
public partial class JudgementSprite : RigidBody2D
{
	[NodePath("Sprite")] private AnimatedSprite2D Sprite;

	public override void _Ready() => this.OnReady();
	public void StartJudgement(Judgement judgement, Vector2 VelocityRangeX, Vector2 VelocityRangeY)
	{
		Sprite.Play(judgement.JudgementName);
		Visible = true;

		SetDeferred("freeze",false);
		LinearVelocity = new Vector2(
			(float)GD.RandRange(VelocityRangeX.X,VelocityRangeX.Y),
			(float)GD.RandRange(VelocityRangeY.X,VelocityRangeY.Y));
		
		Tween tween = CreateTween();
		tween.TweenProperty(Sprite, "modulate:a", 0, 0.2).SetDelay(Conductor.BeatDuration/1000);
		tween.TweenCallback(Callable.From(() => QueueFree()));
	}
}
