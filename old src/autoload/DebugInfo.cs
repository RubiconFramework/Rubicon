using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Rubicon.autoload;

//[Icon("res://assets/miscicons/autoload.png")]
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

    private double byteToMB(long bytes) => bytes / (1024.0 * 1024.0);

    public override void _Ready()
    {
        this.OnReady();
        if (!OS.IsDebugBuild()) VRAMLabel.Text = "VRAM is Unavailable.";
        VersionLabel.Text = $"Rubicon Framework {Main.RubiconVersion} {(OS.IsDebugBuild() ? "[Debug]" : "[Release]")}";
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
            RAMLabel.Text = $"RAM: {byteToMB((long)OS.GetStaticMemoryUsage()):F2} MB [A: {byteToMB(currentProcess.PrivateMemorySize64):F2} MB]";
            VRAMLabel.Text = $"VRAM: {byteToMB((long)Performance.GetMonitor(Performance.Monitor.RenderTextureMemUsed)):F2} MB";
            SceneLabel.Text = $"Scene: {(GetTree().CurrentScene != null && GetTree().CurrentScene.SceneFilePath != "" ? GetTree().CurrentScene.SceneFilePath : "None")}";
            NodeObjectsLabel.Text = $"Node Objects: {Performance.GetMonitor(Performance.Monitor.ObjectNodeCount)}";
        }
        else
        {
            RAMLabel.Text = $"RAM: {byteToMB(currentProcess.WorkingSet64):F2} MB [Alloc: {byteToMB(currentProcess.PrivateMemorySize64):F2} MB]";
            SceneLabel.Text = $"Scene: {(GetTree().CurrentScene != null && GetTree().CurrentScene.SceneFilePath != "" ? GetTree().CurrentScene.SceneFilePath : "None")}";
            NodeObjectsLabel.Text = $"Node Objects: {Performance.GetMonitor(Performance.Monitor.ObjectNodeCount)}";
        }

        /*if (Conductor.ConductorInstance != null)
        {
            ConductorSB.AppendLine($"BPM: {Conductor.B} // Song Position: {Conductor.ConductorInstance.position}")
                .AppendLine($"Crochet: {Conductor.ConductorInstance.crochet} // StepCrochet: {Conductor.ConductorInstance.stepCrochet}")
                .AppendLine($"Step: {Conductor.ConductorInstance.curStep} [Dec: {Conductor.ConductorInstance.curDecBeat}] // Beat: {Conductor.ConductorInstance.curBeat} [Dec: {Conductor.ConductorInstance.curDecStep}] // Section: {Conductor.ConductorInstance.curSection} [Dec: {Conductor.ConductorInstance.curDecSection}]");
        }
        else
        {
            ConductorLabel.Size = new (0,0);
            ConductorSB.AppendLine("Conductor is Unavailable.");
        }*/

        ConductorLabel.Text = ConductorSB.ToString();
    }
}
