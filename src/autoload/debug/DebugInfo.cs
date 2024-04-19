using System.Diagnostics;
using System.Globalization;
using System.Text;
using Godot.Sharp.Extras;

namespace Rubicon.autoload.debug;

[Icon("res://assets/miscicons/autoload.png")]
public partial class DebugInfo : CanvasLayer
{
    [NodePath("VBox/NonDebugLabel")] private Label NonDebugLabel;
    [NodePath("VBox/DebugLabel")] private Label DebugLabel;
    [NodePath("Version")] private Label DebugIndicator;

    private float updateTime;
    private bool showDebugInfo;
    private Process currentProcess = Process.GetCurrentProcess();

    private double byteToMB(long bytes) => bytes / (1024.0 * 1024.0);

    public override void _Ready()
    {
        this.OnReady();
        
        if (OS.IsDebugBuild()) DebugIndicator.Visible = true;
        else
        {
            DebugIndicator.QueueFree();
            DebugLabel.QueueFree();
        }
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
        DebugLabel.Visible = showDebugInfo;
    }

    long workingSet;
    StringBuilder debugText = new();

    private void UpdateText()
    {
        debugText.Clear();
        NonDebugLabel.Text = $"FPS: {Engine.GetFramesPerSecond().ToString(CultureInfo.InvariantCulture)}";
        
        if (OS.IsDebugBuild())
        {
            workingSet = (long)OS.GetStaticMemoryUsage();
            debugText.AppendLine($"RAM: {byteToMB(workingSet):F2} MB [Alloc: {byteToMB(currentProcess.PrivateMemorySize64):F2} MB] // VRAM: {byteToMB((long)Performance.GetMonitor(Performance.Monitor.RenderTextureMemUsed)):F2} MB")
                .AppendLine($"Scene: {(GetTree().CurrentScene != null && GetTree().CurrentScene.SceneFilePath != "" ? GetTree().CurrentScene.SceneFilePath : "None")}");
        }
        else
        {
            workingSet = currentProcess.WorkingSet64;
            debugText.AppendLine($"RAM: {byteToMB(workingSet):F2} MB [Alloc: {byteToMB(currentProcess.PrivateMemorySize64):F2} MB] // VRAM is Unavailable.")
                .AppendLine($"Scene: {(GetTree().CurrentScene != null && GetTree().CurrentScene.SceneFilePath != "" ? GetTree().CurrentScene.SceneFilePath : "None")}");
        }

        if (Conductor.Instance != null)
        {
            debugText.AppendLine("\n//Conductor Variables//")
                .AppendLine($"BPM: {Conductor.Instance.bpm}")
                .AppendLine($"Song Position: {Conductor.Instance.position}")
                .AppendLine($"Crochet: {Conductor.Instance.crochet} // StepCrochet: {Conductor.Instance.stepCrochet}")
                .AppendLine($"Step: {Conductor.Instance.curStep} // Beat: {Conductor.Instance.curBeat} // Section: {Conductor.Instance.curSection}")
                .Append($"Decimal Beat: {Conductor.Instance.curDecBeat} // Decimal Step: {Conductor.Instance.curDecStep} // Decimal Section: {Conductor.Instance.curDecSection}");
        }
        else debugText.AppendLine("Conductor is not available in this scene.");

        DebugLabel.Text = debugText.ToString();
    }
}
