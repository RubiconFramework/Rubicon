#region Imports
	using Rubicon.Backend.Classes;
#endregion

namespace Rubicon.Backend.Autoload;
public partial class Conductor : Node
{
	/*	Why haxe engine's Conductor/MusicBeat is such a mess?
		This is pretty simple but effective
		and i dont need GameplayBase to inherit no MusicBeat bullshit!	*/

	public static double SongPosition = 0;

	public static double SongDuration = 0;
	public static bool UpdatePosition = false;
	
	public static float Bpm = 0;

	public static float BeatDuration;
	public static float StepDuration;
	public static float SectionDuration;

	public static int CurBeat = 0;
	public static int CurStep = 0;
	public static int CurSection = 0;

	public static event Action StepHitEvent;
	public static event Action BeatHitEvent;
	public static event Action SectionHitEvent;

	public static void UpdateBpm(float NewBpm)
	{
		Bpm = NewBpm;
		BeatDuration = 60 / Bpm * 1000;
		StepDuration = BeatDuration / 4;
		SectionDuration = StepDuration * 16;
	}

    private int PrevStep = 0;
	public override void _Process(double delta) {
		PrevStep = CurStep;
		
		if(UpdatePosition) {
			SongPosition += delta*1000;

			CurStep = (int)(Math.Floor(SongPosition) / StepDuration);
			CurBeat = CurStep / 4;
			CurSection = CurStep / 16;
		}

		if (PrevStep != CurStep) StepHitEvent.Invoke();
		if (PrevStep/4 != CurBeat) BeatHitEvent.Invoke();
		if (PrevStep/16 != CurSection) SectionHitEvent.Invoke();
		PrevStep = CurStep;
	}
}	
