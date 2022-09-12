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
using Microsoft.Windows.ApplicationModel.DynamicDependency;

// Resources
//https://docs.microsoft.com/en-us/windows/apps/develop/title-bar?tabs=wasdk
//https://docs.microsoft.com/en-us/windows/apps/desktop/modernize/desktop-to-uwp-enhance
//https://docs.microsoft.com/en-us/windows/apps/windows-app-sdk/downloads

namespace MusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static AppWindow AppWindow { get; private set; }

        //[DllImport("winmm.dll")]
        //public static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                AppWindow = AppWindowExtensions.GetAppWindowFromWPFWindow(this);

                if (AppWindow != null)
                {
                    if (AppWindowTitleBar.IsCustomizationSupported())
                    {
                        CustomizeTitleBar();
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

        public void CustomizeTitleBar()
        {
            // AppWindowTitleBar
            AppWindowTitleBar titleBar = AppWindow.TitleBar;
            titleBar.ExtendsContentIntoTitleBar = true;

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
            titleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(0, 32, 32, 32);
            titleBar.ButtonForegroundColor = Windows.UI.Color.FromArgb(0, 255, 255, 255);
            titleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(255, 37, 37, 37);
            titleBar.ButtonPressedBackgroundColor = Windows.UI.Color.FromArgb(255, 41, 41, 41);
            /// Inactive
            titleBar.ButtonInactiveBackgroundColor = Windows.UI.Color.FromArgb(0, 32, 32, 32);
            titleBar.ButtonInactiveForegroundColor = Windows.UI.Color.FromArgb(0, 255, 255, 255);
        }
    }
}
