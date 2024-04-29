namespace Rubicon.backend;

public partial class MariosMadnessReference : Control
{
    [NodePath("AnimationPlayer")] public AnimationPlayer AnimationPlayer;
    [NodePath("Song")] public Label Song;
    [NodePath("Composer")] public Label Composer;
    public float Duration;

    [NodePath("HSeparator/HSeparator2")] private HSeparator BGLine;
    [NodePath("HSeparator")] private HSeparator FGLine;
    
    public override void _Ready()
    {
        this.OnReady();
        
        AnimationPlayer.Play("In");
        AnimationPlayer.AnimationFinished += AnimFinished;
    }

    private async void AnimFinished(StringName name)
    {
        if (name == "In")
        {
            await ToSignal(GetTree().CreateTimer(Duration), SceneTreeTimer.SignalName.Timeout);
            AnimationPlayer.Play("Out");
        }
        else if (name == "Out")
            this.QueueFree();
    }
}
