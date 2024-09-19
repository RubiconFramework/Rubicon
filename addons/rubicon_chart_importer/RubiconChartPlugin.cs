#if TOOLS
using Rubicon.Extras;

[Tool]
public partial class RubiconChartPlugin : EditorPlugin
{
	private FunkinChartPlugin _funkin;
	
	public override void _EnterTree()
	{
		_funkin = new FunkinChartPlugin();
		AddImportPlugin(_funkin);
	}

	public override void _ExitTree()
	{
		RemoveImportPlugin(_funkin);
		_funkin.Free();
	}
}
#endif
