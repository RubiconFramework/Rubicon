using Rubicon.Core.Chart;

namespace Rubicon.Core;

/// <summary>
/// The global instance of <see cref="ConductorInstance"/>. Made for easier access.
/// </summary>
public static class Conductor
{
	/// <summary>
	/// The current global instance of <see cref="ConductorInstance"/>.
	/// </summary>
	public static ConductorInstance Singleton;

	/// <inheritdoc cref="ConductorInstance.Bpm"/>
	public static double Bpm
	{
		get => Singleton.Bpm;
		set => Singleton.Bpm = value;
	}

	/// <inheritdoc cref="ConductorInstance.BpmIndex"/>
	public static int BpmIndex
	{
		get => Singleton.BpmIndex;
		set => Singleton.BpmIndex = value;
	}

	/// <inheritdoc cref="ConductorInstance.ChartOffset"/>
	public static double ChartOffset
	{
		get => Singleton.ChartOffset;
		set => Singleton.ChartOffset = value;
	}

	/// <inheritdoc cref="ConductorInstance.Speed"/>
	public static double Speed
	{
		get => Singleton.Speed;
		set => Singleton.Speed = value;
	}

	/// <inheritdoc cref="ConductorInstance.Playing"/>
	public static bool Playing
	{
		get => Singleton.Playing;
		set => Singleton.Playing = value;
	}

	/// <inheritdoc cref="ConductorInstance.RawTime"/>
	public static double RawTime => Singleton.RawTime;

	/// <inheritdoc cref="ConductorInstance.UncorrectedTime"/>
	public static double UncorrectedTime => Singleton.UncorrectedTime;

	/// <inheritdoc cref="ConductorInstance.Time"/>
	public static double Time
	{
		get => Singleton.Time;
		set => Singleton.Time = value;
	}

	/// <inheritdoc cref="ConductorInstance.CurrentStep"/>
	public static double CurrentStep => Singleton.CurrentStep;

	/// <inheritdoc cref="ConductorInstance.CurrentBeat"/>
	public static double CurrentBeat => Singleton.CurrentBeat;

	/// <inheritdoc cref="ConductorInstance.CurrentMeasure"/>
	public static double CurrentMeasure => Singleton.CurrentMeasure;

	/// <inheritdoc cref="ConductorInstance.BpmList"/>
	public static BpmInfo[] BpmList
	{
		get => Singleton.BpmList;
		set => Singleton.BpmList = value;
	}

	/// <inheritdoc cref="ConductorInstance.TimeSigNumerator"/>
	public static int TimeSigNumerator
	{
		get => Singleton.TimeSigNumerator;
		set => Singleton.TimeSigNumerator = value;
	}

	/// <inheritdoc cref="ConductorInstance.TimeSigDenominator"/>
	public static int TimeSigDenominator
	{
		get => Singleton.TimeSigDenominator;
		set => Singleton.TimeSigDenominator = value;
	}
	
	/// <inheritdoc cref="ConductorInstance.MeasureHitEventHandler"/>
	public static event ConductorInstance.MeasureHitEventHandler MeasureHit
	{
		add => Singleton.MeasureHit += value;
		remove => Singleton.MeasureHit -= value;
	}
	
	/// <inheritdoc cref="ConductorInstance.BeatHitEventHandler"/>
	public static event ConductorInstance.BeatHitEventHandler BeatHit
	{
		add => Singleton.BeatHit += value;
		remove => Singleton.BeatHit -= value;
	}
	
	/// <inheritdoc cref="ConductorInstance.StepHitEventHandler"/>
	public static event ConductorInstance.StepHitEventHandler StepHit
	{
		add => Singleton.StepHit += value;
		remove => Singleton.StepHit -= value;
	}

	/// <inheritdoc cref="ConductorInstance.Play"/>
	public static void Play(double time = 0d) => Singleton.Play(time);

	/// <inheritdoc cref="ConductorInstance.Resume"/>
	public static void Resume() => Singleton.Resume();

	/// <inheritdoc cref="ConductorInstance.Pause"/>
	public static void Pause() => Singleton.Pause();

	/// <inheritdoc cref="ConductorInstance.Stop"/>
	public static void Stop() => Singleton.Stop();

	/// <inheritdoc cref="ConductorInstance.Reset"/>
	public static void Reset() => Singleton.Reset();
}
