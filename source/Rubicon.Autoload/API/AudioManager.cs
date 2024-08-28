using System;
using System.IO;
using System.Linq;
using Godot;
using Godot.Sharp.Extras;

namespace Rubicon.Autoload.API;

public enum AudioType
{
    Music,
    Sounds
}

[Icon("res://assets/misc/autoload.png")]
public partial class AudioManager : Node
{
    public static AudioManager Instance { get; private set; }
    public static readonly string[] AudioFileTypes = { ".ogg", ".mp3", ".wav" };

    public override void _EnterTree() => Instance = this;
    public override void _Ready() => this.OnReady();

    public static AudioStreamPlayer Play(AudioType type, string path, float volume = 1, bool loop = false, bool restart = false) => Instance.PlayAudio(type, path, volume, loop, restart);

    private AudioStreamPlayer PlayAudio(AudioType type, string path, float volume = 1, bool loop = false, bool restart = false)
    {
        string audioName = Path.GetFileNameWithoutExtension(path);
        AudioStreamPlayer player = FindExistingPlayer(type, audioName);

        if (player != null)
        {
            player.VolumeDb = Mathf.LinearToDb(volume);
            UpdateLoopSettings(player.Stream, loop);

            if (!restart && player.Playing) return player;
            player.Stop();
            player.Play();
            return player;
        }
        
        string fullPath = FindAudioPath(type, path);
        if (string.IsNullOrEmpty(fullPath))
        {
            GD.PrintErr($"Audio file not found: {path}");
            return null;
        }

        player = CreateAudioPlayer(fullPath, volume, loop);
        AttachPlayerToTree(type, player, audioName);

        player.Play();
        return player;
    }

    private AudioStreamPlayer FindExistingPlayer(AudioType type, string audioName)
    {
        string nodePath = $"{type}/{audioName}";
        return GetNodeOrNull<AudioStreamPlayer>(nodePath) ?? FindPlayerRecursively(GetNode(type.ToString()), audioName);
    }

    private static AudioStreamPlayer FindPlayerRecursively(Node parent, string name)
    {
        foreach (Node child in parent.GetChildren())
        {
            if (child is AudioStreamPlayer player && child.Name.ToString().Equals(name, StringComparison.OrdinalIgnoreCase)) 
                return player;
            
            var result = FindPlayerRecursively(child, name);
            if (result != null)
                return result;
        }
        return null;
    }

    private static AudioStreamPlayer CreateAudioPlayer(string path, float volume, bool loop)
    {
        AudioStream stream = GD.Load<AudioStream>(path);
        UpdateLoopSettings(stream, loop);

        return new AudioStreamPlayer
        {
            Stream = stream,
            VolumeDb = Mathf.LinearToDb(volume),
            Autoplay = false
        };
    }

    private static void UpdateLoopSettings(AudioStream stream, bool loop)
    {
        switch (stream)
        {
            case AudioStreamOggVorbis ogg:
                ogg.Loop = loop;
                break;
            case AudioStreamMP3 mp3:
                mp3.Loop = loop;
                break;
            case AudioStreamWav wav:
                wav.LoopMode = loop ? AudioStreamWav.LoopModeEnum.Forward : AudioStreamWav.LoopModeEnum.Disabled;
                break;
        }
    }

    private void AttachPlayerToTree(AudioType type, AudioStreamPlayer player, string name)
    {
        Node parentNode = GetNodeOrNull(type.ToString()) ?? CreateTypeNode(type);
        player.Name = name;
        parentNode.AddChild(player);

        player.Finished += () =>
        {
            if (!ShouldLoop(player.Stream)) 
                player.QueueFree();
        };
    }

    private static bool ShouldLoop(AudioStream stream)
    {
        return stream switch
        {
            AudioStreamOggVorbis ogg => ogg.Loop,
            AudioStreamMP3 mp3 => mp3.Loop,
            AudioStreamWav wav => wav.LoopMode != AudioStreamWav.LoopModeEnum.Disabled,
            _ => false
        };
    }

    private Node CreateTypeNode(AudioType type)
    {
        Node typeNode = new Node { Name = type.ToString() };
        AddChild(typeNode);
        return typeNode;
    }

    private static string FindAudioPath(AudioType type, string path)
    {
        string baseDir = $"res://assets/audio/{type.ToString().ToLower()}";
        
        foreach (var format in AudioFileTypes)
        {
            string formattedPath = $"{baseDir.PathJoin(path)}{format}";
            if (ResourceLoader.Exists(formattedPath)) 
                return formattedPath;
        }
        
        return SearchAudioRecursively(baseDir, Path.GetFileName(path));
    }

    private static string SearchAudioRecursively(string directory, string fileName)
    {
        var dir = DirAccess.Open(directory);
        if (dir == null)
        {
            GD.PrintErr($"An error occurred when trying to access the path: {directory}");
            return string.Empty;
        }

        dir.ListDirBegin();
        string filePath = dir.GetNext();
        while (filePath != string.Empty)
        {
            if (dir.CurrentIsDir())
            {
                string subDirPath = directory.PathJoin(filePath);
                string result = SearchAudioRecursively(subDirPath, fileName);
                if (result != string.Empty)
                {
                    dir.ListDirEnd();
                    return result;
                }
            }
            else if (Path.GetFileNameWithoutExtension(filePath).Equals(fileName, StringComparison.OrdinalIgnoreCase) 
                     && AudioFileTypes.Contains(Path.GetExtension(filePath).ToLower()))
            {
                dir.ListDirEnd();
                return directory.PathJoin(filePath);
            }
            filePath = dir.GetNext();
        }
        dir.ListDirEnd();
        return string.Empty;
    }
}
