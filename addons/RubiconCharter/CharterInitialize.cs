
[Tool]
public partial class CharterInitialize : EditorPlugin
{
    Control ChartEditorInstance = ResourceLoader.Load<PackedScene>("res://addons/RubiconCharter/ChartEditor.tscn").Instantiate<Control>();

    public override void _EnterTree()
    {
        GetViewport().GuiEmbedSubwindows = true;
        EditorInterface.Singleton.GetEditorMainScreen().AddChild(ChartEditorInstance);
    }

    public override void _ExitTree()
    {
        ChartEditorInstance.QueueFree();
        QueueFree();
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