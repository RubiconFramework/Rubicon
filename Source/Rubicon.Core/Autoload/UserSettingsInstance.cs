using Rubicon.Data.Settings;
using Rubicon.Data.Settings.Attributes;

[GlobalClass]
public partial class UserSettingsInstance : Node
{
	[Export] public ConfigFile Data = new();
	public const string SettingsFilePath = "user://settings.cfg";
	public static GeneralSettings Settings { get; private set; } = new GeneralSettings();

	public UserSettingsInstance()
	{
		if (!Load())
		{
			Reset();
			Save();
		}
	}

	public bool Load()
	{
		if (FileAccess.FileExists(SettingsFilePath))
		{
			string configText = FileAccess.GetFileAsString(SettingsFilePath);
			if (Data.Parse(configText) == Error.Ok)
			{
				LoadSectionFromFile(Settings.Modifiers);
				LoadSectionFromFile(Settings.Gameplay);
				LoadSectionFromFile(Settings.Audio);
				LoadSectionFromFile(Settings.Video);
				LoadSectionFromFile(Settings.Misc);
				LoadSectionFromFile(Settings.Debug);
				return true;
			}
		}

		return false;
	}

	public bool Save()
	{
		SerializeSettings();
		if (Data.Save(SettingsFilePath) == Error.Ok)
			return true;

		return false;
	}

	public void Reset()
	{
		Settings = new GeneralSettings();
		SerializeSettings();
	}

	private void SerializeSettings()
	{
		SerializeSection(Settings.Modifiers);
		SerializeSection(Settings.Gameplay);
		SerializeSection(Settings.Audio);
		SerializeSection(Settings.Video);
		SerializeSection(Settings.Misc);
		SerializeSection(Settings.Debug);
	}

	private void SerializeSection(object section)
	{
		var type = section.GetType();
		var attribute = (RubiconSettingsSectionAttribute)Attribute.GetCustomAttribute(type, typeof(RubiconSettingsSectionAttribute));
		if (attribute != null)
		{
			foreach (var property in type.GetProperties()) 
			{
				object value = property.GetValue(section);

				Variant variantValue = value switch
				{
					bool b => b,
					int i => i,
					float f => f,
					string s => s,
					Enum e => Convert.ToInt32(e),
					_ => throw new InvalidOperationException($"Unsupported type: {value!.GetType()}")
				};

				Data.SetValue(attribute.Section, property.Name, variantValue);
			}
		}
	}
	
	private void LoadSectionFromFile(object section)
	{
		var type = section.GetType();
		var attribute = (RubiconSettingsSectionAttribute)Attribute.GetCustomAttribute(type, typeof(RubiconSettingsSectionAttribute));
	
		if (attribute != null)
		{
			foreach (var property in type.GetProperties())
			{
				if (Data.HasSectionKey(attribute.Section, property.Name))
				{
					Variant variantValue = Data.GetValue(attribute.Section, property.Name);
					switch (property.PropertyType.IsEnum)
					{
						case true:
							property.SetValue(section, Enum.ToObject(property.PropertyType, variantValue));
							break;
						default:
							switch (variantValue.VariantType)
							{
								// TODO: Support different Variant types
								case Variant.Type.Int:
									property.SetValue(section, variantValue.AsInt32());
									break;
								case Variant.Type.Float:
									property.SetValue(section, variantValue.AsDouble());
									break;
								case Variant.Type.Bool:
									property.SetValue(section, variantValue.AsBool());
									break;
								case Variant.Type.String:
									property.SetValue(section, variantValue.AsString());
									break;
								case Variant.Type.Array:
									property.SetValue(section, variantValue.AsGodotArray());
									break;
								case Variant.Type.Dictionary:
									property.SetValue(section, variantValue.AsGodotDictionary());
									break;
							}
							
							break;
					}
				}
			}
		}
	}
}
