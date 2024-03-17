using System.Runtime.InteropServices;

namespace Rubicon.backend.scripts.nuclearbombs;

public static class WindowOpacity
{
    const int GWL_EXSTYLE = -20;
    const int WS_EX_LAYERED = 0x80000;
    const int LWA_ALPHA = 0x2;
    
    [DllImport("user32.dll")] static extern IntPtr GetActiveWindow();
    [DllImport("user32.dll")] static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
    [DllImport("user32.dll")] static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    public static void Set(float opacity)
    {
        IntPtr hwnd = GetActiveWindow();
        SetWindowLong(hwnd, GWL_EXSTYLE, WS_EX_LAYERED);
        SetLayeredWindowAttributes(hwnd, 0, (byte)(opacity * 255), LWA_ALPHA);
    }
}