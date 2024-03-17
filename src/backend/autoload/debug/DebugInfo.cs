using System.Diagnostics;
using System.Globalization;
using System.Text;
using Godot.Sharp.Extras;

namespace Rubicon.backend.autoload.debug;

[Icon("res://assets/miscicons/autoload.png")]
public partial class DebugInfo : CanvasLayer
{
    [NodePath("NonDebugLabel")] private Label NonDebugLabel;
    [NodePath("DebugLabel")] private Label DebugLabel;
    [NodePath("Version")] private Label DebugIndicator;

    private float updateTime;
    private bool showDebugInfo;
    private Process currentProcess;

    private double byteToMB(long bytes) => bytes / (1024.0 * 1024.0);

    public override void _Ready()
    {
        this.OnReady();
        currentProcess = Process.GetCurrentProcess();
        if (OS.IsDebugBuild()) DebugIndicator.Visible = true;
        else DebugLabel.QueueFree();
    }

    public override void _PhysicsProcess(double delta)
    {
        updateTime += (float)delta;
        if (updateTime >= 1)
        {
            updateTime = 0;
            UpdateText();
        }

        if(Input.IsActionJustPressed("debug_info")) showDebugInfo = !showDebugInfo;
        DebugLabel.Visible = showDebugInfo;
    }

    private void UpdateText()
    {
        NonDebugLabel.Text = $"FPS: {Engine.GetFramesPerSecond().ToString(CultureInfo.InvariantCulture)}";

        var currentScene = GetTree().CurrentScene;
        StringBuilder debugText = new();

        long workingSet;
        if (OS.IsDebugBuild())
        {
            workingSet = (long)OS.GetStaticMemoryUsage();
            double VRAM = Performance.GetMonitor(Performance.Monitor.RenderTextureMemUsed);
            debugText.AppendLine($"RAM: {byteToMB(workingSet):F2} MB")
                .AppendLine($"Private Memory: {byteToMB(currentProcess.PrivateMemorySize64):F2} MB")
                .AppendLine($"VRAM: {byteToMB((long)VRAM):F2} MB")
                .AppendLine($"Scene: {(currentScene != null && currentScene.SceneFilePath != "" ? currentScene.SceneFilePath : "None")}");
            
            if (Conductor.Instance != null)
            {
                debugText.AppendLine("\n//Conductor Variables//")
                    .AppendLine($"BPM: {Conductor.Instance.bpm}")
                    .AppendLine($"Position: {Conductor.Instance.position}")
                    .AppendLine($"Crochet: {Conductor.Instance.crochet}")
                    .AppendLine($"StepCrochet: {Conductor.Instance.stepCrochet}")
                    .AppendLine($"Step: {Conductor.Instance.curStep}")
                    .AppendLine($"Beat: {Conductor.Instance.curBeat}")
                    .AppendLine($"Section: {Conductor.Instance.curSection}")
                    .AppendLine($"Decimal Beat: {Conductor.Instance.curDecBeat}")
                    .AppendLine($"Decimal Step: {Conductor.Instance.curDecStep}")
                    .Append($"Decimal Section: {Conductor.Instance.curDecSection}");
            }
            else debugText.AppendLine("\n//Conductor Variables//").AppendLine("Conductor is Unavailable.");
        }
        else
        {
            workingSet = currentProcess.WorkingSet64;
            debugText.AppendLine($"RAM: {byteToMB(workingSet):F2} MB")
                .AppendLine($"Private Memory: {byteToMB(currentProcess.PrivateMemorySize64):F2} MB")
                .AppendLine("VRAM is Unavailable [Release Build].")
                .AppendLine($"Scene: {(currentScene != null && currentScene.SceneFilePath != "" ? currentScene.SceneFilePath : "None")}");

            if (Conductor.Instance != null)
            { 
                debugText.AppendLine("\n//Conductor Variables//")
                    .AppendLine($"BPM: {Conductor.Instance.bpm}")
                    .AppendLine($"Position: {Conductor.Instance.position}")
                    .AppendLine($"Crochet: {Conductor.Instance.crochet}")
                    .AppendLine($"StepCrochet: {Conductor.Instance.stepCrochet}")
                    .AppendLine($"Step: {Conductor.Instance.curStep}")
                    .AppendLine($"Beat: {Conductor.Instance.curBeat}")
                    .AppendLine($"Section: {Conductor.Instance.curSection}")
                    .AppendLine($"Decimal Beat: {Conductor.Instance.curDecBeat}")
                    .AppendLine($"Decimal Step: {Conductor.Instance.curDecStep}")
                    .Append($"Decimal Section: {Conductor.Instance.curDecSection}");
            }
            else debugText.AppendLine("\n//Conductor Variables//").AppendLine("Conductor is Unavailable.");
        }
        
        DebugLabel.Text = debugText.ToString();
    }
}
