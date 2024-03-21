using BaseRubicon.Scenes.Options.Elements;

namespace BaseRubicon.Scenes.Options.Submenus.Keybinds;

public partial class Keybinds : BaseSubmenu
{
    [NodePath("Container/NoteLeft")] private Button NoteLeft;
    [NodePath("Container/NoteRight")] private Button NoteRight;
    [NodePath("Container/NoteUp")] private Button NoteUp;
    [NodePath("Container/NoteDown")] private Button NoteDown;

    public override void _Ready()
    {
        base._Ready();
        this.OnReady();
    }
}
