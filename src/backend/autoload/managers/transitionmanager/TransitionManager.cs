using Rubicon.Scenes.Options.Submenus.Misc.Enums;

namespace Rubicon.Backend.Autoload.Managers.TransitionManager;

[Icon("res://assets/miscicons/autoload.png")]
public partial class TransitionManager : CanvasLayer
{
    public static TransitionManager Instance { get; private set; }

    public override void _EnterTree()
    {
        base._EnterTree();
        Instance = this;
    }

    public async void ChangeScene(string path)
    {
        if (!Main.GameSettings.Misc.SceneTransitions)
        {
            GetTree().ChangeSceneToFile(path);
            await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
            Main.DiscordRpcClient.UpdateDetails(GetTree().CurrentScene.Name);
            return;
        }
        
        AnimationPlayer player = GetNode<AnimationPlayer>(Main.GameSettings.Misc.Transitions.ToString());
        player.Play("Start");
        player.AnimationFinished += TransitionFinished;
        async void TransitionFinished(StringName animName)
        {
            player.AnimationFinished -= TransitionFinished;
            GetTree().ChangeSceneToFile(path);
            player.Play("End");
            await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
            Main.DiscordRpcClient.UpdateDetails(GetTree().CurrentScene.Name);
        }
    }
    
    public async void ChangeScene(string path, TransitionType transitionType)
    {
        if (!Main.GameSettings.Misc.SceneTransitions)
        {
            GetTree().ChangeSceneToFile(path);
            await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
            Main.DiscordRpcClient.UpdateDetails(GetTree().CurrentScene.Name);
            return;
        }

        AnimationPlayer player = GetNode<AnimationPlayer>(transitionType.ToString());
        player.Play("Start");
        player.AnimationFinished += TransitionFinished;
        async void TransitionFinished(StringName animName)
        {
            player.AnimationFinished -= TransitionFinished;
            GetTree().ChangeSceneToFile(path);
            player.Play("End");
            await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
            Main.DiscordRpcClient.UpdateDetails(GetTree().CurrentScene.Name);
        }
    }

}
