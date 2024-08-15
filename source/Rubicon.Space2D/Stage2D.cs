using Godot;
using Godot.Collections;
using Rubicon.Data;
using Array = System.Array;

namespace Rubicon.Space2D;

public partial class Stage2D : Node2D
{
    public static Stage2D Instance;

    public CameraController2D CameraController;
    
    [Export] public SpawnPoint2D[] SpawnPoints;
    
    [Export] public Array<CharacterGroup2D> CharacterGroups = new Array<CharacterGroup2D>();

    [ExportGroup("Focus Points"), Export] public int MainFocus = 2; 
    
    [Export] public CameraFocusPoint2D[] FocusPoints;

    public override void _Ready()
    {
        if (Instance != null)
        {
            QueueFree();
            return;
        }

        Instance = this;
        base._Ready();
    }

    public CharacterGroup2D CreateCharacterGroup(string[] characterNames, int spawnPoint)
    {
        if (spawnPoint >= SpawnPoints.Length)
            GD.Print($"Spawn point index {spawnPoint} goes beyond the amount of character spawns existing ({SpawnPoints.Length}). Defaulting to index ${spawnPoint % SpawnPoints.Length}");

        SpawnPoint2D currentSpawnPoint = SpawnPoints[spawnPoint % SpawnPoints.Length];
        CharacterGroup2D characterGroup = new CharacterGroup2D();
        for (int i = 0; i < characterNames.Length; i++)
        {
            Character2D character = LoadCharacter(characterNames[i]);
            if (currentSpawnPoint.LeftFacing != character.Data.LeftFacing)
                character.Scale = new Vector2(character.Scale.X * -1f, character.Scale.Y);
            
            characterGroup.Characters.Add(character);
            currentSpawnPoint.Spawns[i % currentSpawnPoint.Spawns.Length].AddChild(character);
            character.Position = Vector2.Zero;
        }
        
        CharacterGroups.Add(characterGroup);
        return characterGroup;
    }
    
    public Character2D LoadCharacter(string characterName)
    {
        PackedScene packedCharacter = null;
        string path = $"res://{GameData.AssetsFolder}/characters/{characterName}/char2d.tscn";
        if (ResourceLoader.Exists(path))
        {
            packedCharacter = GD.Load<PackedScene>(path);
        }
        else
        {
            GD.PrintErr($"Character {characterName} at path {path} does not exist! Defaulting to funkin/bf");
            packedCharacter = GD.Load<PackedScene>($"res://{GameData.AssetsFolder}/characters/funkin/bf/char2d.tscn");
        }
            
        return packedCharacter.Instantiate<Character2D>();
    }
}