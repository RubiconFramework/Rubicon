using Rubicon.scenes.options.submenus.misc.enums;

namespace Rubicon.autoload.managers.transitionmanager;

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
        if (!Main.RubiconSettings.Misc.SceneTransitions)
        {
            GetTree().ChangeSceneToFile(path);
            await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
            Main.DiscordRpcClient.UpdateDetails(GetTree().CurrentScene.Name);
            return;
        }
        
        AnimationPlayer player = GetNode<AnimationPlayer>(Main.RubiconSettings.Misc.Transitions.ToString());
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
        if (!Main.RubiconSettings.Misc.SceneTransitions)
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
