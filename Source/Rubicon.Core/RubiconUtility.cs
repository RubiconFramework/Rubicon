namespace Rubicon.Core;

/// <summary>
/// A general purpose utility class for Rubicon Engine
/// </summary>
public static class RubiconUtility
{
    /// <summary>
    /// Creates a number based on the versions provided below
    /// </summary>
    /// <param name="major">The major build</param>
    /// <param name="minor">The minor build</param>
    /// <param name="patch"></param>
    /// <param name="build"></param>
    /// <returns>A number based on the four versions</returns>
    public static uint CreateVersion(byte major, byte minor, byte patch, byte build) => ((uint)major << 24) | ((uint)minor << 16) | ((uint)patch << 8) | build;

    /// <summary>
    /// Creates a string representing the version in text form.
    /// </summary>
    /// <param name="version">The provided version</param>
    /// <returns>The version in text form</returns>
    public static string VersionToString(uint version) => $"{(version & 0xFF000000) >> 24}.{(version & 0x00FF0000) >> 16}.{(version & 0x0000FF00) >> 8}.{version & 0x000000FF}";
}