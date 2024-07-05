
namespace FNFGodot.Gameplay.Resources;
[GlobalClass]
public partial class Judgement : Resource
{
	[Export] public string JudgementName;
	[Export] public float Time;
	[Export] public int ScoreAdd;
	[Export] public float RatingAdd;
	[Export] public float HealthAdd;
	[Export] public bool ValidHit;
	[Export] public bool ShouldSplash;
}
