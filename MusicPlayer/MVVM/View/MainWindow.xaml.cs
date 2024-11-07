using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Media;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using System.ComponentModel;

using System.Collections.ObjectModel;
using System.Globalization;
using Windows.Media;
using MusicPlayer.Classes;
using System.Runtime.CompilerServices;

using Microsoft.UI;
using Microsoft.UI.Windowing;
using System.Runtime.InteropServices;
using MusicPlayer.Core;
using MusicPlayer.MVVM.ViewModel;
using static System.Net.Mime.MediaTypeNames;

// Resources
// https://docs.microsoft.com/en-us/windows/apps/develop/title-bar?tabs=wasdk
// https://docs.microsoft.com/en-us/windows/apps/desktop/modernize/desktop-to-uwp-enhance
// https://docs.microsoft.com/en-us/windows/apps/windows-app-sdk/downloads

namespace MusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static AppWindow AppWindow { get; private set; }

        [DllImport("Shcore.dll", SetLastError = true)]
        internal static extern int GetDpiForMonitor(IntPtr hmonitor, Monitor_DPI_Type dpiType, out uint dpiX, out uint dpiY);

        internal enum Monitor_DPI_Type : int
        {
            MDT_Effective_DPI = 0,
            MDT_Angular_DPI = 1,
            MDT_Raw_DPI = 2,
            MDT_Default = MDT_Effective_DPI
        }

        //[DllImport("winmm.dll")]
        //public static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);

        public MainWindow()
        {
            InitializeComponent();

            if (false) return;

            try
            {
                AppWindow = AppWindowExtensions.GetAppWindowFromWPFWindow(AppWindowExtensions.GetMainWindow());

                if (AppWindow != null)
                {
                    if (AppWindowTitleBar.IsCustomizationSupported())
                    {
                        CustomizeTitleBar();
                        AppWindowExtensions.GetMainWindow().Loaded += MainWindow_Loaded;
                        AppWindowExtensions.GetMainWindow().SizeChanged += MainWindow_SizeChanged;

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
                this.Title = "Custom Window Not Supported";
            }
        }

        #region Titlebar
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetDragRegionForCustomTitleBar();
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetDragRegionForCustomTitleBar();
        }

        private static void CustomizeTitleBar()
        {
            // AppWindowTitleBar
            AppWindowTitleBar titleBar = AppWindow.TitleBar;
            titleBar.ExtendsContentIntoTitleBar = true;

            // Size
            titleBar.PreferredHeightOption = TitleBarHeightOption.Standard; // 32
            titleBar.PreferredHeightOption = TitleBarHeightOption.Tall; // 46
            GlobalViewModel.Instance.AppWindowTitlebarManager.TitlebarHeight = titleBar.Height;

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

        private void SetDragRegionForCustomTitleBar()
        {
            if (AppWindow.TitleBar.ExtendsContentIntoTitleBar)
            {
                double scaleAdjustment = GetScaleAdjustment();

                LeftPaddingColumn.Width = new GridLength(AppWindow.TitleBar.LeftInset / scaleAdjustment);
                RightPaddingColumn.Width = new GridLength(AppWindow.TitleBar.RightInset / scaleAdjustment);

                List<Windows.Graphics.RectInt32> dragRectsList = new();

                Windows.Graphics.RectInt32 dragRectL;
                dragRectL.X = (int)((LeftPaddingColumn.ActualWidth) * scaleAdjustment);
                dragRectL.Y = 0;
                dragRectL.Height = (int)(AppWindow.TitleBar.Height * scaleAdjustment);
                dragRectL.Width = (int)((IconTitleColumn.ActualWidth + Test1.ActualWidth) * scaleAdjustment);
                dragRectsList.Add(dragRectL);

                Windows.Graphics.RectInt32 dragRectR;
                dragRectR.X = (int)((LeftPaddingColumn.ActualWidth + IconTitleColumn.ActualWidth) * scaleAdjustment);
                dragRectR.Y = 0;
                dragRectR.Height = (int)(AppWindow.TitleBar.Height * scaleAdjustment);
                dragRectR.Width = 0/*(int)(RightDragColumn.ActualWidth * scaleAdjustment)*/;
                dragRectsList.Add(dragRectR);

                Windows.Graphics.RectInt32[] dragRects = dragRectsList.ToArray();
                AppWindow.TitleBar.SetDragRectangles(dragRects);
            }
        }
        #endregion Titlebar

        bool isMaximized = false;

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key) {

                // Fullscreen Toggle
                case Key.F11:
                    isMaximized = !isMaximized;

                    if (isMaximized)
                    { // Fullscreen
                        // Collapse Content
                        this.AppTitleBar.Visibility = Visibility.Collapsed;

                        // 
                        this.WindowStyle = WindowStyle.None;
                        this.WindowState = WindowState.Normal;
                        this.WindowState = WindowState.Maximized;
                    }
                    else
                    { // Normal
                        // Collapse Content
                        this.AppTitleBar.Visibility = Visibility.Visible;

                        // 
                        this.WindowStyle = WindowStyle.SingleBorderWindow;
                        this.WindowState = WindowState.Maximized;
                    }

                    // this.UpdateLayout();

                    break;

                // Add | Remove
                case Key.J:
                    GlobalViewModel.Instance.AudioPlayer.SubtractTime(10000);
                    break;

                case Key.L:
                    GlobalViewModel.Instance.AudioPlayer.AddTime(10000);
                    break;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {

        }

        int mouseMoves = 0;
        private void hello_MouseEnter(object sender, MouseEventArgs e)
        {
            test1.Text = "Mouse Entered";
        }

        private void hello_MouseLeave(object sender, MouseEventArgs e)
        {
            test1.Text = "Mouse Left";
        }

        private void hello_MouseMove(object sender, MouseEventArgs e)
        {
            mouseMoves++;
            test2.Text = "Moves: " + mouseMoves;

            double mouseX = e.GetPosition(hello).X;
            double percentage = mouseX / hello.ActualWidth; // Calculate the percentage of mouse position
            double newValue = hello.Minimum + (hello.Maximum - hello.Minimum) * percentage; // Calculate the value based on percentage
            // hello.Value = newValue; // Update the Slider's value

            test3.Text = percentage.ToString();
            test4.Text = hello.Maximum.ToString();
            trackbarHoverTime.Text = HelperMethods.MsToTime(newValue);
        }

        private void VolumeSliderLeftMouseUp(object sender, MouseButtonEventArgs e)
        {
            GlobalViewModel.Instance.SaveVolumeToDatabase();
        }
    }
}
