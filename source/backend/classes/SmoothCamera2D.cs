
namespace Rubicon.backend.classes;
public partial class SmoothCamera2D : Camera2D
{
	[ExportGroup("Camera Smoothing")]
	[Export] public bool SmoothZoomEnabled = true;
	[Export] public float StaticZoom = 1;
	[Export] public float SmoothZoomSpeed = 15;

	public override void _Process(double delta)
	{
		if(SmoothZoomEnabled) {
			float FinalSpeed = (float)Mathf.Clamp(delta * SmoothZoomSpeed, 0, 1);
			Zoom = new Vector2(Mathf.Lerp(Zoom.X, StaticZoom, FinalSpeed),Mathf.Lerp(Zoom.Y, StaticZoom, FinalSpeed));
		}
	}
}
