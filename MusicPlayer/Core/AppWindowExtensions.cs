using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace MusicPlayer.Core
{
    public static class AppWindowExtensions
    {
        public static AppWindow GetAppWindowFromWPFWindow(this Window wpfWindow)
        {
            // Get the HWND of the top level WPF window.
            IntPtr hwnd = new WindowInteropHelper(wpfWindow).EnsureHandle();

            // Get the WindowId from the HWND.
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hwnd);

            // Return AppWindow from the WindowId.
            return AppWindow.GetFromWindowId(windowId);
        }

        /// <summary>
        /// Opens a native Windows FolderPicker
        /// </summary>
        /// <param name="folderPicker">Change FolderPicker behaviour</param>
        /// <returns></returns>
        public static async Task<StorageFolder> OpenFolderPicker(FolderPicker folderPicker = null)
        {
            if (folderPicker == null)
                folderPicker = new();

            MainWindow window = (MusicPlayer.MainWindow)App.Current.MainWindow;
            IntPtr hwnd = new WindowInteropHelper(window).EnsureHandle();
            InitializeWithWindow.Initialize(folderPicker, hwnd);

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            return folder;
        }
    }
}
