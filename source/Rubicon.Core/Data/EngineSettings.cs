namespace Rubicon.Core.Data;

[GlobalClass]
public partial class EngineSettings : Resource
{
    public static double PerfectHitWindow => Instance._perfectHitWindow;

    public static double GreatHitWindow => Instance._greatHitWindow;

    public static double GoodHitWindow => Instance._goodHitWindow;

    public static double BadHitWindow => Instance._badHitWindow;
    
    private static readonly EngineSettings Instance = GD.Load<EngineSettings>("res://rubicon_settings.tres");

    [ExportGroup("Hit Windows"), Export] private double _perfectHitWindow = 25d;

    [Export] private double _greatHitWindow = 50d;

    [Export] private double _goodHitWindow = 90d;

    [Export] private double _badHitWindow = 135d;
}