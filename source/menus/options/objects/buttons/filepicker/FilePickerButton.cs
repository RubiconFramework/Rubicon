namespace OldRubicon.scenes.options.objects.buttons.filepicker;

public partial class FilePickerButton : Button
{
    [NodePath("AnimationPlayer")] private AnimationPlayer AnimationPlayer;
    [NodePath("AnimationPlayer/FileDialog")] private FileDialog FileDialog;

    public override void _Ready()
    {
        this.OnReady();
        Pressed += () => AnimationPlayer.Play("Start");
        FileDialog.Canceled += () => AnimationPlayer.Play("End");
        
        FileDialog.FileSelected += file =>
        {
            Text += $" {file} ";
            AnimationPlayer.Play("End");
        };
    }
}
