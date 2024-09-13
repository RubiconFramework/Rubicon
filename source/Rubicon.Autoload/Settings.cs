using Rubicon.Data;

namespace Rubicon.Autoload;
public partial class Settings : Node
{
	private const string FilePath = "user://settings.tres";
	
	public static GeneralSettings General { get; private set; } = new();
	
	public override void _Ready()
	{
		base._Ready();
		Read();
	}

	public static void Read()
	{
		if (!ResourceLoader.Exists(FilePath))
		{
			Save();
			return;
		}
		
		Resource settingsRes = GD.Load<Resource>(FilePath);
		if (settingsRes is not GeneralSettings general)
		{
			Save();
			return;
		}
		
		General = general;
	}

	public static void Save()
	{
		ResourceSaver.Save(General, FilePath);
		GD.Print($"Successfully saved settings to file: {FilePath}");
	}
}
