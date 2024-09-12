using System.Diagnostics;
using System.Linq;
using System.Text;
using Rubicon.Autoload;
using Rubicon.Core;
using Rubicon.Core.Chart;

namespace Rubicon.backend.autoload;

//[Icon("res://assets/misc/autoload.png")]
public partial class DebugInfo : CanvasLayer
{
    /*Main Info (Always Visible)*/
    //Performance
    [NodePath("InfoContainer/MainInformation/FPS")] private Label FPS;
    [NodePath("InfoContainer/MainInformation/RAM")] private Label RAM;
    [NodePath("InfoContainer/MainInformation/VRAM")] private Label VRAM;
    
    /*Debug Info (Keybind Based)*/
    //Scene Tree
    [NodePath("InfoContainer/DebugInformation/Objects/AllObjects")] private Label AllObjects;
    [NodePath("InfoContainer/DebugInformation/Objects/NodeObjects")] private Label NodeObjects;
    [NodePath("InfoContainer/DebugInformation/Objects/ResourceObjects")] private Label ResourceObjects;
    
    //Versions
    [NodePath("InfoContainer/DebugInformation/Versions/Game")] private Label GameVersion;
    [NodePath("InfoContainer/DebugInformation/Versions/Rubicon")] private Label RubiconVersion;
    [NodePath("InfoContainer/DebugInformation/Versions/Godot")] private Label GodotVersion;
    
    //Misc
    [NodePath("InfoContainer/DebugInformation/Scene")] private Label CurrentScene;
    [NodePath("InfoContainer/DebugInformation/Conductor")] private Label ConductorInfo;

    /*Visibility Shit*/
    [NodePath("InfoContainer/DebugInformation")] private VBoxContainer DebugInformation; 
    
    private Process CurrentProcess = Process.GetCurrentProcess();
    private float RAMUpdateTime;
    private float ObjectUpdateTime;
    
    public override void _Ready()
    {
        this.OnReady();
        if (!OS.IsDebugBuild()) VRAM.Visible = false;
        DebugInformation.VisibilityChanged += () =>
        {
            if (!DebugInformation.Visible) return;
            UpdateObjects();
            UpdateScene();
        };
        DebugInformation.Visible = false;
        UpdateStaticLabels();
    }
    
    private static string ConvertToMemoryFormat(long mem)
    {
        // Stole this from holofunk lol
        if (mem >= 0x40000000)
            return (float)Math.Round(mem / 1024f / 1024f / 1024f, 2) + " GB";
        if (mem >= 0x100000)
            return (float)Math.Round(mem / 1024f / 1024f, 2) + " MB";
        if (mem >= 0x400)
            return (float)Math.Round(mem / 1024f, 2) + " KB";

        return mem + " B";
    }
    
    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("DEBUG_INFO")) 
            DebugInformation.Visible = !DebugInformation.Visible;
        
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

        if (Conductor.Singleton != null)
        {
            UpdateConductor();
            ConductorInfo.Visible = true;
        }
        else ConductorInfo.Visible = false;
    }

    private void UpdateStaticLabels()
    {
        GameVersion.Text = $"{ProjectSettings.GetSetting("application/config/name").AsString()} {ProjectSettings.GetSetting("application/config/version").AsString()} {(OS.IsDebugBuild() ? "[Debug]" : "[Release]")}";
        RubiconVersion.Text = $"Rubicon Engine {RubiconEngine.VersionString}";
        GodotVersion.Text = $"Godot Engine {Engine.GetVersionInfo()["major"]}.{Engine.GetVersionInfo()["minor"]}.{Engine.GetVersionInfo()["patch"]} [{Engine.GetVersionInfo()["status"]}]";
        
        string GetKeybinds(Node node) => string.Join(", ", InputMap.ActionGetEvents(node.Name).OfType<InputEventKey>().Select(key => key.AsTextPhysicalKeycode()));
    }

    private void UpdateFPS() => FPS.Text = $"FPS: {Engine.GetFramesPerSecond()}";

    private void UpdateRAM() => RAM.Text = $"RAM: {ConvertToMemoryFormat(CurrentProcess.WorkingSet64)} [{ConvertToMemoryFormat(CurrentProcess.PrivateMemorySize64)}]";

    private void UpdateVRAM() => VRAM.Text = $"VRAM: {ConvertToMemoryFormat((long)Performance.GetMonitor(Performance.Monitor.RenderTextureMemUsed))}";

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

        ConductorSB.AppendLine($"Conductor BPM: {Conductor.Bpm} --- Current Position (s): {Conductor.RawTime}")
            .AppendLine("BPM List: {");
        foreach (BpmInfo bpm in Conductor.BpmList)
            ConductorSB.AppendLine(
                $"\t(Time: {bpm.Time}, Exact Time (ms): {bpm.MsTime}, BPM: {bpm.Bpm}, Time Signature: {bpm.TimeSignatureNumerator}/{bpm.TimeSignatureDenominator})");

        ConductorSB.AppendLine("}")
            .AppendLine($"Current Step: {Conductor.CurrentStep}")
            .AppendLine($"Current Beat: {Conductor.CurrentBeat}")
            .AppendLine($"Current Measure (Section): {Conductor.CurrentMeasure}");

        ConductorInfo.Text = ConductorSB.ToString();
    }
}
