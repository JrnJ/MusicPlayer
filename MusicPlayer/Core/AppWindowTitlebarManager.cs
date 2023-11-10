using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.UI;
using Microsoft.UI.Windowing;
using System.Runtime.InteropServices;
using MusicPlayer.Core;
using MusicPlayer.MVVM.ViewModel;
using static System.Net.Mime.MediaTypeNames;

namespace MusicPlayer.Core
{
    internal class AppWindowTitlebarManager : ObservableObject
    {
        [DllImport("Shcore.dll", SetLastError = true)]
        internal static extern int GetDpiForMonitor(IntPtr hmonitor, Monitor_DPI_Type dpiType, out uint dpiX, out uint dpiY);

        internal enum Monitor_DPI_Type : int
        {
            MDT_Effective_DPI = 0,
            MDT_Angular_DPI = 1,
            MDT_Raw_DPI = 2,
            MDT_Default = MDT_Effective_DPI
        }

        // Windowing
        public static AppWindow AppWindow { get; private set; }
        private MainWindow MainWindow { get; }

        // Properties
        private int _titlebarHeight;

        public int TitlebarHeight
        {
            get { return _titlebarHeight; }
            set { _titlebarHeight = value; OnPropertyChanged(); }
        }

        public AppWindowTitlebarManager()
        {
            return;
            try
            {
                MainWindow = AppWindowExtensions.GetMainWindow();
                AppWindow = AppWindowExtensions.GetAppWindowFromWPFWindow(MainWindow);

                if (AppWindow != null)
                {
                    if (AppWindowTitleBar.IsCustomizationSupported())
                    {
                        CustomizeTitleBar();
                        MainWindow.Loaded += MainWindow_Loaded;
                        MainWindow.SizeChanged += MainWindow_SizeChanged;

                        // Ensure first call to functions
                        MainWindow_Loaded(null, null);
                        MainWindow_SizeChanged(null, null);
                    }
                    else
                    {
                        MessageBox.Show("Titlebar customization not supported on this device!", "Error");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex);

                //MessageBox.Show("Could not load custom window!", "Error");
                MainWindow.Title = "Custom Window Not Supported";
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetDragRegionForCustomTitleBar();
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetDragRegionForCustomTitleBar();
        }

        private void CustomizeTitleBar()
        {
            // AppWindowTitleBar
            AppWindowTitleBar titleBar = AppWindow.TitleBar;
            titleBar.ExtendsContentIntoTitleBar = true;

            // Size
            //titleBar.PreferredHeightOption = TitleBarHeightOption.Standard; // 32
            titleBar.PreferredHeightOption = TitleBarHeightOption.Tall; // 46
            TitlebarHeight = titleBar.Height;

            // Icon
            //titleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;)
            AppWindow.SetIcon("/Images/LockScreenLogo.scale-200.png");

            // Title


            // Bar
            titleBar.BackgroundColor = Windows.UI.Color.FromArgb(255, 32, 32, 32);
            titleBar.ForegroundColor = Windows.UI.Color.FromArgb(255, 255, 255, 255);
            /// Inactive
            titleBar.InactiveBackgroundColor = Windows.UI.Color.FromArgb(255, 32, 32, 32);
            titleBar.InactiveForegroundColor = Windows.UI.Color.FromArgb(255, 255, 255, 255);

            // Buttons
            titleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(0, 32, 32, 32);          // #0
            titleBar.ButtonForegroundColor = Windows.UI.Color.FromArgb(0, 255, 255, 255);       // #0
            titleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(255, 18, 18, 18);   // #121212
            titleBar.ButtonPressedBackgroundColor = Windows.UI.Color.FromArgb(255, 22, 22, 22); // #161616
            /// Inactive
            titleBar.ButtonInactiveBackgroundColor = Windows.UI.Color.FromArgb(0, 32, 32, 32);
            titleBar.ButtonInactiveForegroundColor = Windows.UI.Color.FromArgb(0, 255, 255, 255);
        }

        private static double GetScaleAdjustment()
        {
            DisplayArea displayArea =
                DisplayArea.GetFromWindowId(
                    AppWindowExtensions.GetAppWindowFromWPFWindow(AppWindowExtensions.GetMainWindow()).Id, DisplayAreaFallback.Primary);

            IntPtr hMonitor = Win32Interop.GetMonitorFromDisplayId(displayArea.DisplayId);

            // Get DPI
            int result = GetDpiForMonitor(hMonitor, Monitor_DPI_Type.MDT_Default, out uint dpiX, out uint _);
            if (result != 0)
            {
                // Error handling here
                throw new Exception("Could not get DPI for monitor.");
            }

            return (uint)(((long)dpiX * 100 + (96 >> 1)) / 96) / 100.0;
        }

        #region LeftPaddingColumn
        private GridLength _leftPaddingColumnWidth;

        public GridLength LeftPaddingColumnWidth
        {
            get { return _leftPaddingColumnWidth; }
            set { _leftPaddingColumnWidth = value; OnPropertyChanged(); }
        }

        private double _leftPaddingColumnActualWidth;

        public double LeftPaddingColumnActualWidth
        {
            get { return _leftPaddingColumnActualWidth; }
            set { _leftPaddingColumnActualWidth = value; OnPropertyChanged(); }
        }
        #endregion LeftPaddingColumn

        #region RightPaddingColumn
        private GridLength _rightPaddingColumnWidth;

        public GridLength RightPaddingColumnWidth
        {
            get { return _rightPaddingColumnWidth; }
            set { _rightPaddingColumnWidth = value; OnPropertyChanged(); }
        }

        private double _rightPaddingColumnActualWidth;

        public double RightPaddingColumnActualWidth
        {
            get { return _rightPaddingColumnActualWidth; }
            set { _rightPaddingColumnActualWidth = value; OnPropertyChanged(); }
        }
        #endregion RightPaddingColumn

        #region Extra Columns
        private double _iconTitleColumnActualWidth;

        public double IconTitleColumnActualWidth
        {
            get { return _iconTitleColumnActualWidth; }
            set { _iconTitleColumnActualWidth = value; OnPropertyChanged(); }
        }

        private double _testColumnActualWidth;

        public double TestColumnActualWidth
        {
            get { return _testColumnActualWidth; }
            set { _testColumnActualWidth = value; OnPropertyChanged(); }
        }
        #endregion Extra Columns

        private void SetDragRegionForCustomTitleBar()
        {
            if (AppWindow.TitleBar.ExtendsContentIntoTitleBar)
            {
                double scaleAdjustment = GetScaleAdjustment();

                LeftPaddingColumnWidth = new GridLength(AppWindow.TitleBar.LeftInset / scaleAdjustment);
                RightPaddingColumnWidth = new GridLength(AppWindow.TitleBar.RightInset / scaleAdjustment);

                List<Windows.Graphics.RectInt32> dragRectsList = new();

                Windows.Graphics.RectInt32 dragRectL;
                dragRectL.X = (int)((LeftPaddingColumnActualWidth) * scaleAdjustment);
                dragRectL.Y = 0;
                dragRectL.Height = (int)(AppWindow.TitleBar.Height * scaleAdjustment);
                dragRectL.Width = (int)((IconTitleColumnActualWidth + TestColumnActualWidth) * scaleAdjustment);
                dragRectsList.Add(dragRectL);

                Windows.Graphics.RectInt32 dragRectR;
                dragRectR.X = (int)((LeftPaddingColumnActualWidth + IconTitleColumnActualWidth) * scaleAdjustment);
                dragRectR.Y = 0;
                dragRectR.Height = (int)(AppWindow.TitleBar.Height * scaleAdjustment);
                dragRectR.Width = 0/*(int)(RightDragColumn.ActualWidth * scaleAdjustment)*/;
                dragRectsList.Add(dragRectR);

                Windows.Graphics.RectInt32[] dragRects = dragRectsList.ToArray();
                AppWindow.TitleBar.SetDragRectangles(dragRects);
            }
        }
    }
}
