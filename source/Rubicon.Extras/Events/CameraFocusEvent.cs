using Godot;
using Rubicon.API.Events;
using Rubicon.Game;

namespace Rubicon.Extras.Events;

public class CameraFocusEvent : ISongEvent
{
    public string Name => "Set Camera Focus";

    public void OnTrigger(string[] args)
    {
        bool snap = args.Length > 1 && args[2].ToLower() == "true";
        if (int.TryParse(args[0], out int idx))
        {
            if (RubiconGame.Instance != null)
                RubiconGame.Instance.CameraController2D.FocusOnCameraPoint(idx, snap);
        }
        else
        {
            GD.PrintErr($"Failed to parse {args[0]} into an int. Not switching camera points.");   
        }
    }
}