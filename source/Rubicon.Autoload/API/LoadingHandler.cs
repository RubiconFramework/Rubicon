using Godot;
using Rubicon.Data;

namespace Rubicon.Autoload.API;

[Icon("res://assets/misc/autoload.png")]
public partial class LoadingHandler : CanvasLayer
{
    public static LoadingHandler Instance { get; private set; }
    public static string LastScenePath = "";
    public static string NewScenePath = "";
    public static bool IsLoading;
    public static readonly Godot.Collections.Array LoadingProgress = new();
    private static bool transitioning;

    public LoadingHandler()
    {
        Instance = this;
    }

    public static void ChangeScene(string newScene, bool loadingScreen = false)
    {
        NewScenePath = newScene;
        Instance.ChangeSceneWithTransition(loadingScreen ? "res://source/backend/LoadingScreen.tscn" : newScene);
    }
    
    public static void ChangeScene(string newScene, TransitionType forcedTransitionType, bool loadingScreen = false)
    {
        NewScenePath = newScene;
        Instance.ChangeSceneWithTransition(loadingScreen ? "res://source/backend/LoadingScreen.tscn" : newScene, forcedTransitionType);
    }

    public async void ChangeSceneWithTransition(string path)
    {
        if (!SaveData.Misc.SceneTransitions)
        {
            GetTree().ChangeSceneToFile(path);
            await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
            if (path == "res://source/backend/LoadingScreen.tscn") LoadScene();
            return;
        }

        AnimationPlayer player = GetNode<AnimationPlayer>(SaveData.Misc.Transitions.ToString());
        player.Play("Start");
        player.AnimationFinished += TransitionFinished;

        async void TransitionFinished(StringName animName)
        {
            player.AnimationFinished -= TransitionFinished;
            GetTree().ChangeSceneToFile(path);
            player.Play("End");
            await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);

            if (path == "res://source/backend/LoadingScreen.tscn") LoadScene();
        }
    }

    public async void ChangeSceneWithTransition(string path, TransitionType forcedTransitionType)
    {
        if (!SaveData.Misc.SceneTransitions)
        {
            GetTree().ChangeSceneToFile(path);
            await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
            if (path == "res://source/backend/LoadingScreen.tscn") LoadScene();
            return;
        }

        AnimationPlayer player = GetNode<AnimationPlayer>(forcedTransitionType.ToString());
        player.Play("Start");
        player.AnimationFinished += TransitionFinished;

        async void TransitionFinished(StringName animName)
        {
            player.AnimationFinished -= TransitionFinished;
            GetTree().ChangeSceneToFile(path);
            player.Play("End");
            await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
            if (path == "res://source/backend/LoadingScreen.tscn") LoadScene();
        }
    }

    private static void LoadScene()
    {
        if (ResourceLoader.Exists(NewScenePath))
        {
            ResourceLoader.LoadThreadedRequest(NewScenePath, "", true, ResourceLoader.CacheMode.Ignore);
            IsLoading = true;
            LoadingProgress.Clear();
            GD.Print("Loading new scene...");
        }
        else GD.PushError("New scene file was not found.");
    }

    public override void _Process(double delta)
    {
        if (!IsLoading) return;

        ResourceLoader.ThreadLoadStatus status = ResourceLoader.LoadThreadedGetStatus(NewScenePath, LoadingProgress);
        GD.Print($"Loading Progress: {(LoadingProgress.Count > 0 ? LoadingProgress[0].ToString() : "0")}");

        switch (status)
        {
            case ResourceLoader.ThreadLoadStatus.Loaded when !transitioning:
                GD.Print("New scene loaded.");
                transitioning = true;
                IsLoading = false;

                Resource loadedScene = ResourceLoader.LoadThreadedGet(NewScenePath);
                SceneTreeTimer timer = GetTree().CreateTimer(1);
                timer.Connect("timeout", Callable.From(async () => {
                    transitioning = false;
                    AnimationPlayer player = GetNode<AnimationPlayer>(SaveData.Misc.Transitions.ToString());
                    player.Play("Start");
                    await ToSignal(player, "animation_finished");
                    GetTree().ChangeSceneToPacked((PackedScene)loadedScene);
                    if (DiscordHandler.Client.IsInitialized) DiscordHandler.Client.UpdateDetails(GetTree().CurrentScene.Name);
                    player.Play("End");
                }));
                break;

            case ResourceLoader.ThreadLoadStatus.InProgress:
                break;

            case ResourceLoader.ThreadLoadStatus.InvalidResource:
            case ResourceLoader.ThreadLoadStatus.Failed:
            default:
                GD.Print("Failed to load the new scene.");
                break;
        }
    }
}
