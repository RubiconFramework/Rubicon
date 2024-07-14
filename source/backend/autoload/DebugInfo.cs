using System.Diagnostics;
using System.Globalization;
using System.Text;
using Rubicon.Backend.Autoload;

namespace Rubicon.backend.autoload;

[Icon("res://assets/miscicons/autoload.png")]
public partial class DebugInfo : CanvasLayer
{
    [NodePath("InfoContainer/Performance/FPS")] private Label FPSLabel;
    [NodePath("InfoContainer/Performance/RAM")] private Label RAMLabel;
    [NodePath("InfoContainer/Performance/VRAM")] private Label VRAMLabel;
    [NodePath("InfoContainer/Performance/NodeObjects")] private Label NodeObjectsLabel;
    [NodePath("InfoContainer/Version")] private Label VersionLabel;
    [NodePath("InfoContainer/Scene")] private Label SceneLabel;
    [NodePath("InfoContainer/Conductor")] private Label ConductorLabel;

    private float updateTime;
    private bool showDebugInfo;
    private Process currentProcess = Process.GetCurrentProcess();
    
    public override void _Ready()
    {
        this.OnReady();
        if (!OS.IsDebugBuild()) VRAMLabel.Text = "VRAM is Unavailable.";
        VersionLabel.Text = $"Rubicon {Main.RubiconVersion} {(OS.IsDebugBuild() ? "[Debug]" : "[Release]")}";
    }

    public override void _PhysicsProcess(double delta)
    {
        updateTime += (float)delta;
        if (updateTime >= 1)
        {
            updateTime = 0;
            UpdateText();
        }

        if (Input.IsActionJustPressed("debug_info")) showDebugInfo = !showDebugInfo;
        SceneLabel.Visible = showDebugInfo;
        ConductorLabel.Visible = showDebugInfo;
        VersionLabel.Visible = showDebugInfo;
    }

    StringBuilder ConductorSB = new();

    private void UpdateText()
    {
        ConductorSB.Clear();
        
        FPSLabel.Text = $"FPS: {Engine.GetFramesPerSecond().ToString(CultureInfo.InvariantCulture)}";
        if (OS.IsDebugBuild())
        {
            RAMLabel.Text = $"RAM: {Main.byteToMB((long)OS.GetStaticMemoryUsage()):F2} MB [A: {Main.byteToMB(currentProcess.PrivateMemorySize64):F2} MB]";
            VRAMLabel.Text = $"VRAM: {Main.byteToMB((long)Performance.GetMonitor(Performance.Monitor.RenderTextureMemUsed)):F2} MB";
            SceneLabel.Text = $"Scene: {(GetTree().CurrentScene != null && GetTree().CurrentScene.SceneFilePath != "" ? GetTree().CurrentScene.SceneFilePath : "None")}";
            NodeObjectsLabel.Text = $"Node Objects: {Performance.GetMonitor(Performance.Monitor.ObjectNodeCount)}";
        }
        else
        {
            RAMLabel.Text = $"RAM: {Main.byteToMB(currentProcess.WorkingSet64):F2} MB [Alloc: {Main.byteToMB(currentProcess.PrivateMemorySize64):F2} MB]";
            SceneLabel.Text = $"Scene: {(GetTree().CurrentScene != null && GetTree().CurrentScene.SceneFilePath != "" ? GetTree().CurrentScene.SceneFilePath : "None")}";
            NodeObjectsLabel.Text = $"Node Objects: {Performance.GetMonitor(Performance.Monitor.ObjectNodeCount)}";
        }

        ConductorSB.AppendLine($"BPM: {Conductor.BPM} // Song Position: {Conductor.SongPosition}")
            .AppendLine($"Step: {Conductor.CurStep} [{Conductor.StepDuration}] ")
            .AppendLine($"Beat: {Conductor.CurBeat} [{Conductor.BeatDuration}]")
            .AppendLine($"Section: {Conductor.CurSection} [{Conductor.SectionDuration}]");

        ConductorLabel.Text = ConductorSB.ToString();
    }
}
