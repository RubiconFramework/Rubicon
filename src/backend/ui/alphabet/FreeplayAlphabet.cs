using Global = BaseRubicon.Backend.Autoload.Global;

namespace BaseRubicon.Backend.UI.Alphabet;

[Tool]
[Icon("res://fnf_icon.png")]
public partial class FreeplayAlphabet : Alphabet
{
    [Export] private bool is_template;

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (!is_template && !Engine.IsEditorHint()) Visible = !(Position.X < -(Size.Y + 20) || Position.Y > Global.windowSize.Y + 20);
    }
}
