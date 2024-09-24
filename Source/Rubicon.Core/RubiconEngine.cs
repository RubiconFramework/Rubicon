namespace Rubicon.Core;

/// <summary>
/// A class for a global instance of <see cref="RubiconEngineSingleton"/>, as well as other static things.
/// </summary>
public static class RubiconEngine
{
	/// <summary>
	/// The global instance of <see cref="RubiconEngineSingleton"/>.
	/// </summary>
	public static RubiconEngineSingleton Singleton;
	
	/// <summary>
	/// The current version of Rubicon being used.
	/// </summary>
	public static readonly uint Version = RubiconUtility.CreateVersion(0, 1, 0, 0);
	
	/// <summary>
	/// A tag for the current version.
	/// </summary>
	public static readonly string SubVersion = "-alpha";

	/// <summary>
	/// The Rubicon version, in string format.
	/// </summary>
	public static string VersionString => RubiconUtility.VersionToString(Version) + SubVersion;
}
