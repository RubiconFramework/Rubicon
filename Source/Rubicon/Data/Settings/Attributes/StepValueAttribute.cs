namespace Rubicon.Data.Settings.Attributes;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class StepValueAttribute : Attribute
{
	public float Step { get; set; }

	public StepValueAttribute(float step)
	{
		Step = step;
	}
}
