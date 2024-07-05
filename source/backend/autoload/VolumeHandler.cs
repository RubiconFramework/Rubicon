
namespace Rubicon.Backend.Autoload;
public partial class VolumeHandler : Control
{
	private float Volume = 0.6f;
	public override void _Input(InputEvent @event) {
		if(@event is InputEventKey && (Input.IsActionJustPressed("volume_down") || Input.IsActionJustPressed("volume_up"))) {
			ChangeVolume(Input.GetAxis("volume_down","volume_up")/10);
		}
	}

	private void ChangeVolume(float amount)
	{
		Volume = Mathf.Clamp(Volume+amount,0,2);
		GD.Print($"Volume updated to {Volume} from value {amount}");
		UpdateVolume();
	}

	private void UpdateVolume()
	{
		AudioServer.SetBusVolumeDb(0,Mathf.LinearToDb(Volume));
	}
}
