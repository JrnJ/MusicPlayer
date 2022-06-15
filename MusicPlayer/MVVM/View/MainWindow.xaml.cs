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

namespace MusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // Classes
        private AudioPlayer _audioPlayer;

        public AudioPlayer AudioPlayer
        {
            get { return _audioPlayer; }
            set { _audioPlayer = value; }
        }

        private Playlist _myMusic;

        public Playlist MyMusic
        {
            get { return _myMusic; }
            set { _myMusic = value; }
        }

        public RadioButton CurrentUISong = new RadioButton();
        public int CurrentSongIndex = 0;

        public double SongTotalMs;

        public bool LeftMouseDownOnSlider = false;

        public MainWindow()
        {
            InitializeComponent();

            ConfigureAudioPlayer();
            ConfigureSettings();
        }

        #region Configuration
        private void ConfigureAudioPlayer()
        {
            // AudioPlayer
            AudioPlayer = new AudioPlayer();
            AudioPlayer.MediaPlayer.MediaEnded += MediaPlayerMediaEnded;

            // Timer Tick Event
            AudioPlayer.Timer.Tick += Timer_Tick;
        }

        private void ConfigureSettings()
        {
            // Load Settings
            AppSettings.GetSettingsFromFile();
            AudioPlayer.Volume = AppSettings.Volume;
            MyMusic = new Playlist(0, new ObservableCollection<Song>(), "My Music", "All music from all folders.");

            // Create a playlist of all songs
            for (int i = 0; i < AppSettings.MusicFolders.Count; i++)
            {
                List<Song> songs = FileHandler.GetSongsFromFolder(AppSettings.MusicFolders[i]);

                if (songs != null)
                {
                    //MyMusic.Songs.AddRange(songs);
                    for (int i2 = 0; i2 < songs.Count; i2++)
                    {
                        MyMusic.Songs.Add(songs[i2]);
                    }
                }
            }
        }
        #endregion Configuration

        #region MediaPlayerEvents

        //private void MediaPlayerButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        //{
        //    // Call to MainWindow thread
        //    Dispatcher.Invoke(() => { ButtonPressed(sender, args); });
        //}

        private void MediaPlayerMediaEnded(Windows.Media.Playback.MediaPlayer sender, object args)
        {
            // Call to MainWindow thread
            Dispatcher.Invoke(() => { MediaEnded(); });
        }
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

        public void MediaEnded()
        {
            // this might get difficult later on
            // maybe put audioplayer in global or something
            AudioPlayer.Pause();

            //if (SelectedPlaylist.Songs.Count - 1 > CurrentSongIndex)
            //{
            //    CurrentSongIndex += 1;
            //    OpenMedia(SelectedPlaylist.Songs[CurrentSongIndex]);
            //    AudioPlayer.Play();
            //}
        }

        private void UpdateTime()
        {
            // Set Slider Accordingly
            sliderSongTimePlayed.Value = AudioPlayer.MediaPlayer.Position.TotalMilliseconds;

            // Update UI
            tblCurrentTime.Text = HelperMethods.MsToTime(AudioPlayer.MediaPlayer.Position.TotalMilliseconds);
        }
        #endregion MusicThings

        #region WindowEvents
        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                #region Reverse
                // Reverse 10s
                case Key.J:
                    AudioPlayer.SubtractTime(10);
                    break;
                // Reverse 5
                case Key.Left:
                    AudioPlayer.SubtractTime(5);
                    break;
                #endregion Reverse
                #region Forward
                // Forward 10s
                case Key.L:
                    AudioPlayer.AddTime(10);
                    break;
                // Forward 5
                case Key.Right:
                    AudioPlayer.AddTime(5);
                    break;
                #endregion Forward
                #region Playpause
                case Key.Space:
                    if (AudioPlayer.IsPlaying)
                        AudioPlayer.Pause();
                    else
                        AudioPlayer.Play();
                    break;
                case Key.K:
                    if (AudioPlayer.IsPlaying)
                        AudioPlayer.Pause();
                    else
                        AudioPlayer.Play();
                    break;
                #endregion Playpause

                // Do this
                // https://blog.magnusmontin.net/2015/03/31/implementing-global-hot-keys-in-wpf/
                #region ChangeVolume
                // https://blog.magnusmontin.net/2015/03/31/implementing-global-hot-keys-in-wpf/
                case Key.NumPad8:
                    AppSettings.Volume += 0.05;
                    break;
                case Key.NumPad2:
                    AppSettings.Volume -= 0.05;
                    break;
                #endregion ChangeVolume
                default:
                    break;
            }
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            // Minimize window
            //WindowState = WindowState.Minimized;

            //e.Cancel = true;
        }
        #endregion WindowEvents

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateTime();
        }

        private void OpenMedia(Song song)
        {
            AudioPlayer.OpenMedia(song);

            // Reset background
            // Changed it to a radiobutton which eliminates this, until further testing of radiobutton at least
            //if (CurrentUISong != null)
            //    CurrentUISong.Style = Application.Current.FindResource("AlbumSong") as Style;

            //UIElement uiElement = (UIElement)icSongs.ItemContainerGenerator.ContainerFromIndex(CurrentSongIndex);
            //CurrentUISong = VisualTreeHelper.GetChild(uiElement, 0) as Button;

            //CurrentUISong.Style = Application.Current.FindResource("SelectedAlbumSong") as Style;

            // Change SelectedRadioButton
            // Doesnt work, tbh cant be botherd atm so will change this later
            // this code is very questionable anyways, lets rework this some day, thanos snapping it atm
            //UIElement uiElement = (UIElement)icSongs.ItemContainerGenerator.ContainerFromIndex(CurrentSongIndex);
            //CurrentUISong = VisualTreeHelper.GetChild(uiElement, 0) as RadioButton;
            //CurrentUISong.IsChecked.Equals(true);

            // Maybe change where this is placed
            tblInfoSongTitle.Text = song.Title;
            tblInfoSongArtist.Text = song.ContributingArtists;
            sliderSongTimePlayed.Value = 0;
            tblCurrentTime.Text = "0:00";

            // Update UI
            SongTotalMs = song.Length;
            tblFinalTime.Text = HelperMethods.MsToTime(SongTotalMs);
            sliderSongTimePlayed.Maximum = SongTotalMs;

            // Remove whenever this mess is fixed
            AudioPlayer.Play();
        }

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
            if (LeftMouseDownOnSlider)
            {
                //sliderSongTimePlayed.Value = 0;
                // MIN: 0

                // Add x amount of time to song
                int msToAdd = int.Parse(Math.Round(sliderSongTimePlayed.Value - AudioPlayer.MediaPlayer.Position.TotalMilliseconds, 0).ToString());
                AudioPlayer.MediaPlayer.Position = AudioPlayer.MediaPlayer.Position.Add(new TimeSpan(0, 0, 0, 0, msToAdd));

                // Start Timer
                UpdateTime();
                AudioPlayer.Timer.Start();
                LeftMouseDownOnSlider = false;
            }
        }

        #region Controls
        private void PausePlayClick(object sender, RoutedEventArgs e)
        {
            if (AudioPlayer.IsPlaying)
                AudioPlayer.Pause();
            else
                AudioPlayer.Play();
        }

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
