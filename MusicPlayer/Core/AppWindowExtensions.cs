using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
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
        /// Does what it says
        /// </summary>
        /// <returns>The MainWindow</returns>
        public static MainWindow GetMainWindow() => (MusicPlayer.MainWindow) App.Current.MainWindow;

        /// <summary>
        /// Opens a native Windows FolderPicker
        /// </summary>
        /// <param name="folderPicker">Change FolderPicker behaviour</param>
        /// <returns>Folder Selected</returns>
        public static async Task<StorageFolder> OpenFolderPicker(FolderPicker folderPicker = null)
        {
            folderPicker ??= new();

            MainWindow window = (MusicPlayer.MainWindow)App.Current.MainWindow;
            IntPtr hwnd = new WindowInteropHelper(window).EnsureHandle();
            InitializeWithWindow.Initialize(folderPicker, hwnd);

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            return folder;
        }

        /// <summary>
        /// Opens a native Windows FileOpenPicker
        /// </summary>
        /// <param name="filePicker">Change FileOpenPicker behaviour</param>
        /// <returns>File(s) Selected</returns>
        public static async Task<StorageFile> OpenFilePicker(FileOpenPicker filePicker = null)
        {
            filePicker ??= new();

            MainWindow window = (MusicPlayer.MainWindow)App.Current.MainWindow;
            IntPtr hwnd = new WindowInteropHelper(window).EnsureHandle();
            InitializeWithWindow.Initialize(filePicker, hwnd);

            StorageFile file = await filePicker.PickSingleFileAsync();
            return file;
        }

        private const int COLOR_WINDOWFRAME = 6;

        [DllImport("user32.dll")]
        private static extern uint GetSysColor(int nIndex);

        public static Color GetBorderColor(this Window window) {
            var hwnd = new WindowInteropHelper(window).EnsureHandle;
            var dpi = VisualTreeHelper.GetDpi(window);
            var borderThickness = SystemParameters.WindowResizeBorderThickness;
            var leftBorderWidth = (int)Math.Ceiling(borderThickness.Left * dpi.DpiScaleX);
            var topBorderHeight = (int)Math.Ceiling(borderThickness.Top * dpi.DpiScaleY);
            var rightBorderWidth = (int)Math.Ceiling(borderThickness.Right * dpi.DpiScaleX);
            var bottomBorderHeight = (int)Math.Ceiling(borderThickness.Bottom * dpi.DpiScaleY);
            var rect = new RECT {
                left = 0,
                top = 0,
                right = leftBorderWidth + rightBorderWidth,
                bottom = topBorderHeight + bottomBorderHeight
            };
            AdjustWindowRectEx(ref rect, (uint)window.WindowStyle, false, (uint)window.WindowState);
            var colorRef = GetSysColor(COLOR_WINDOWFRAME);
            var r = (byte)(colorRef & 0xff);
            var g = (byte)((colorRef >> 8) & 0xff);
            var b = (byte)((colorRef >> 16) & 0xff);
            return Color.FromRgb(r, g, b);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool AdjustWindowRectEx(ref RECT lpRect, uint dwStyle, bool bMenu, uint dwExStyle);
    }
}
