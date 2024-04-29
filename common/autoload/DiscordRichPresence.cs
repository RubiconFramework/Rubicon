using DiscordRPC;
using DiscordRPC.Logging;

namespace Rubicon.common.autoload;

public partial class DiscordRichPresence : Node
{
    public const string ClientID = "1218405526677749760";

    public static DiscordRpcClient Client = new(ClientID);
    public static DiscordRichPresence Instance { get; private set; } 

    public override void _EnterTree() => Instance = this;
    public override void _ExitTree() => Instance = null;

    public void Toggle(bool enable)
    {
        try
        {
            if (enable)
            {
                if (Client.IsDisposed) Client = new(ClientID);
				
                if (!Client.IsInitialized)
                {
                    Client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };
                    Client.OnReady += (_, e) => GD.Print($"Discord RPC: Received Ready from user: {e.User.Username}");
                    Client.Initialize();
                }
				
                Client.SetPresence(new()
                {
                    Details = GetTree().CurrentScene?.Name ?? "Unknown Scene",
                    Assets = new()
                    {
                        LargeImageKey = "image_large",
                        LargeImageText = $"Version {ProjectSettings.Singleton.GetSetting("application/config/version", "1.0").ToString()} {(OS.IsDebugBuild() ? "Debug" : "Release")} Build",
                    }
                });
            }
            else
            {
                if (Client.IsInitialized)
                {
                    Client.ClearPresence();
                    Client.Dispose();
                }
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Error {(enable ? "initializing" : "disabling")} Discord RPC: {ex.Message}");
        }
    }
}
