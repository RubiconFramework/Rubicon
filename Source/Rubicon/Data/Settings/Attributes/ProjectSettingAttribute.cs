namespace Rubicon.Data.Settings.Attributes;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class ProjectSettingAttribute : Attribute
{
	public string SettingName { get; set; }
	public ProjectSettingAttribute(string name)
	{
		SettingName = name;
	}
}