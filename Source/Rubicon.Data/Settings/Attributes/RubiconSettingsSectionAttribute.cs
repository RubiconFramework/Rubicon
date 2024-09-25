namespace Rubicon.Data.Settings.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class RubiconSettingsSectionAttribute : Attribute
{
	public string Section { get; set; }

	public RubiconSettingsSectionAttribute(string section)
	{
		Section = section;
	}
}