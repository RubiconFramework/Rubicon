using System.Collections;
using Godot;
using Rubicon.API;
using Rubicon.API.Coroutines;
using Rubicon.Data;
using Rubicon.Game;

namespace HoloFunk;

[SongBind("konkonbeats")]
public class KonkonVideo : IGameCoroutine
{
    public IEnumerator Execute()
    {
        PackedScene packedVideo =
            GD.Load<PackedScene>($"res://{GameData.AssetsFolder}/songs/konkonbeats/VideoScene.tscn");

        Control videoScene = packedVideo.Instantiate<Control>();
        RubiconGame.Instance.AddChild(videoScene);
        RubiconGame.Instance.MoveChild(videoScene, RubiconGame.Instance.ViewportContainer.GetIndex() + 1);

        VideoStreamPlayer video = videoScene.GetNode<VideoStreamPlayer>("AspectRatioContainer/VideoStreamPlayer");
        video.Volume = 0;
        video.Play();
        
        yield break;
    }
}