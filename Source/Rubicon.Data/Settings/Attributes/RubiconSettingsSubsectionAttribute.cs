namespace Rubicon.Data.Settings.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class RubiconSettingsSubsectionAttribute : Attribute
{
	public string SectionName { get; set; }
	public RubiconSettingsSubsectionAttribute(string sectionName)
	{
		SectionName = sectionName;
	}
}