//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using Microsoft.UI;
//using Microsoft.UI.Windowing;
//using System.Runtime.InteropServices;
//using MusicPlayer.Core;
//using System.Windows;

//namespace MusicPlayer.Core
//{
//    public class TitlebarCustomizer
//    {
//        [DllImport("Shcore.dll", SetLastError = true)]
//        internal static extern int GetDpiForMonitor(IntPtr hmonitor, Monitor_DPI_Type dpiType, out uint dpiX, out uint dpiY);

//        internal enum Monitor_DPI_Type : int
//        {
//            MDT_Effective_DPI = 0,
//            MDT_Angular_DPI = 1,
//            MDT_Raw_DPI = 2,
//            MDT_Default = MDT_Effective_DPI
//        }

//        public AppWindow AppWindow { get; private set; }

//        public TitlebarCustomizer(MainWindow mainWindow)
//        {
//            try
//            {
//                AppWindow = AppWindowExtensions.GetAppWindowFromWPFWindow(mainWindow);

//                if (AppWindow != null)
//                {
//                    if (AppWindowTitleBar.IsCustomizationSupported())
//                    {
//                        CustomizeTitleBar();
//                        mainWindow.Loaded += MainWindow_Loaded;
//                        mainWindow.SizeChanged += MainWindow_SizeChanged;
//                    }
//                    else
//                    {
//                        MessageBox.Show("Titlebar customization not supported on this device!", "Error");
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("ERROR: " + ex);

//                //MessageBox.Show("Could not load custom window!", "Error");
//                mainWindow.Title = "Custom Window Not Supported";
//            }
//        }

//        private void CustomizeTitleBar()
//        {
//            // AppWindowTitleBar
//            AppWindowTitleBar titleBar = AppWindow.TitleBar;
//            titleBar.ExtendsContentIntoTitleBar = true;

//            // Icon
//            //titleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;)
//            AppWindow.SetIcon("/Images/LockScreenLogo.scale-200.png");

//            // Title


//            // Bar
//            titleBar.BackgroundColor = Windows.UI.Color.FromArgb(255, 32, 32, 32);
//            titleBar.ForegroundColor = Windows.UI.Color.FromArgb(255, 255, 255, 255);
//            /// Inactive
//            titleBar.InactiveBackgroundColor = Windows.UI.Color.FromArgb(255, 32, 32, 32);
//            titleBar.InactiveForegroundColor = Windows.UI.Color.FromArgb(255, 255, 255, 255);

//            // Buttons
//            titleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(0, 32, 32, 32);
//            titleBar.ButtonForegroundColor = Windows.UI.Color.FromArgb(0, 255, 255, 255);
//            titleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(255, 37, 37, 37);   // #252525
//            titleBar.ButtonPressedBackgroundColor = Windows.UI.Color.FromArgb(255, 41, 41, 41); // #292929
//            /// Inactive
//            titleBar.ButtonInactiveBackgroundColor = Windows.UI.Color.FromArgb(0, 32, 32, 32);
//            titleBar.ButtonInactiveForegroundColor = Windows.UI.Color.FromArgb(0, 255, 255, 255);
//        }

//        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
//        {
//            SetDragRegionForCustomTitleBar();
//        }

//        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
//        {
//            SetDragRegionForCustomTitleBar();
//        }

//        private double GetScaleAdjustment()
//        {
//            DisplayArea displayArea = DisplayArea.GetFromWindowId(AppWindowExtensions.GetAppWindowFromWPFWindow(AppWindowExtensions.GetMainWindow()).Id, DisplayAreaFallback.Primary);
//            IntPtr hMonitor = Win32Interop.GetMonitorFromDisplayId(displayArea.DisplayId);

//            // Get DPI
//            int result = GetDpiForMonitor(hMonitor, Monitor_DPI_Type.MDT_Default, out uint dpiX, out uint _);
//            if (result != 0)
//            {
//                // Error handling here
//                throw new Exception("Could not get DPI for monitor.");
//            }

//            return (uint)(((long)dpiX * 100 + (96 >> 1)) / 96) / 100.0;
//        }

//        private void SetDragRegionForCustomTitleBar()
//        {
//            if (AppWindow.TitleBar.ExtendsContentIntoTitleBar)
//            {
//                double scaleAdjustment = GetScaleAdjustment();

//                LeftPaddingColumn.Width = new GridLength(AppWindow.TitleBar.LeftInset / scaleAdjustment);
//                RightPaddingColumn.Width = new GridLength(AppWindow.TitleBar.RightInset / scaleAdjustment);

//                List<Windows.Graphics.RectInt32> dragRectsList = new();

//                Windows.Graphics.RectInt32 dragRectL;
//                dragRectL.X = (int)((LeftPaddingColumn.ActualWidth) * scaleAdjustment);
//                dragRectL.Y = 0;
//                dragRectL.Height = (int)(mainWindow.AppTitleBar.ActualHeight * scaleAdjustment);
//                dragRectL.Width = (int)((IconTitleColumn.ActualWidth/* + SomethingElse*/) * scaleAdjustment);
//                dragRectsList.Add(dragRectL);

//                Windows.Graphics.RectInt32 dragRectR;
//                dragRectR.X = (int)((LeftPaddingColumn.ActualWidth + IconTitleColumn.ActualWidth) * scaleAdjustment);
//                dragRectR.Y = 0;
//                dragRectR.Height = (int)(mainWindow.AppTitleBar.ActualHeight * scaleAdjustment);
//                dragRectR.Width = 0/*(int)(RightDragColumn.ActualWidth * scaleAdjustment)*/;
//                dragRectsList.Add(dragRectR);

//                Windows.Graphics.RectInt32[] dragRects = dragRectsList.ToArray();
//                AppWindow.TitleBar.SetDragRectangles(dragRects);
//            }
//        }
//    }
//}
