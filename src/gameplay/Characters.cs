using System.Linq;
using Rubicon.gameplay.elements.scripts;

namespace Rubicon.gameplay;

public partial class GameplayScene
{
    public void GenerateCharacter(ref Character2D character, string characterType, Vector2 position)
    {
        if (character == null) throw new ArgumentNullException(nameof(character));
        string path3d = Song.Is3D ? "3D/" : "";
        string charPath = $"res://assets/gameplay/characters/{path3d + characterType}.tscn";

        character = ResourceLoader.Load<PackedScene>(charPath)?.Instantiate<Character2D>() ?? 
                    GD.Load<PackedScene>("res://assets/gameplay/characters/bf.tscn").Instantiate<Character2D>();

        if (character != null)
        {
            character.Position = position;
            AddChild(character);
        }
    }

    public void GenPlayer()
    {
        GenerateCharacter(ref player, Song.Player, stage.characterPositions["Player"]);
        player.isPlayer = true;
    }

    public void GenOpponent()
    {
        GenerateCharacter(ref opponent, Song.Opponent, stage.characterPositions["Opponent"]);

        if (Song.Opponent == Song.Spectator)
        {
            opponent.Position = stage.characterPositions["Spectator"];
            spectator.Visible = false;
        }
    }

    public void GenSpectator() => GenerateCharacter(ref spectator, Song.Spectator, stage.characterPositions["Spectator"]);

    public static void CharacterDance(Character2D charToDance, bool force = false)
    {
        if (charToDance?.danceOnBeat == true && (force || charToDance.lastAnim.StartsWith("sing"))) charToDance.dance();
    }

    private void HandleHoldAnimation()
    {
        if (pressed.Contains(true) || !player.lastAnim.StartsWith("sing") || player.holdTimer < Conductor.Instance.stepCrochet * player.singDuration * 0.0011)
            return;

        player.holdTimer = 0f;
        player.dance();
    }
}
