namespace Rubicon.Core;

/// <summary>
/// A Node that contains basic engine info and may help with other utilities.
/// More useful in GDScript than it is in C#.
/// </summary>
[GlobalClass]
public partial class RubiconEngineSingleton : Node
{
	public override void _Ready()
	{
		if (RubiconEngine.Singleton != null)
		{
			QueueFree();
			return;
		}

		RubiconEngine.Singleton = this;
		
		// Override the current scale size with the one set in the Rubicon project settings
		// This is done so that the editor can stay in a 16:9 aspect ratio while keeping
		// the 4:3 support in-game typically.
		GetWindow().ContentScaleSize = ProjectSettings.GetSetting("rubicon/general/content_minimum_size").AsVector2I();
		
		// Initialize Conductor as well
		Conductor.Singleton = GetTree().Root.GetNode<ConductorSingleton>("Conductor");
	}

	/// <inheritdoc cref="RubiconEngine.Version"/>
	public uint GetVersion() => RubiconEngine.Version;

	/// <inheritdoc cref="RubiconEngine.SubVersion"/>
	public string GetSubVersion() => RubiconEngine.SubVersion;

	/// <inheritdoc cref="RubiconEngine.VersionString"/>
	public string GetVersionString() => RubiconEngine.VersionString;
}
