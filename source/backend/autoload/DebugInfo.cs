using System.Diagnostics;
using System.Globalization;
using System.Text;
using Rubicon.Backend.Autoload;

namespace Rubicon.backend.autoload;

[Icon("res://assets/miscicons/autoload.png")]
public partial class DebugInfo : CanvasLayer
{
    /*Main Info (Always Visible)*/
    //Performance
    [NodePath("InfoContainer/MainInformation/FPS")] private Label FPS;
    [NodePath("InfoContainer/MainInformation/RAM")] private Label RAM;
    [NodePath("InfoContainer/MainInformation/VRAM")] private Label VRAM;
    
    /*Debug Info*/
    //Scene Tree
    [NodePath("InfoContainer/DebugInformation/Objects/AllObjects")] private Label AllObjects;
    [NodePath("InfoContainer/DebugInformation/Objects/NodeObjects")] private Label NodeObjects;
    [NodePath("InfoContainer/DebugInformation/Objects/ResourceObjects")] private Label ResourceObjects;
    
    //Versions
    [NodePath("InfoContainer/DebugInformation/Versions/Rubicon")] private Label RubiconVersion;
    [NodePath("InfoContainer/DebugInformation/Versions/Godot")] private Label GodotVersion;
    
    //Misc
    [NodePath("InfoContainer/DebugInformation/Scene")] private Label CurrentScene;
    [NodePath("InfoContainer/DebugInformation/Conductor")] private Label ConductorInfo;

    private Process CurrentProcess = Process.GetCurrentProcess();

    [NodePath("InfoContainer/DebugInformation")] private VBoxContainer DebugInformation; //for visibility
    
    private float RAMUpdateTime;
    private float ObjectUpdateTime;
    
    public override void _Ready()
    {
        this.OnReady();
        if (!OS.IsDebugBuild()) VRAM.Visible = false;
        DebugInformation.Visible = false;
        DebugInformation.VisibilityChanged += () =>
        {
            if (!DebugInformation.Visible) return;
            UpdateObjects();
            UpdateScene();
        };
        RubiconVersion.Text = $"Rubicon Framework {Main.RubiconVersion} {(OS.IsDebugBuild() ? "[Debug]" : "[Release]")}";
        GodotVersion.Text = $"Godot Engine {Engine.GetVersionInfo()["major"]}.{Engine.GetVersionInfo()["minor"]}.{Engine.GetVersionInfo()["patch"]} [{Engine.GetVersionInfo()["status"]}]";
    }

    private static string byteToReadableUnit(long bytes)
    {
        double size = bytes;
        string unit = "MB";
        size /= (1024.0 * 1024.0);
        if (size >= 1024.0)
        {
            size /= 1024.0;
            unit = "GB";
        }
        return $"{size:F2} {unit}";
    }
    
    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("debug_info")) DebugInformation.Visible = !DebugInformation.Visible;
        
        UpdateFPS();
        
        RAMUpdateTime += (float)delta;
        if (RAMUpdateTime >= 1f)
        {
            UpdateRAM();
            if (VRAM.Visible) UpdateVRAM();
            RAMUpdateTime = 0f;
        }

        if (!DebugInformation.Visible) return;
        
        ObjectUpdateTime += (float)delta;
        if (ObjectUpdateTime >= 2f)
        {
            UpdateObjects();
            UpdateScene();
            CurrentScene.Text = $"Scene: {(GetTree().CurrentScene != null && GetTree().CurrentScene.SceneFilePath != "" ? GetTree().CurrentScene.SceneFilePath : "None")}";
            ObjectUpdateTime = 0f;
        }
        
        UpdateConductor();
    }
    
    private void UpdateFPS() => FPS.Text = $"FPS: {Engine.GetFramesPerSecond()}";

    private void UpdateRAM() => RAM.Text = OS.IsDebugBuild() ? $"RAM: {byteToReadableUnit((long)OS.GetStaticMemoryUsage())} [{byteToReadableUnit(CurrentProcess.PrivateMemorySize64)}]" : $"RAM: {byteToReadableUnit(CurrentProcess.WorkingSet64)} [{byteToReadableUnit(CurrentProcess.PrivateMemorySize64)}]";

    private void UpdateVRAM() => VRAM.Text = $"VRAM: {byteToReadableUnit((long)Performance.GetMonitor(Performance.Monitor.RenderTextureMemUsed))}";

    private void UpdateScene() => CurrentScene.Text = $"Scene: {(GetTree().CurrentScene != null && GetTree().CurrentScene.SceneFilePath != "" ? GetTree().CurrentScene.SceneFilePath : "None")}";

    private void UpdateObjects()
    {
        AllObjects.Text = $"All Instantiated Objects: {Performance.GetMonitor(Performance.Monitor.ObjectCount)}";
        ResourceObjects.Text = $"Resource Objects in Use: {Performance.GetMonitor(Performance.Monitor.ObjectResourceCount)}";
        NodeObjects.Text = $"Node Objects: {Performance.GetMonitor(Performance.Monitor.ObjectNodeCount)} (Orphan Nodes: {Performance.GetMonitor(Performance.Monitor.ObjectOrphanNodeCount)})";
    }

    private readonly StringBuilder ConductorSB = new();
    private void UpdateConductor()
    {
        ConductorSB.Clear();
        
        ConductorSB.AppendLine($"Conductor BPM: {Conductor.BPM} --- Song Position: {Conductor.SongPosition}")
            .AppendLine($"CurStep: {Conductor.CurStep} [Duration: {Conductor.StepDuration}]")
            .AppendLine($"CurBeat: {Conductor.CurBeat} [Duration: {Conductor.BeatDuration}]")
            .AppendLine($"CurSection: {Conductor.CurSection} [Duration: {Conductor.SectionDuration}]");

        ConductorInfo.Text = ConductorSB.ToString();
    }
}
