namespace Rubicon.Core.Autoload;

/// <summary>
/// A Node that contains basic engine info and may help with other utilities.
/// More useful in GDScript than it is in C#.
/// </summary>
[GlobalClass]
public partial class RubiconEngineSingleton : Node
{
	/// <summary>
	/// The scene that the game first starts with. Automatically set by <see cref="_Ready"/>.
	/// Will always be the main scene when exported, but can vary in editor.
	/// </summary>
	public string StartingScene;
	
	/// <summary>
	/// The type of node the starting scene is. Automatically set by <see cref="_Ready"/>.
	/// Will always be the main scene's type when exported, but can vary in editor.
	/// </summary>
	public Type StartingSceneType;
	
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

		Window root = GetTree().Root;
		
		// Link Conductor as well
		Conductor.Singleton = root.GetNode<ConductorSingleton>("Conductor");

		StartingScene = GetTree().CurrentScene.Name;
		StartingSceneType = GetTree().CurrentScene.GetType();
	}

	/// <inheritdoc cref="RubiconEngine.Version"/>
	public uint GetVersion() => RubiconEngine.Version;

	/// <inheritdoc cref="RubiconEngine.SubVersion"/>
	public string GetSubVersion() => RubiconEngine.SubVersion;

	/// <inheritdoc cref="RubiconEngine.VersionString"/>
	public string GetVersionString() => RubiconEngine.VersionString;
}
