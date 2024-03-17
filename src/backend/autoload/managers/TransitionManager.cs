namespace Rubicon.backend.autoload.managers;

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
        if (!Global.Settings.Misc.SceneTransitions)
        {
            GetTree().ChangeSceneToFile(path);
            await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
            Global.DiscordRpcClient.UpdateDetails(GetTree().CurrentScene.Name);
            return;
        }
        
        AnimationPlayer player = GetNode<AnimationPlayer>(Global.Settings.Misc.Transitions.ToString());
        player.Play("Start");
        player.AnimationFinished += TransitionFinished;
        async void TransitionFinished(StringName animName)
        {
            player.AnimationFinished -= TransitionFinished;
            GetTree().ChangeSceneToFile(path);
            player.Play("End");
            await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
            Global.DiscordRpcClient.UpdateDetails(GetTree().CurrentScene.Name);
        }
    }
}
