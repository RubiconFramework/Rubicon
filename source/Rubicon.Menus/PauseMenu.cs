using Godot;
using Rubicon.Game;
using Rubicon.Game.Utilities;

namespace Rubicon.Menus;

/// <summary>
/// Basic Rubicon pause menu.
/// </summary>
public partial class PauseMenu : BasePauseMenu
{
    /// <summary>
    /// Should activate when PauseOpened is invoked.
    /// </summary>
    private void OnPauseOpened()
    {
        Visible = true;
        AllowControl = true;
    }

    /// <summary>
    /// Should activate when DownPressed is invoked.
    /// </summary>
    private void OnDownPressed(bool wasPressed)
    {
        if (!wasPressed || !RubiconGame.Instance.Paused)
            return;

        Selection = (Selection + 1) % Options.Length;
            
        AudioStreamPlayer move = AudioStreamUtil.CreatePlayer(MoveCursor);
        AddChild(move);
        move.Play();
    }

    /// <summary>
    /// Should activate when UpPressed is invoked.
    /// </summary>
    private void OnUpPressed(bool wasPressed)
    {
        if (!wasPressed || !RubiconGame.Instance.Paused)
            return;

        Selection--;
        if (Selection < 0)
            Selection = Options.Length - 1;
            
        AudioStreamPlayer move = AudioStreamUtil.CreatePlayer(MoveCursor);
        AddChild(move);
        move.Play();
    }

    /// <summary>
    /// Should activate when ConfirmPressed is invoked.
    /// </summary>
    private void OnConfirmPressed(bool wasPressed)
    {
        if (!wasPressed)
            return;
            
        switch (Selection)
        {
            case 0: // Resume
                RubiconGame.Instance.Resume();
                Visible = false;
                break;
            case 1: // Restart
                // Nothing :(
                break;
            case 2: // Exit To Menu
                // Still nothing :(
                // GetTree().ChangeSceneToFile("res://scenes/menus/DebugMainMenu.tscn");
                break;
        }
    }
}