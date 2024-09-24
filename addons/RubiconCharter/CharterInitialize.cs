
using Charter;
using Charter.Scripts;

[Tool] public partial class CharterInitialize : EditorPlugin
{
    ChartEditor ChartEditorInstance = ResourceLoader.Load<PackedScene>("res://addons/RubiconCharter/ChartEditor.tscn").Instantiate<ChartEditor>();
    CharterPreferenceManager preferenceManager = new();
    bool ShownWelcomeWindow = false;

    public override void _EnterTree()
    {
        ChartEditorInstance.GetNode<Window>("WelcomeWindow").Visible = false;
        ChartEditorInstance.preferenceManager = preferenceManager;
        EditorInterface.Singleton.GetEditorMainScreen().AddChild(ChartEditorInstance);

        preferenceManager.Load();
        GD.Print(preferenceManager.Preferences.ShowWelcomeWindow);

        MainScreenChanged += _MainScreenChanged;
    }

    public override void _ExitTree()
    {
        MainScreenChanged -= _MainScreenChanged;

        ChartEditorInstance.QueueFree();
        QueueFree();
    }

    public void _MainScreenChanged(string screenName)
    {
        if (screenName == "Chart Editor" && preferenceManager.Preferences.ShowWelcomeWindow && !ShownWelcomeWindow)
        {
            ChartEditorInstance.GetNode<Window>("WelcomeWindow").PopupCentered();
            ShownWelcomeWindow = true;
        }
    }

    public override bool _HasMainScreen()
    {
        return true;
    }

    public override string _GetPluginName()
    {
        return "Chart Editor";
    }

    public override Texture2D _GetPluginIcon()
    {
        return EditorInterface.Singleton.GetEditorTheme().GetIcon("Node", "EditorIcons");
    }
}