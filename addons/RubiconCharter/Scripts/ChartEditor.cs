
using Charter.Scripts;

namespace Charter;

[Tool]
public partial class ChartEditor : Control
{
    public CharterPreferenceManager preferenceManager = new();

    public void CloseWindow(NodePath WindowPath)
    {
        GetNode<Window>(FixNodePath(WindowPath)).Visible = false;
    }

    public void ShowWindow(NodePath WindowPath)
    {
        GetNode<Window>(FixNodePath(WindowPath)).PopupCentered();
    }

    public void MakeFileDialog(NodePath LineEditPath)
    {
        #if TOOLS
        EditorFileDialog fileDialog = new()
        {
            Title = "Select a chart",
            FileMode = EditorFileDialog.FileModeEnum.OpenFile,
            //CurrentDir = "res://songs/",
            Size = new Vector2I(512, 512),
            InitialPosition = Window.WindowInitialPosition.CenterMainWindowScreen,
            Filters = ["*.tres"]
        };
        fileDialog.FileSelected += (string path) => GetNode<LineEdit>(FixNodePath(LineEditPath)).Text = path;
        AddChild(fileDialog);
        #endif
    }

    public void ShowAgainToggle(bool toggle)
    {
        preferenceManager.Preferences.ShowWelcomeWindow = toggle;
        //preferenceManager.Save();
    }

    private string FixNodePath(NodePath Path)
    {
        return Path.ToString().Replace("../../../../../../../", "");
    }
}
