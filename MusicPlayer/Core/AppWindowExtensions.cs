using System.Windows;
using System.Windows.Interop;

using Microsoft.UI;
using Microsoft.UI.Windowing;

namespace MusicPlayer.Core
{
    public static class AppWindowExtensions
    {
        public static AppWindow GetAppWindowFromWPFWindow(this Window wpfWindow)
        {
            // Get the HWND of the top level WPF window.
            var hwnd = new WindowInteropHelper(wpfWindow).EnsureHandle();

            // Get the WindowId from the HWND.
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hwnd);

            // Return AppWindow from the WindowId.
            return AppWindow.GetFromWindowId(windowId);
        }
    }
}
