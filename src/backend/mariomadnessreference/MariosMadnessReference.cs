namespace Rubicon.backend.mariomadnessreference;

public partial class MariosMadnessReference : Control
{
    [NodePath("AnimationPlayer")] public AnimationPlayer AnimationPlayer;
    [NodePath("HBoxContainer")] public HBoxContainer HBoxContainer;
    
    [NodePath("HBoxContainer/Rubicon/Song")] public Label Song;
    [NodePath("HBoxContainer/Rubicon/Composer")] public Label Composer;
    
    public override void _Ready() => this.OnReady();

    public void StartIntro(string songName, string songComposer)
    {
        Song.Text = songName;
        Composer.Text = songComposer;

        float newSizeX = HBoxContainer.Size.X;
    
        if (Song.Size.X > Composer.Size.X)
            if (Song.Size.X > HBoxContainer.Size.X) newSizeX = Song.Size.X + 5;
        
        else if (Composer.Size.X > Song.Size.X)
            if (Composer.Size.X > HBoxContainer.Size.X) newSizeX = Composer.Size.X + 5;
        
        if (!newSizeX.Equals(HBoxContainer.Size.X)) 
            HBoxContainer.Size = new(newSizeX, HBoxContainer.Size.Y);

        AnimationPlayer.Play("In");
    }

    public void Finish()
    {
        AnimationPlayer.Play("Out");
        AnimationPlayer.AnimationFinished += _ =>
        {
            if (_ == "Out") QueueFree();
        };
    }
}
