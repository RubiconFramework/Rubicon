using System;
using System.IO;
using Godot;

namespace Rubicon.Game.Utilities
{
    public static class AudioStreamUtil
    {
        public enum AudioStreamType
        {
            OggVorbis,
            Mp3,
            Wav
        }
        
        /// <summary>
        /// Loads an audio stream from the provided path and returns the audio stream retrieved.
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>The AudioStream retrieved</returns>
        public static AudioStream LoadStream(string path)
        {
            string pathNoExt = path.Replace(Path.GetExtension(path), "");
            switch (Path.GetExtension(path))
            {
                case ".ogg":
                    return LoadStream(pathNoExt, AudioStreamType.OggVorbis);
                case ".wav":
                    return LoadStream(pathNoExt, AudioStreamType.Wav);
                case ".mp3":
                    return LoadStream(pathNoExt, AudioStreamType.Mp3);
            }

            GD.Print($"Path \"{path}\" does not contain a valid extension!");
            return null;
        }

        /// <summary>
        /// Loads an audio stream from the provided path and audio stream type and returns the audio stream retrieved.
        /// </summary>
        /// <param name="path">The path</param>
        /// <param name="type">The type of audio file</param>
        /// <returns>The AudioStream retrieved</returns>
        public static AudioStream LoadStream(string path, AudioStreamType type)
        {
            switch (type)
            {
                case AudioStreamType.OggVorbis:
                {
                    if (!ResourceLoader.Exists(path + ".ogg"))
                    {
                        GD.Print($"Path for audio stream {path}.ogg was not found!");
                        return null;
                    }
                    
                    return GD.Load<AudioStreamOggVorbis>(path + ".ogg");
                }

                case AudioStreamType.Mp3:
                {
                    if (!ResourceLoader.Exists(path + ".mp3"))
                    {
                        GD.Print($"Path for audio stream {path}.mp3 was not found!");
                        return null;
                    }
                    
                    return GD.Load<AudioStreamMP3>(path + ".mp3");
                }
                case AudioStreamType.Wav:
                {
                    if (!ResourceLoader.Exists(path + ".wav"))
                    {
                        GD.Print($"Path for audio stream {path}.wav was not found!");
                        return null;
                    }
                    
                    return GD.Load<AudioStreamWav>(path + ".wav");
                }
            }

            return null;
        }

        public static AudioStreamPlayer CreatePlayer(string resPath, bool destroyOnComplete = true)
        {
            return CreatePlayer(LoadStream(resPath), destroyOnComplete);
        }
        
        public static AudioStreamPlayer CreatePlayer(string resPath, AudioStreamType type, bool destroyOnComplete = true)
        {
            return CreatePlayer(LoadStream(resPath, type), destroyOnComplete);
        }
        
        public static AudioStreamPlayer CreatePlayer(AudioStream stream, bool destroyOnComplete = true)
        {
            AudioStreamPlayer player = new AudioStreamPlayer();
            player.Stream = stream;

            if (destroyOnComplete)
            {
                player.Finished += () =>
                {
                    player.QueueFree();
                };
            }

            return player;
        }
    }
}