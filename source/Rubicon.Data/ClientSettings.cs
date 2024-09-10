namespace Rubicon.Data;

public partial class ClientSettings : Resource
{
    [Export] public GameplaySettings GameplaySettings = new();
    [Export] public AudioSettings AudioSettings = new();
    [Export] public VideoSettings VideoSettings = new();
    [Export] public MiscSettings MiscSettings = new();
}

public partial class GameplaySettings : Resource
{
    [Export] public bool Downscroll = false;
}

public partial class AudioSettings : Resource
{
    [Export] public int killlll = 0;
}

public partial class VideoSettings : Resource
{
    [Export] public bool killpeople = false;
}

public partial class MiscSettings : Resource
{
    [Export] public bool arrestifkillpeople = false;
}
