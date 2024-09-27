namespace Rubicon.Data.Generation;

/// <summary>
/// Used to mark classes that should be a singleton. Generates a static class so it's easier to use in C#.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class StaticAutoloadSingletonAttribute(string nameSpace, string className) : Attribute
{
    public string ClassName { get; private set; } = className;

    public string Namespace { get; private set; } = nameSpace;
}