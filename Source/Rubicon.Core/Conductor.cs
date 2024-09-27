using Rubicon.Core.Autoload;
using Rubicon.Core.Chart;

namespace Rubicon.Core;

/// <summary>
/// The global instance of <see cref="ConductorSingleton"/>. Made for easier access.
/// </summary>
public static class Conductor
{
	/// <summary>
	/// The current global instance of <see cref="ConductorSingleton"/>.
	/// </summary>
	public static ConductorSingleton Singleton;

	/// <inheritdoc cref="ConductorSingleton.Bpm"/>
	public static double Bpm
	{
		get => Singleton.Bpm;
		set => Singleton.Bpm = value;
	}

	/// <inheritdoc cref="ConductorSingleton.BpmIndex"/>
	public static int BpmIndex
	{
		get => Singleton.BpmIndex;
		set => Singleton.BpmIndex = value;
	}

	/// <inheritdoc cref="ConductorSingleton.ChartOffset"/>
	public static double ChartOffset
	{
		get => Singleton.ChartOffset;
		set => Singleton.ChartOffset = value;
	}

	/// <inheritdoc cref="ConductorSingleton.Speed"/>
	public static double Speed
	{
		get => Singleton.Speed;
		set => Singleton.Speed = value;
	}

	/// <inheritdoc cref="ConductorSingleton.Playing"/>
	public static bool Playing
	{
		get => Singleton.Playing;
		set => Singleton.Playing = value;
	}

	/// <inheritdoc cref="ConductorSingleton.RawTime"/>
	public static double RawTime => Singleton.RawTime;

	/// <inheritdoc cref="ConductorSingleton.UncorrectedTime"/>
	public static double UncorrectedTime => Singleton.UncorrectedTime;

	/// <inheritdoc cref="ConductorSingleton.Time"/>
	public static double Time
	{
		get => Singleton.Time;
		set => Singleton.Time = value;
	}

	/// <inheritdoc cref="ConductorSingleton.CurrentStep"/>
	public static double CurrentStep => Singleton.CurrentStep;

	/// <inheritdoc cref="ConductorSingleton.CurrentBeat"/>
	public static double CurrentBeat => Singleton.CurrentBeat;

	/// <inheritdoc cref="ConductorSingleton.CurrentMeasure"/>
	public static double CurrentMeasure => Singleton.CurrentMeasure;

	/// <inheritdoc cref="ConductorSingleton.BpmList"/>
	public static BpmInfo[] BpmList
	{
		get => Singleton.BpmList;
		set => Singleton.BpmList = value;
	}

	/// <inheritdoc cref="ConductorSingleton.TimeSigNumerator"/>
	public static int TimeSigNumerator
	{
		get => Singleton.TimeSigNumerator;
		set => Singleton.TimeSigNumerator = value;
	}

	/// <inheritdoc cref="ConductorSingleton.TimeSigDenominator"/>
	public static int TimeSigDenominator
	{
		get => Singleton.TimeSigDenominator;
		set => Singleton.TimeSigDenominator = value;
	}
	
	/// <inheritdoc cref="ConductorSingleton.MeasureHitEventHandler"/>
	public static event ConductorSingleton.MeasureHitEventHandler MeasureHit
	{
		add => Singleton.MeasureHit += value;
		remove => Singleton.MeasureHit -= value;
	}
	
	/// <inheritdoc cref="ConductorSingleton.BeatHitEventHandler"/>
	public static event ConductorSingleton.BeatHitEventHandler BeatHit
	{
		add => Singleton.BeatHit += value;
		remove => Singleton.BeatHit -= value;
	}
	
	/// <inheritdoc cref="ConductorSingleton.StepHitEventHandler"/>
	public static event ConductorSingleton.StepHitEventHandler StepHit
	{
		add => Singleton.StepHit += value;
		remove => Singleton.StepHit -= value;
	}

	/// <inheritdoc cref="ConductorSingleton.Play"/>
	public static void Play(double time = 0d) => Singleton.Play(time);

	/// <inheritdoc cref="ConductorSingleton.Resume"/>
	public static void Resume() => Singleton.Resume();

	/// <inheritdoc cref="ConductorSingleton.Pause"/>
	public static void Pause() => Singleton.Pause();

	/// <inheritdoc cref="ConductorSingleton.Stop"/>
	public static void Stop() => Singleton.Stop();

	/// <inheritdoc cref="ConductorSingleton.Reset"/>
	public static void Reset() => Singleton.Reset();
}