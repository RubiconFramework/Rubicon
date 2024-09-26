
using Charter.Scripts;

namespace Charter;

[Tool]
public partial class ChartEditor : Control
{
    public CharterPreferenceManager preferenceManager = new();

    public void OnWindowClose(string WindowPath)
    {
        GetNode<Window>(WindowPath).Visible = false;
    }

    public void ShowWindow(string WindowPath)
    {
        GetNode<Window>(WindowPath).PopupCentered();
    }

    public void MakeFileDialog()
    {
        #if TOOLS
            EditorFileDialog fileDialog = new();
            fileDialog.Title = "Select a chart";
            fileDialog.FileMode = EditorFileDialog.FileModeEnum.OpenFile;
            fileDialog.Size = new Vector2I(512, 512);
            fileDialog.InitialPosition = Window.WindowInitialPosition.CenterMainWindowScreen;
            fileDialog.Filters = ["*.tres"];
            //fileDialog.FileSelected += FileSelected;
            AddChild(fileDialog);
        #endif
    }



    public void ShowAgainToggle(bool toggle)
    {
        preferenceManager.Preferences.ShowWelcomeWindow = toggle;
        //preferenceManager.Save();
    }
}
