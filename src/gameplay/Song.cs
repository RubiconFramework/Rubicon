using System.Collections.Generic;
using System.Linq;
using Rubicon.backend.autoload.debug.ScreenNotifier;
using Rubicon.backend.common.enums;
using Rubicon.gameplay.elements.resources;
using Global = Rubicon.backend.autoload.Global;

namespace Rubicon.gameplay;

public partial class GameplayScene
{
    private Chart Song;
    private readonly List<AudioStreamPlayer> tracks = new();
    private AudioStreamPlayer inst, vocals;

    public void StartSong()
    {
        Conductor.Instance.position = 0f;
        foreach (AudioStreamPlayer track in tracks) track.Play((float)Conductor.Instance.position / 1000);
        startingSong = false;
    }

    public void EndSong()
    {
        // Handle end of the song
    }

    public void LoadSong()
    {
        if (Global.Song == null)
        {
            Global.Song = Chart.LoadChart("imscared", "normal");
            Song = Global.Song;
        }

        Conductor.Instance.MapBPMChanges(Song);
        Conductor.Instance.bpm = Song.Bpm;
        Conductor.Instance.position = Conductor.Instance.stepCrochet * 5;

        string songPath = $"res://assets/songs/{Song.SongName.ToLower()}/song/";

        foreach (var f in Global.audioFormats)
        {
            if (ResourceLoader.Exists($"{songPath}inst.{f}"))
            {
                inst.Stream = GD.Load<AudioStream>($"{songPath}inst.{f}");
                if (vocals.Stream == null && ResourceLoader.Exists($"{songPath}voices.{f}"))
                {
                    vocals.Stream = GD.Load<AudioStream>($"{songPath}voices.{f}");
                    tracks.Add(vocals);
                }
            }
        }

        if (inst.Stream == null)
        {
            ScreenNotifier.Instance.Notify($"Inst not found on path: '{songPath}inst' with every format, ending song", true, NotificationType.Error);
            endingSong = true;
            EndSong();
            return;
        }

        foreach (AudioStreamPlayer track in tracks) track.PitchScale = Conductor.Instance.rate;
    }
    
    private void UpdateSongProgress(double delta)
    {
        if (endingSong || tracks == null || tracks.Count == 0 || tracks[0].Stream == null)
        {
            if (endingSong) GD.Print("Song has ended. Returning");
            else GD.PrintErr("There's nothing in tracks. Returning");
            return;
        }

        Conductor.Instance.position += (float)delta * 1000f * Conductor.Instance.rate;

        if (Conductor.Instance.position >= tracks[0].Stream.GetLength())
        {
            EndSong();
            return;
        }

        if (Conductor.Instance.position >= 0f && startingSong) StartSong();
    }

    public void SyncSong()
    {
        foreach (var track in tracks.Where(track => track.Stream != null)) 
            track.Play((float)Conductor.Instance.position / 1000f);
    }

    public void GameOver()
    {
        // Handle game over scenario
    }

    public void SkipIntro()
    {
        // Handle skipping the intro
    }
}
