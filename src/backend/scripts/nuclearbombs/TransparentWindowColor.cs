using System.Runtime.InteropServices;

namespace Rubicon.backend.scripts.nuclearbombs;

public static class TransparentWindowColor
{
    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_LAYERED = 0x80000;
    private const int LWA_COLORKEY = 0x1;
    
    [DllImport("user32.dll")] private static extern IntPtr GetActiveWindow();
    [DllImport("user32.dll")] private static extern int SetWindowLongA(IntPtr hWnd, int nIndex, int dwNewLong);
    [DllImport("user32.dll")] private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
    [DllImport("kernel32.dll")] private static extern uint GetLastError();

    public static void SetTransparency(uint color)
    {
        IntPtr win = GetActiveWindow();

        if (win == IntPtr.Zero)
        {
            GD.PrintErr("Error finding window!!! how!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            GD.PrintErr("Error code: " + GetLastError());
            return;
        }

        if (SetWindowLongA(win, GWL_EXSTYLE, WS_EX_LAYERED) == 0)
        {
            GD.PrintErr("Error setting window to layered, what the fuck lol");
            GD.PrintErr("Error code: " + GetLastError());
        }

        if (!SetLayeredWindowAttributes(win, color, 0, LWA_COLORKEY))
        {
            GD.PrintErr("Error setting color key, ok this ones not so bad");
            GD.PrintErr("Error code: " + GetLastError());
        }
    }
}