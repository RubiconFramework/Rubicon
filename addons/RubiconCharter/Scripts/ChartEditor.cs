
using Charter.Scripts;

namespace Charter;

[Tool]
public partial class ChartEditor : Control
{
    public CharterPreferenceManager preferenceManager = new();

    public void OnWindowClose()
    {
        GetNode<Window>("WelcomeWindow").QueueFree();
    }

    public void ShowAgainToggle(bool toggle)
    {
        preferenceManager.Preferences.ShowWelcomeWindow = toggle;
        preferenceManager.Save();
    }
}
