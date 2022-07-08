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

namespace MusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // Classes
        public RadioButton CurrentUISong = new RadioButton();

        public bool LeftMouseDownOnSlider = false;

        public static AppWindow AppWindow;

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

#region MediaPlayerEvents

        //private void MediaPlayerButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        //{
        //    // Call to MainWindow thread
        //    Dispatcher.Invoke(() => { ButtonPressed(sender, args); });
        //}
#endregion MediaPlayerEvents

#region MusicThings
        //public void ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        //{
        //    switch (args.Button)
        //    {
        //        case SystemMediaTransportControlsButton.Previous:
        //            PauseMusic();
        //            if (CurrentSongIndex >= 1)
        //            {
        //                CurrentSongIndex -= 1;
        //                OpenMedia(SelectedPlaylist.Songs[CurrentSongIndex]);
        //                PlayMusic();
        //            }
        //            break;
        //        case SystemMediaTransportControlsButton.Next:
        //            PauseMusic();
        //            if (SelectedPlaylist.Songs.Count - 1 > CurrentSongIndex)
        //            {
        //                CurrentSongIndex += 1;
        //                OpenMedia(SelectedPlaylist.Songs[CurrentSongIndex]);
        //                PlayMusic();
        //            }
        //            break;
        //    }
        //}
#endregion MusicThings

#region WindowEvents
        //private void WindowKeyDown(object sender, KeyEventArgs e)
        //{
        //    switch (e.Key)
        //    {
        //        #region Reverse
        //        // Reverse 10s
        //        case Key.J:
        //            AudioPlayer.SubtractTime(10);
        //            break;
        //        // Reverse 5
        //        case Key.Left:
        //            AudioPlayer.SubtractTime(5);
        //            break;
        //        #endregion Reverse
        //        #region Forward
        //        // Forward 10s
        //        case Key.L:
        //            AudioPlayer.AddTime(10);
        //            break;
        //        // Forward 5
        //        case Key.Right:
        //            AudioPlayer.AddTime(5);
        //            break;
        //        #endregion Forward
        //        #region Playpause
        //        case Key.Space:
        //            if (AudioPlayer.IsPlaying)
        //                AudioPlayer.Pause();
        //            else
        //                AudioPlayer.Play();
        //            break;
        //        case Key.K:
        //            if (AudioPlayer.IsPlaying)
        //                AudioPlayer.Pause();
        //            else
        //                AudioPlayer.Play();
        //            break;
        //        #endregion Playpause

        //        // Do this
        //        // https://blog.magnusmontin.net/2015/03/31/implementing-global-hot-keys-in-wpf/
        //        #region ChangeVolume
        //        // https://blog.magnusmontin.net/2015/03/31/implementing-global-hot-keys-in-wpf/
        //        case Key.NumPad8:
        //            AppSettings.Volume += 0.05;
        //            break;
        //        case Key.NumPad2:
        //            AppSettings.Volume -= 0.05;
        //            break;
        //        #endregion ChangeVolume
        //        default:
        //            break;
        //    }
        //}
#endregion WindowEvents

        private void sliderSongTimePlayed_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LeftMouseDownOnSlider = true;
            //Track track = sliderSongTimePlayed.Template.FindName("PART_Track", sliderSongTimePlayed) as Track;
            // This value is for getting the thubs width to play in accout if thats even needed

            double value = sliderSongTimePlayed.Maximum / (sliderSongTimePlayed.ActualWidth + 0) * e.GetPosition(sliderSongTimePlayed).X;
            sliderSongTimePlayed.Value = value;
        }

        private void sliderSongTimePlayed_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //if (LeftMouseDownOnSlider)
            //{
            //    //sliderSongTimePlayed.Value = 0;
            //    // MIN: 0

            //    // Add x amount of time to song
            //    int msToAdd = int.Parse(Math.Round(sliderSongTimePlayed.Value - AudioPlayer.MediaPlayer.Position.TotalMilliseconds, 0).ToString());
            //    AudioPlayer.MediaPlayer.Position = AudioPlayer.MediaPlayer.Position.Add(new TimeSpan(0, 0, 0, 0, msToAdd));

            //    // Start Timer
            //    UpdateTime();
            //    AudioPlayer.Timer.Start();
            //    LeftMouseDownOnSlider = false;
            //}
        }

#region Controls
        private void ShuffleClick(object sender, RoutedEventArgs e)
        {

        }

        private void RepeatClick(object sender, RoutedEventArgs e)
        {

        }

        private void PreviousSongClick(object sender, RoutedEventArgs e)
        {

        }

        private void NextSongClick(object sender, RoutedEventArgs e)
        {

        }
#endregion Controls

        //private void PlaylistItemClick(object sender, RoutedEventArgs e)
        //{
        //    int playlistId = int.Parse((sender as FrameworkElement).Tag.ToString());

        //    // ehm maybe not :)
        //    Playlist playlist = Playlists.ToList().Find(x => x.Id == playlistId);
        //    LoadPlaylist(playlist);
        //}

        //private void AlbumSongClick(object sender, RoutedEventArgs e)
        //{
        //    int songId = int.Parse((sender as FrameworkElement).Tag.ToString());
        //    Song song = SelectedPlaylist.Songs.Where(x => x.Id == songId).First();
        //    CurrentSongIndex = SelectedPlaylist.Songs.IndexOf(song);

        //    OpenMedia(SelectedPlaylist.Songs[CurrentSongIndex]);
        //}

#region VolumeSlider
        private void VolumeSliderPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Save settings
            AppSettings.SaveSettingsToFile();
        }
#endregion VolumeSlider

        private void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            (sender as TextBox).IsReadOnly = !(sender as TextBox).IsReadOnly;

            if ((sender as TextBox).IsReadOnly)
            {
                (sender as TextBox).Cursor = Cursors.Arrow;
            }
            else
            {
                (sender as TextBox).Cursor = Cursors.IBeam;
            }
        }

        private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBox).IsReadOnly)
            {
                e.Handled.Equals(true);
                if ((sender as TextBox).SelectionLength != 0)
                    (sender as TextBox).SelectionLength = 0;
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                TextBox_MouseDoubleClick(sender, null);
                //Playlists.First(x => x.Id == CurrentPlaylist.Id).Name = (sender as TextBox).Text;

                //FileHandler.SavePlaylistsLocation(Playlists);
            }
        }

#region Interface Implementations
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
#endregion Interface Implementations
    }
}
