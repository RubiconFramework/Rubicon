using Rubicon.common.autoload.managers.enums;

namespace Rubicon.common.autoload.managers;

[Icon("res://assets/miscicons/autoload.png")]
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

		var Audio = Main.RubiconSettings.Audio;
		foreach (var volumeSetting in new[] {Audio.MasterVolume, Audio.SFXVolume, Audio.InstVolume, Audio.MusicVolume, Audio.VoiceVolume}) 
			VolumeManager.Instance.ChangeVolume(volumeSetting);
	}

	public AudioStreamPlayer PlayAudio(AudioType type, string path, float volume = 1, bool loop = false)
	{
		if (type == AudioType.Music)
		{
			foreach(var node in GetNode(type.ToString().ToLower()).GetChildren())
			{
				var playerBullshit = (AudioStreamPlayer)node;
				playerBullshit.Stop();
			}   
		}

		var player = GetNodeOrNull<AudioStreamPlayer>($"{type.ToString().ToLower()}/{path}");
		if (player != null && type == AudioType.Music && player.Playing && player.Stream == GD.Load<AudioStream>(ConstructAudioPath(path, type.ToString().ToLower()))) return player;

		if (player is null)
		{
			string finalPath = ConstructAudioPath(path, type.ToString().ToLower());
			if (string.IsNullOrEmpty(finalPath)) return null;
			var audiostream = GD.Load<AudioStream>(finalPath);

			player = new()
			{
				Stream = audiostream,
				VolumeDb = LinearToDB(volume),
				Autoplay = true,
			};
		
			GetNode<Node>($"{type.ToString().ToLower()}/{path}").AddChild(player);
			player.Finished += () => player.QueueFree();
		}

		player.Finished += () =>
		{
			if (loop && type == AudioType.Music) player.Play();
		};

		player.Play();
		if (type == AudioType.Music) music = player;
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
		foreach (var format in Main.AudioFormats)
		{
			string formattedPath = $"res://assets/{type}/{path}.{format}";
			if (ResourceLoader.Exists(formattedPath)) return formattedPath;
		}
		return string.Empty;
	}
	
	public static float LinearToDB(float linear)
	{
		if (linear <= 0) return -80.0f;
		return (float)Math.Log10(linear) * 20;
	}
}
