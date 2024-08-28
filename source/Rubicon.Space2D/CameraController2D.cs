using System.Linq;
using Godot;
using Promise.Framework;
using Vector2 = Godot.Vector2;

namespace Rubicon.Space2D;

public partial class CameraController2D : Node2D
{
    [ExportGroup("Status"), Export] public int Focus = 2;

    [ExportGroup("Targets"), Export] public Vector2 TargetPosition = Vector2.Zero;

    [Export] public float TargetRotation = 0f;

    [Export] public float TargetZoom = 1f;
    
    [ExportSubgroup("Offset"), Export] public Vector2 OffsetPosition = Vector2.Zero;

    [Export] public float OffsetRotation = 0f;
    
    [Export] public float OffsetZoom = 0f;

    [ExportGroup("Settings"), Export] public bool UseCharacterCameras = true;
    
    [Export] public bool InterpolatePosition = true;
    
    [Export] public bool InterpolateRotation = true;
    
    [Export] public bool InterpolateZoom = true;
    
    [Export] public bool IgnoreCameraFocus = false;
    
    [Export] public float FollowSpeed = 0.02f;
    
    [Export] public float BounceLerp = 0.05f;
    
    [ExportSubgroup("Major Bounce"), Export] public bool EnableMajorBounce = true;
    
    [Export] public int MajorBounceBeat = 4;
    
    [Export] public float MajorBounceIntensity = 0.2f;
    
    [ExportSubgroup("Minor Bounce"), Export] public bool EnableMinorBounce = false;
    
    [Export] public int MinorBounceBeat = 2;
    
    [Export] public float MinorBounceIntensity = 0.0f;

    [ExportGroup("References"), Export] public Camera2D Camera { get; private set; }

    [Export] public Stage2D Stage;
    
    [Signal] public delegate void OnFocusEventHandler(int idx, bool snap = false);
    [Signal] public delegate void OnBounceEventHandler(float intensity = 0f);
    [Signal] public delegate void OnSnapEventHandler();
    
    private int _lastBeat = 0;
    
    public override void _Process(double delta)
    {
        base._Process(delta);

        int beat = Mathf.FloorToInt(Mathf.Abs(Conductor.CurrentBeat));
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
            
        Vector2 targetPos = TargetPosition + OffsetPosition;
        float targetRot = TargetRotation + OffsetRotation;
        GlobalPosition = InterpolatePosition && !GlobalPosition.IsEqualApprox(targetPos) ? GlobalPosition.Lerp(targetPos, FollowSpeed) : targetPos;
        GlobalRotation = InterpolateRotation && !Mathf.IsEqualApprox(GlobalRotation, targetRot) ? Mathf.Lerp(GlobalRotation, targetRot, FollowSpeed) : targetRot;

        Vector2 targetZ = Vector2.One * (TargetZoom + OffsetZoom);
        Camera.Zoom = InterpolateZoom && !Camera.Zoom.IsEqualApprox(targetZ) ? Camera.Zoom.Lerp(targetZ, BounceLerp) : targetZ;
    }
    
    public void FocusOnCameraPoint(int idx, bool snap = false)
    {
        Focus = idx;

        if (IgnoreCameraFocus)
        {
            EmitSignal(SignalName.OnFocus, idx, snap);
            return;
        }

        // checks
        CameraFocusPoint2D camPoint = Stage.FocusPoints[idx];
        if (UseCharacterCameras && idx < Stage.CharacterGroups.Count &&
            Stage.CharacterGroups[idx].Characters.Count > 0 && Stage.CharacterGroups[idx].Characters.Any(x => x.Active))
        {
            Character2D firstActive = Stage.CharacterGroups[idx].Characters.FirstOrDefault(x => x.Active);
            camPoint = firstActive.FocusPoint;
        }
        
        TargetPosition = Stage.Position;
        TargetRotation = 0;
            
        NodePath pathToPoint = Stage.GetPathTo(camPoint);
        string[] splitPath = pathToPoint.ToString().Split('/');
        string currentPath = splitPath[0];
        for (int i = 0; i < splitPath.Length; i++)
        {
            Node curNode = Stage.GetNode(currentPath);
            
            if (curNode is Node2D node2D and not ParallaxLayer)
            {
                TargetRotation += node2D.Rotation;

                Vector2 pos = node2D.Position.Rotated(node2D.Rotation);
                Node parent = node2D.GetParent();
                if (parent is Node2D parent2D and not ParallaxLayer)
                    pos *= parent2D.Scale;
                else if (parent is Control parentControl)
                    pos *= parentControl.Scale;

                TargetPosition += pos;
            }
            else if (curNode is Control control)
            {
                TargetRotation += control.Rotation;
                
                Vector2 pos = control.Position.Rotated(control.Rotation);
                Node parent = control.GetParent();
                if (parent is Node2D parent2D and not ParallaxLayer)
                    pos *= parent2D.Scale;
                else if (parent is Control parentControl)
                    pos *= parentControl.Scale;

                TargetPosition += pos;
            }

            if (i + 1 < splitPath.Length)
                currentPath += "/" + splitPath[i + 1];
        }

        //TargetPosition = camPoint.GlobalPosition;
        //TargetRotation = camPoint.GlobalRotation;

        if (camPoint.UseCustomZoom)
            TargetZoom = camPoint.Zoom;

        if (snap)
            Snap();
            
        EmitSignal(SignalName.OnFocus, idx, snap);
    }
    
    public void Snap()
    {
        GlobalPosition = TargetPosition;
        GlobalRotation = TargetRotation;
        Camera.Zoom = Vector2.One * TargetZoom;

        EmitSignal(SignalName.OnSnap);
    }
    
    public void Bounce(float intensity = 0f)
    {
        Camera.Zoom += Vector2.One * intensity;
    }
}