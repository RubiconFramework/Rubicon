namespace Rubicon.Data.Settings.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class RubiconSettingsSectionAttribute : Attribute
{
	public string SectionName { get; set; }
	public bool GeneratedInSettingsMenu { get; set; }
	public string SectionIconPath { get; set; }

	public RubiconSettingsSectionAttribute(string sectionName, bool generatedInSettingsMenu = true, string sectionIconPath = null)
	{
		SectionName = sectionName;
		GeneratedInSettingsMenu = generatedInSettingsMenu; 
		SectionIconPath = sectionIconPath;
	}
}