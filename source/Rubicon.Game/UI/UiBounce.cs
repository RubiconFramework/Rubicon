using Godot;
using Promise.Framework;

namespace Rubicon.Game.UI;

public partial class UiBounce : Control
{
    [ExportGroup("Targets"), Export] public Vector2 TargetScale = Vector2.One;

    [ExportGroup("Settings"), Export] public float BounceLerp = 0.05f;
    [Export] public bool Interpolate = true;

    [ExportGroup("Bounce Targets")]
    [Export] public bool EnableMajorBounce = true;
    [Export] public int MajorBounceBeat = 4;
    [Export] public float MajorBounceIntensity = 0.03f;

    [Export] public bool EnableMinorBounce = false;
    [Export] public int MinorBounceBeat = 2;
    [Export] public float MinorBounceIntensity = 0.0f;        

    private int _lastBeat = 0;
        
    public override void _Process(double delta)
    {
        base._Process(delta);

        // Correct pivot to center
        PivotOffset = Size / 2f;

        int beat = Mathf.FloorToInt(Mathf.Abs(Conductor.Instance.CurrentBeat));
        if (beat != _lastBeat)
        {
            if (EnableMajorBounce && beat % MajorBounceBeat == 0)
                Bounce(MajorBounceIntensity);
            if (EnableMinorBounce && beat % MinorBounceBeat == 0)
                Bounce(MinorBounceIntensity);
        }

        _lastBeat = beat;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (Interpolate)
            Scale = Scale.Lerp(TargetScale, BounceLerp);
        else
            Scale = TargetScale;
    }

    /// <summary>
    /// Makes the HUD bounce forward / back.
    /// </summary>
    /// <param name="intensity"></param>
    public void Bounce(float intensity = 0f)
    {
        Vector2 scale = Scale;
        scale += Vector2.One * intensity;
        Scale = scale;
    }
}