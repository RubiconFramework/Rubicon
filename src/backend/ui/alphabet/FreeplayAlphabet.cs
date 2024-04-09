
using Godot;

namespace Rubicon.backend.ui.alphabet;

[Tool]
[Icon("res://fnf_icon.png")]
public partial class FreeplayAlphabet : Alphabet
{
    [Export] private bool is_template;

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (!is_template && !Engine.IsEditorHint()) Visible = !(Position.X < -(Size.Y + 20) || Position.Y > Main.WindowSize.Y + 20);
    }
}
