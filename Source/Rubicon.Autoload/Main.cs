global using Godot;
global using Godot.Sharp.Extras;
global using System;

namespace Rubicon.Autoload;

public partial class Main : Node
{
    public static readonly string RubiconVersion = ProjectSettings.Singleton.GetSetting("application/config/version", "1").ToString();
}
