
using System.Threading;

namespace Rubicon.Backend.Autoload;
public partial class DebugInfo : CanvasLayer
{
	[NodePath("FPS")] private RichTextLabel fps;
	[NodePath("FPS/Info")] private Label info;
	[NodePath("Keys")] private Label keys;

	private float updateCooldown;
	private float vramPeak;
	public override void _Ready() {
		this.OnReady();
	}

	private float ByteToReadable(float bytes) => bytes / (1024 * 1024);

	public override void _PhysicsProcess(double delta) 
	{
		if (updateCooldown <= 0)
			updateCooldown = 0.5f;
		else if (updateCooldown > 0 && Visible)
			updateCooldown -= (float)delta;

		if(Input.IsActionJustPressed("debug_fps")) fps.Visible = !fps.Visible;
		if(Input.IsActionJustPressed("debug_info") && OS.IsDebugBuild())
		{
			fps.Visible = true;
			info.Visible = !info.Visible && fps.Visible;
		}
		if(Input.IsActionJustPressed("debug_keys") && OS.IsDebugBuild()) keys.Visible = !keys.Visible; 

		if (fps.Visible && updateCooldown <= 0)
		{
			string fpsColor = "white";
			if (Engine.GetFramesPerSecond() < 60) fpsColor = "yellow";
			if (Engine.GetFramesPerSecond() < 30) fpsColor = "red";

			fps.Text = $"FPS: [color={fpsColor}]{Engine.GetFramesPerSecond()}";
			if (info.Visible)
			{
				float currentVram = (float)Performance.GetMonitor(Performance.Monitor.RenderTextureMemUsed);
				if(currentVram > vramPeak) vramPeak = currentVram;
				
				info.Text = "> Memory Info" +
				$"\nRAM: {Math.Round(ByteToReadable(OS.GetStaticMemoryUsage()), 2)}MB / {Math.Round(ByteToReadable(OS.GetStaticMemoryPeakUsage()), 2)}MB" +
				$"\nVRAM: {Math.Round(ByteToReadable(currentVram), 2)}MB / {Math.Round(ByteToReadable(vramPeak), 2)}MB";
			}
		}
	}
}
