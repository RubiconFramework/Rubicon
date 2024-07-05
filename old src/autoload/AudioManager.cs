using OldRubicon.autoload.enums;

namespace OldRubicon.autoload;

//[Icon("res://assets/miscicons/autoload.png")]
public partial class AudioManager : Node
{
	public static AudioManager Instance { get; private set; }

	public AudioStreamPlayer music;

	public override void _EnterTree()
	{
		base._EnterTree();
		Instance = this;
	}

	public override void _Ready()
	{
		this.OnReady();
    }

    public AudioStreamPlayer PlayAudio(AudioType type, string path, float volume = 1, bool loop = false)
    {
        GD.Print("PlayAudio called with type: " + type + ", path: " + path + ", volume: " + volume + ", loop: " + loop);

        string nodePath = $"{type.ToString().ToLower()}/{path}";
        GD.Print("Checking for existing player at path: " + nodePath);
        AudioStreamPlayer player = GetNodeOrNull<AudioStreamPlayer>(nodePath);

        if (player != null && type == AudioType.Music && player.Playing && player.Stream ==
            GD.Load<AudioStream>(ConstructAudioPath(path, type.ToString().ToLower())))
        {
            GD.Print("Found existing player: " + player.GetPath());
            return player;
        }

        if (player == null)
        {
            GD.Print("Player not found, creating new player");
            string finalPath = ConstructAudioPath(path, type.ToString().ToLower());
            if (string.IsNullOrEmpty(finalPath))
            {
                GD.PrintErr("Audio file not found for path: " + path);
                return null;
            }

            GD.Print("Loading audio stream from: " + finalPath);
            var audiostream = GD.Load<AudioStream>(finalPath);
            player = new AudioStreamPlayer
            {
                Stream = audiostream,
                VolumeDb = LinearToDB(volume),
                Autoplay = true
            };

            GD.Print("Creating new player node: " + player.GetPath());

            Node parentNode = GetNodeOrNull<Node>(type.ToString().ToLower());
            if (parentNode == null)
            {
                GD.Print("Parent node not found, creating new parent node");
                parentNode = new Node();
                AddChild(parentNode);
            }

            GD.Print("Adding player node to parent node: " + parentNode.GetPath());
            parentNode.AddChild(player);
            player.Finished += () =>
            {
                GD.Print("Player finished: " + player.GetPath());
                player.QueueFree();
            };
        }

        player.Finished += () =>
        {
            if (loop)
            {
                GD.Print("Looping player: " + player.GetPath());
                player.Play();
            }
        };

        GD.Print("Playing player: " + player.GetPath());
        player.Play();

        if (type == AudioType.Music)
        {
            GD.Print("Setting current music player: " + player.GetPath());
            music = player;
        }

        return player;
    }

    public AudioStreamPlayer PlayMusic(string path, float volume = 1f)
    {
        AudioStreamPlayer player = new()
        {
            Stream = GD.Load<AudioStream>(path),
            VolumeDb = LinearToDB(volume),
            Autoplay = false
        };
        return player;
    }

    public void StopAudio(AudioType type, string path)
    {
        var player = GetNodeOrNull<AudioStreamPlayer>($"{type.ToString().ToLower()}/{path}");
        player?.Stop();
        player?.QueueFree();
    }

    private static string ConstructAudioPath(string path, string type)
    {
        /*foreach (var format in Main.AudioFormats)
        {
            string formattedPath = $"res://assets/{type}/{path}.{format}";
            if (ResourceLoader.Exists(formattedPath))
                return formattedPath;
        }*/

        return string.Empty;
    }
	
	public static float LinearToDB(float linear)
	{
		if (linear <= 0) return -80.0f;
		return (float)Math.Log10(linear) * 20;
	}
}
