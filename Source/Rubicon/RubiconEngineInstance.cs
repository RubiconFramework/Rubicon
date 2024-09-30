/*
 * Copyright 2024 Rubicon Team.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

global using Godot;
global using Godot.Sharp.Extras;
global using System;

using Rubicon.Data.Generation;

namespace Rubicon;

/// <summary>
/// A Node that contains basic engine info. Essentially the Main class.
/// More useful in GDScript than it is in C#.
/// </summary>
[GlobalClass, StaticAutoloadSingleton("Rubicon", "RubiconEngine")]
public partial class RubiconEngineInstance : Node
{
	/// <summary>
	/// The current version of Rubicon being used.
	/// </summary>
	public static uint Version => RubiconUtility.CreateVersion(0, 1, 0, 0);

	/// <summary>
	/// A tag for the current version.
	/// </summary>
	public static string VersionTag => "-alpha";

	/// <summary>
	/// The current Rubicon version, in string format.
	/// </summary>
	public static string VersionToString => RubiconUtility.VersionToString(Version) + VersionTag;
	
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
		// Override the current scale size with the one set in the Rubicon project settings
		// This is done so that the editor can stay in a 16:9 aspect ratio while keeping
		// the 4:3 support in-game typically.
		GetWindow().ContentScaleSize = ProjectSettings.GetSetting("rubicon/general/content_minimum_size").AsVector2I();

		StartingScene = GetTree().CurrentScene.Name;
		StartingSceneType = GetTree().CurrentScene.GetType();
	}

	/// <inheritdoc cref="Version"/>
	public uint GetVersion() => Version;

	/// <inheritdoc cref="VersionTag"/>
	public string GetVersionTag() => VersionTag;

	/// <inheritdoc cref="VersionToString"/>
	public string GetVersionToString() => VersionToString;
}