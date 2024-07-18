using System.Collections.Generic;
using System.Linq;
using Rubicon.gameplay.resources;

namespace Rubicon.gameplay.classes;
public partial class HighScore : Node
{
	public int Score = 0;
	public int Misses = 0;
	public int Combo = 0;
	public float Rating = 0;
	public static readonly List<Judgement> Judgements = new();
	
	public readonly Dictionary<string, int> JudgementHitList = new(){
		{"sick",0},
		{"good",0},
		{"bad",0},
		{"shit",0}
	};

	public static void SetJudgementArray(IEnumerable<Judgement> array)
	{
		Judgements.Clear();
		foreach(Judgement judgement in array) Judgements.Add(judgement);
		Judgements.Sort((a, b) => -a.Time.CompareTo(b.Time));
	}

	public static Judgement JudgeHit(float time)
	{
		Judgement FinalJudgement = Judgements[^1];
		foreach (var judgement in Judgements.Where(judgement => time < judgement.Time)) FinalJudgement = judgement;
		return FinalJudgement;
	}
}
