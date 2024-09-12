using System;
using Rubicon.Core.Chart;

namespace Rubicon.Core;

/// <summary>
/// The global Conductor class, which keeps track of most of the time-related things.
/// </summary>
[GlobalClass]
public partial class Conductor : Node
{
	#region Singleton
	/// <summary>
	/// The current instance of the Conductor class.
	/// </summary>
	public static Conductor Singleton;
	#endregion

	#region Status Variables
	/// <summary>
	/// The current BPM at the moment.
	/// </summary>
	public static double Bpm
	{
		get => Singleton._bpm;
		set => Singleton._bpm = value;
	}

	/// <summary>
	/// The index pointing to which BPM is currently set based on the bpms array.
	/// </summary>
	public static int BpmIndex
	{
		get => Singleton._bpmIndex;
		set => Singleton._bpmIndex = value;
	}

	/// <summary>
	/// Corrects the time so the chart can be accurate.
	/// </summary>
	public static double ChartOffset
	{
		get => Singleton._chartOffset;
		set => Singleton._chartOffset = value;
	}
	
	/// <summary>
	/// How fast the Conductor goes.
	/// </summary>
	public static double Speed
	{
		get => Singleton._speed;
		set => Singleton._speed = value;
	}

	/// <summary>
	/// Is true when the Conductor has been started with Start() or Play(), false when either Pause() or Stop() is called.
	/// </summary>
	public static bool Playing => Singleton._playing;
	#endregion

	#region Time Variables
	/// <summary>
	/// The raw timestamp of this Conductor, without any corrections made to it.
	/// </summary>
	public static double RawTime => Singleton.GetRawTime();
		
	/// <summary>
	/// The raw timestamp of this Conductor + the chart offset.
	/// </summary>
	public static double UncorrectedTime => Singleton.GetUncorrectedTime();

	/// <summary>
	/// The current timestamp from when the time was last set.
	/// Equivalent to Conductor.songPosition in most other FNF projects.
	/// </summary>
	public static double Time
	{
		get => Singleton.GetTime();
		set => Singleton.SetTime(value);
	}

	/// <summary>
	/// The current step according to the time, which also keeps BPM changes in mind.
	/// </summary>
	public static double CurrentStep => Singleton.GetCurrentStep();

	/// <summary>
	/// The current beat according to the time, which also keeps BPM changes in mind.
	/// </summary>
	public static double CurrentBeat => Singleton.GetCurrentBeat();

	/// <summary>
	/// The current measure according to the time, which also keeps BPM changes in mind.
	/// </summary>
	public static double CurrentMeasure => Singleton.GetCurrentMeasure();
	#endregion

	#region Event Handlers
	/// <summary>
	/// Event triggered when a beat is hit.
	/// </summary>
	[Signal]
	public delegate void BeatHitEventHandler(int beat);
	public static event BeatHitEventHandler OnBeatHit;

	/// <summary>
	/// Event triggered when a step is hit.
	/// </summary>
	[Signal]
	public delegate void StepHitEventHandler(int step);
	public static event StepHitEventHandler OnStepHit;
	
	/// <summary>
	/// Event triggered when a measure (section) is hit.
	/// </summary>
	[Signal]
	public delegate void MeasureHitEventHandler(int section);
	public static event MeasureHitEventHandler OnMeasureHit;

	private int _lastBeat = -1;
	private int _lastStep = -1;
	private int _lastMeasure = -1;
	#endregion
	
	#region Other Variables
	/// <summary>
	/// All BPMs listed in the Conductor currently.
	/// </summary>
	public static BpmInfo[] BpmList
	{
		get => Singleton.GetBpms();
		set => Singleton.SetBpms(value);
	}
		
	/// <summary>
	/// A whole number that indicates the number of beats in each measure.
	/// </summary>
	public static int TimeSigNumerator
	{
		get => Singleton._timeSigNumerator;
		set => Singleton._timeSigNumerator = value;
	}
		
	/// <summary>
	/// A whole number representing what note values the beats indicated in TimeSigNumerator are.
	/// </summary>
	public static int TimeSigDenominator
	{
		get => Singleton._timeSigDenominator;
		set => Singleton._timeSigDenominator = value;
	}
	#endregion

	#region Private Fields
	private double _relativeStartTime;
	private double _relativeTimeOffset;
	private double _lastTime = double.NegativeInfinity;
	private double _delta;
	private double _time;
		
	private double _cachedStep;
	private double _cachedStepTime;

	private double _cachedBeat;
	private double _cachedBeatTime;

	private double _cachedMeasure;
	private double _cachedMeasureTime;
	
	private int _timeSigNumerator = 4;
	private int _timeSigDenominator = 4;
	
	#region Exported Fields
	[ExportGroup("Status"), Export] private double _bpm = 100;
	[Export] private int _bpmIndex = 0;
	[Export] private double _chartOffset = 0;
	[Export] private double _speed = 1f;
	[Export] private bool _playing = false;
	[ExportGroup("Info"), Export] private BpmInfo[] _bpms = { new() { Bpm = 100 } };
	#endregion
	#endregion

	#region Constructor
	/// <summary>
	/// Constructs a new Conductor instance. Will not work if there is already one running.
	/// </summary>
	public Conductor()
	{
		if (Singleton != null)
		{
			QueueFree();
			return;
		}
			
		Singleton = this;
	}
	#endregion
	
	#region Static Methods
	/// <summary>
	/// Starts the Conductor at the time provided.
	/// </summary>
	/// <param name="time">The time the Conductor starts at. Default is 0</param>
	public static void Start(double time = 0d) => Singleton._Start(time);
	
	/// <summary>
	/// Resumes the Conductor at the last time it was paused at.
	/// </summary>
	public static void Play() => Singleton._Play();
	
	/// <summary>
	/// Stops the Conductor from ticking, but keeps the current time.
	/// </summary>
	public static void Pause() => Singleton._Pause();
	
	/// <summary>
	/// Stops the Conductor entirely, resetting the time to 0.
	/// </summary>
	public static void Stop() => Singleton._Stop();
	
	/// <summary>
	/// Resets the Conductor, wiping all its fields with its default values.
	/// </summary>
	public static void Reset() => Singleton._Reset();
	#endregion
	
	#region Private Methods
		
	#region Process Method
	public override void _Process(double delta)
	{
		if (!Playing)
		{
			_relativeStartTime = Godot.Time.GetUnixTimeFromSystem();
			_relativeTimeOffset = _time;
		}

		base._Process(delta);
			
		// Handles bpm changing
		if (BpmIndex < BpmList.Length - 1 && BpmList[BpmIndex + 1].MsTime / 1000f <= Time)
		{
			BpmIndex++;
			Bpm = BpmList[BpmIndex].Bpm;
		}
		
		int beat = (int)Math.Floor(CurrentBeat);
		int step = (int)Math.Floor(CurrentStep);
		int measure = (int)Math.Floor(CurrentMeasure);

		if (beat > _lastBeat)
		{
			_lastBeat = beat;
			EmitSignal(SignalName.BeatHit, beat);
			OnBeatHit?.Invoke(beat);
		}

		if (step > _lastStep)
		{
			_lastStep = step;
			EmitSignal(SignalName.StepHit, step);
			OnStepHit?.Invoke(step);
		}

		if (measure > _lastMeasure)
		{
			_lastMeasure = measure;
			EmitSignal(SignalName.MeasureHit, measure);
			OnMeasureHit?.Invoke(measure);
		}
	}
	#endregion
	
	private void _Start(double time = 0d)
	{
		Play();
		Time = time;
	}
	
	private void _Play() => _playing = true;
	
	private void _Pause()
	{
		_time = GetRawTime();
		_playing = false;
	}
	
	private void _Stop()
	{
		SetTime(0);
		Pause();
	}
	
	private void _Reset()
	{
		TimeSigNumerator = TimeSigDenominator = 4;
		BpmList = new BpmInfo[] { new() { Bpm = 100 } };
		Bpm = BpmList[0].Bpm;
		BpmIndex = 0;
		ChartOffset = 0;
		Speed = 1f;
		Stop();
	}
	#endregion
		
	#region Getters and Setters
	private double GetRawTime() =>
		Playing ? Godot.Time.GetUnixTimeFromSystem() - _relativeStartTime + _relativeTimeOffset :
		_time != 0d ? _time : 0d;
	
	private double GetUncorrectedTime() => GetRawTime() + ChartOffset / 1000d;
	
	private double GetTime() => GetUncorrectedTime() * Speed;
	
	private void SetTime(double time)
	{
		_time = time;
		_relativeStartTime = Godot.Time.GetUnixTimeFromSystem();
		_relativeTimeOffset = time;
	}
	
	private double GetCurrentStep()
	{
		if (_cachedStepTime.Equals(Time))
			return _cachedStep;

		if (BpmList.Length <= 1)
			return Time / (60d / (Bpm * TimeSigDenominator));

		_cachedStepTime = Time;
		_cachedStep = (Time - BpmList[BpmIndex].MsTime / 1000d) / (60 / (Bpm * TimeSigDenominator)) + (BpmList[BpmIndex].Time * TimeSigNumerator * TimeSigDenominator);
		return _cachedStep;
	}
	
	private double GetCurrentBeat()
	{
		if (_cachedBeatTime.Equals(Time))
			return _cachedBeat;

		if (BpmList.Length <= 1)
			return Time / (60d / Bpm);

		_cachedBeatTime = Time;
		_cachedBeat = (Time - BpmList[BpmIndex].MsTime / 1000d) / (60d / Bpm) + BpmList[BpmIndex].Time * TimeSigNumerator;
		return _cachedBeat;
	}
	
	private double GetCurrentMeasure()
	{
		if (_cachedMeasureTime.Equals(Time))
			return _cachedMeasure;

		if (BpmList.Length <= 1)
			return Time / (60d / (Bpm / TimeSigNumerator));

		_cachedMeasureTime = Time;
		_cachedMeasure = (Time - BpmList[BpmIndex].MsTime / 1000d) / (60d / (Bpm / TimeSigNumerator)) + BpmList[BpmIndex].Time;
		return _cachedMeasure;
	}
	
	private BpmInfo[] GetBpms() => _bpms;
	
	private void SetBpms(BpmInfo[] data)
	{
		_bpms = data;
		Bpm = BpmList[0].Bpm;
	}
	#endregion
}
