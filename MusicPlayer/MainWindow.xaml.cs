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
using System.Runtime.InteropServices;
using MusicPlayer.Classes;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.Media;
using System.Collections.ObjectModel;
using System.Globalization;

namespace MusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged, IMultiValueConverter
    {
        // Classes
        public DispatcherTimer Timer = new DispatcherTimer();
        public SystemMediaTransportControls MediaControls;
        public Debugger Debugger;

        private Windows.Media.Playback.MediaPlayer _mediaPlayer;

        public Windows.Media.Playback.MediaPlayer MediaPlayer
        {
            get { return _mediaPlayer; }
            set { _mediaPlayer = value; }
        }

        private List<Playlist> _playlists;

        public List<Playlist> Playlists
        {
            get { return _playlists; }
            set { _playlists = value; }
        }

        private Playlist _selectedPlaylist;

        public Playlist SelectedPlaylist
        {
            get { return _selectedPlaylist; }
            set { _selectedPlaylist = value; OnPropertyChanged(); }
        }

        private Playlist _myMusic;

        public Playlist MyMusic
        {
            get { return _myMusic; }
            set { _myMusic = value; }
        }

        private Settings _settings;

        public Settings Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }

        public Button CurrentUISong = new Button();
        public Song CurrentSong;

        public int CurrentSongIndex = 0;

        public double SongTotalMs;
        public bool IsPlaying = false;

        public bool LeftMouseDownOnSlider = false;

        // DLL
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        public MainWindow()
        {
            InitializeComponent();
            //AllocConsole();
            Debugger = new Debugger(this);
            Debugger.SetActive(false);

            ConfigureMediaPlayer();
            ConfigurePlaylists();
            ConfigureSettings();

            DataContext = this;
        }

        #region Configuration
        private void ConfigureSettings()
        {
            // Load Settings
            Settings = FileHandler.GetSettings();
            MediaPlayer.Volume = Settings.Volume;
            MyMusic = new Playlist(0, new List<Song>(), "My Music", "All music from all folders.");

            // Create a playlist of all songs
            for (int i = 0; i < Settings.MusicFolders.Count; i++)
            {
                List<Song> songs = FileHandler.GetSongsFromFolder(Settings.MusicFolders[i]);

                if (songs != null)
                {
                    MyMusic.Songs.AddRange(songs);
                }
                else
                {

                }
            }
        }

        private void ConfigureMediaPlayer()
        {
            MediaPlayer = new Windows.Media.Playback.MediaPlayer();

            // Mediaplayer events
            MediaPlayer.MediaOpened += MediaPlayerMediaOpened;
            MediaPlayer.MediaEnded += MediaPlayerMediaEnded;
            MediaPlayer.VolumeChanged += MediaPlayerVolumeChanged;
            MediaPlayer.CommandManager.IsEnabled = true;

            // try get next song to work
            MediaPlayer.AutoPlay = true;

            // Instantiate Timer
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Tick += Timer_Tick;
        }

        public void ConfigurePlaylists()
        {
            // Load Playlist
            Playlists = FileHandler.GetPlaylists();

            if (Playlists == null)
            {
                MessageBox.Show("No playlists found!");
            }
            else
            {
                LoadPlaylist(Playlists[0]);

                for (int i = 0; i < Playlists.Count; i++)
                {
                    spPlaylists.Children.Add(CreatePlaylistTabUI(Playlists[i]));
                }
            }
        }
        #endregion Configuration

        #region MediaPlayerEvents
        private void MediaPlayerMediaOpened(Windows.Media.Playback.MediaPlayer sender, object args)
        {
            if (CurrentSong != null)
            {
                UpdateSMTCDisplay(CurrentSong);
            }
        }

        private void MediaPlayerMediaEnded(Windows.Media.Playback.MediaPlayer sender, object args)
        {
            // Call to MainWindow thread
            Dispatcher.Invoke(() => { MediaEnded(); });
        }

        private void MediaPlayerVolumeChanged(Windows.Media.Playback.MediaPlayer sender, object args)
        {
            // Apply volume change to settings
            Settings.Volume = MediaPlayer.Volume;
        }
        #endregion MediaPlayerEvents

        #region MusicThings
        public void MediaEnded()
        {
            PauseMusic();

            if (SelectedPlaylist.Songs.Count - 1 > CurrentSongIndex)
            {
                CurrentSongIndex += 1;
                OpenMedia(SelectedPlaylist.Songs[CurrentSongIndex]);
            }
        }

        private void UpdateSMTCDisplay(Song song)
        {
            SystemMediaTransportControlsDisplayUpdater updater = MediaPlayer.SystemMediaTransportControls.DisplayUpdater;
            updater.ClearAll();

            updater.Type = MediaPlaybackType.Video;
            updater.VideoProperties.Title = song.Title;
            updater.VideoProperties.Subtitle = song.ContributingArtists;
            updater.Thumbnail = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromUri(new Uri("C:/Users/jeroe/Downloads/SongImagePlaceholder.png"));

            // Update the system media transport controls
            updater.Update();
        }

        private void UpdateTime()
        {
            // Set Slider Accordingly
            sliderSongTimePlayed.Value = MediaPlayer.Position.TotalMilliseconds;

            // Update UI
            tblCurrentTime.Text = HelperMethods.MsToTime(MediaPlayer.Position.TotalMilliseconds);
        }
        #endregion MusicThings

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateTime();
        }

        private void OpenMedia(Song song)
        {
            // Change this fast lmfao
            CurrentSong = song;

            MediaPlayer.Pause();

            //MediaPlayer.Open();
            MediaPlayer.SetUriSource(new Uri(song.Path));

            // Reset background
            CurrentUISong.Style = Application.Current.FindResource("AlbumSong") as Style;

            // Change new background
            CurrentUISong = FindName($"Song{CurrentSongIndex}") as Button;
            CurrentUISong.Style = Application.Current.FindResource("SelectedAlbumSong") as Style;

            // Maybe change where this is placed
            tblInfoSongTitle.Text = song.Title;
            tblInfoSongArtist.Text = song.ContributingArtists;
            sliderSongTimePlayed.Value = 0;
            tblCurrentTime.Text = "0:00";

            // Update UI
            SongTotalMs = song.Length;
            tblFinalTime.Text = HelperMethods.MsToTime(SongTotalMs);
            sliderSongTimePlayed.Maximum = SongTotalMs;

            PlayMusic();
        }

        private void PlayMusic()
        {
            MediaPlayer.Play();
            IsPlaying = true;

            Timer.Start();
        }

        /// <summary>
        /// Pauses the song
        /// </summary>
        private void PauseMusic()
        {
            MediaPlayer.Pause();
            IsPlaying = false;

            Timer.Stop();
        }

        /// <summary>
        /// Adds an amount of time in seconds to the song
        /// </summary>
        /// <param name="amount">Time in seconds</param>
        private void AddTime(int amount)
        {
            MediaPlayer.Position = MediaPlayer.Position.Add(new TimeSpan(0, 0, amount));
            Timer_Tick(Timer, null);
        }

        /// <summary>
        /// Subtracts an amount of time in seconds from the song
        /// </summary>
        /// <param name="amount">Time in seconds</param>
        private void SubtractTime(int amount)
        {
            MediaPlayer.Position = MediaPlayer.Position.Subtract(new TimeSpan(0, 0, amount));
            Timer_Tick(Timer, null);
        }

        // Shortcuts
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                #region Reverse
                // Reverse 10s
                case Key.J:
                    SubtractTime(10);
                    break;
                // Reverse 5
                case Key.Left:
                    SubtractTime(5);
                    break;
                #endregion Reverse
                #region Forward
                // Forward 10s
                case Key.L:
                    AddTime(10);
                    break;
                // Forward 5
                case Key.Right:
                    AddTime(5);
                    break;
                #endregion Forward
                #region Playpause
                case Key.Space:
                    if (IsPlaying)
                        PauseMusic();
                    else
                        PlayMusic();
                    break;
                case Key.K:
                    if (IsPlaying)
                        PauseMusic();
                    else
                        PlayMusic();
                    break;
                #endregion Playpause

                // Do this
                // https://blog.magnusmontin.net/2015/03/31/implementing-global-hot-keys-in-wpf/
                #region ChangeVolume
                // https://blog.magnusmontin.net/2015/03/31/implementing-global-hot-keys-in-wpf/
                case Key.NumPad8:
                    Settings.Volume += 0.05;
                    break;
                case Key.NumPad2:
                    Settings.Volume -= 0.05;
                    break;
                #endregion ChangeVolume
                default:
                    break;
            }
        }

        private void sliderSongTimePlayed_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LeftMouseDownOnSlider = true;
            //Track track = sliderSongTimePlayed.Template.FindName("PART_Track", sliderSongTimePlayed) as Track;
            // This value is for getting the thubs width to play in accout if thats even needed

            double value = sliderSongTimePlayed.Maximum / (sliderSongTimePlayed.ActualWidth + 0) * e.GetPosition(sliderSongTimePlayed).X;
            sliderSongTimePlayed.Value = value;

            // Pause timer
            Timer.Stop();
        }

        private void sliderSongTimePlayed_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (LeftMouseDownOnSlider)
            {
                //Debugger.WriteLine(e.GetPosition(sliderSongTimePlayed).ToString());
                //sliderSongTimePlayed.Value = 0;
                // MIN: 0
                //Debugger.WriteLine(sliderSongTimePlayed.ActualWidth.ToString());
                Debugger.WriteLine(sliderSongTimePlayed.Value.ToString());

                // Add x amount of time to song
                int msToAdd = int.Parse(Math.Round(sliderSongTimePlayed.Value - MediaPlayer.Position.TotalMilliseconds, 0).ToString());
                MediaPlayer.Position = MediaPlayer.Position.Add(new TimeSpan(0, 0, 0, 0, msToAdd));

                // Start Timer
                UpdateTime();
                Timer.Start();
                LeftMouseDownOnSlider = false;
            }
        }

        #region Controls
        private void PausePlayClick(object sender, RoutedEventArgs e)
        {
            if (IsPlaying)
                PauseMusic();
            else
                PlayMusic();
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

        public void LoadPlaylist(Playlist playlist)
        {
            SelectedPlaylist = playlist;
            spPlaylistSongs.Children.Clear();

            // Loop thru all songs
            for (int i = 0; i < playlist.Songs.Count; i++)
            {
                spPlaylistSongs.Children.Add(CreateSongUI(playlist.Songs[i], i));

                // Add song to screen
                if (playlist.Songs[i] == CurrentSong)
                {
                    CurrentSongIndex = i;

                    // Change new background
                    CurrentUISong = FindName($"Song{CurrentSongIndex}") as Button;
                    CurrentUISong.Style = Application.Current.FindResource("SelectedAlbumSong") as Style;
                }
            }
        }

        #region CreateUI
        public Button CreateSongUI(Song song, int index)
        {
            // Create UI Element
            Button btn = new()
            {
                Style = Application.Current.FindResource("AlbumSong") as Style,
                DataContext = song,
                Tag = song.Id
            };
            btn.Click += AlbumSongClick;
            btn.MouseRightButtonDown += AlbumSongRightMouseDown;
            btn.Name = RegisterAndOrSetName($"Song{index}", btn);

            return btn;
        }

        public Border CreatePlaylistTabUI(Playlist playlist)
        {  
            Border border = new()
            {
                Padding = new Thickness(0, 8, 0, 8),
                Tag = playlist.Id
            };
            border.MouseDown += SelectPlaylistClick;
            border.Name = RegisterAndOrSetName($"Playlist{playlist.Id}", border);

            StackPanel sp = new()
            {
                VerticalAlignment = VerticalAlignment.Center
            };

            TextBlock tblPlaylistName = new()
            {
                // Binding
                FontSize = 18
            };
            Binding tblPlaylistNameTextBinding = new Binding("Name");
            tblPlaylistNameTextBinding.Source = Playlists[playlist.Id];
            BindingOperations.SetBinding(tblPlaylistName, TextBlock.TextProperty, tblPlaylistNameTextBinding);

            DockPanel dpPlaylistInfo = new();

            TextBlock tblPlaylistSongCount = new();
            Binding tblSongCountBinding = new Binding("Songs.Count");
            tblSongCountBinding.Source = Playlists[playlist.Id];
            tblSongCountBinding.StringFormat = "{0} Songs, ";
            BindingOperations.SetBinding(tblPlaylistSongCount, TextBlock.TextProperty, tblSongCountBinding);

            TextBlock tblPlaylistDuration = new();
            Binding tblPlaylistDurationBinding = new Binding("PlaylistDuration");
            tblPlaylistDurationBinding.Source = Playlists[playlist.Id];
            BindingOperations.SetBinding(tblPlaylistDuration, TextBlock.TextProperty, tblPlaylistDurationBinding);

            dpPlaylistInfo.Children.Add(tblPlaylistSongCount);
            dpPlaylistInfo.Children.Add(tblPlaylistDuration);

            sp.Children.Add(tblPlaylistName);
            sp.Children.Add(dpPlaylistInfo);

            border.Child = sp;

            return border;
        }
        #endregion CreateUI

        private void AlbumSongClick(object sender, RoutedEventArgs e)
        {
            CurrentSongIndex = int.Parse((sender as Button).Name.Replace("Song", ""));

            OpenMedia(SelectedPlaylist.Songs[CurrentSongIndex]);
            PlayMusic();
        }

        private void AlbumSongRightMouseDown(object sender, MouseButtonEventArgs e)
        {
            Button btn = sender as Button;

            // Check if Tags are the same
            // If so, change visibility to Collapsed
            // If not, move popup to new position

            if (popupSongOptions.Tag != btn.Tag)
            {
                // Move popup to element
                popupSongOptions.Tag = btn.Tag;

                Point mousePos = PointToScreen(Mouse.GetPosition(Application.Current.Windows[0]));
                popupSongOptions.Margin = new Thickness(
                    mousePos.X,
                    mousePos.Y,
                    0, 0);

                // Make element visible
                popupSongOptions.Visibility = Visibility.Visible;
            }
            else
            {
                // If Tags are the same the element is already open and should be closed
                popupSongOptions.Visibility = Visibility.Collapsed;
                popupSongOptions.Tag = null;
            }
        }

        private void SelectPlaylistClick(object sender, MouseButtonEventArgs e)
        {
            // Load that playlist5
            int playlistId = int.Parse((sender as Border).Name.Replace("Playlist", ""));

            Playlist playlist = Playlists.Find(x => x.Id == playlistId);
            LoadPlaylist(playlist);
        }

        private string RegisterAndOrSetName(string name, object obj)
        {
            if (FindName(name) != null)
                UnregisterName(name);

            RegisterName(name, obj);

            return name;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoadPlaylist(MyMusic);
        }

        private void Slider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Save settings
            FileHandler.SaveSettings(Settings);
        }

        #region Interface Implementations
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return String.Format("{0} {1}", values[0], values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion Interface Implementations

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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Show My Music and allow a selection
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            tblSearch.Text = (sender as TextBox).Text == "" ? "Search..." : "";

            // Check textbox
            if (!string.IsNullOrWhiteSpace((sender as TextBox).Text))
            {
                // Clear playlist
                spPlaylistSongs.Children.Clear();

                for (int i = 0; i < MyMusic.Songs.Count; i++)
                {
                    // Check if song contains them
                    if (!string.IsNullOrWhiteSpace(MyMusic.Songs[i].Title))
                        if (MyMusic.Songs[i].Title.ToLower().Contains((sender as TextBox).Text.ToLower()) || 
                            MyMusic.Songs[i].ContributingArtists.ToLower().Contains((sender as TextBox).Text.ToLower()))
                            spPlaylistSongs.Children.Add(CreateSongUI(MyMusic.Songs[i], i));
                }
            }
            else
            {
                // Clear and fill the search playlist
                spPlaylistSongs.Children.Clear();
                for (int i = 0; i < MyMusic.Songs.Count; i++)
                {
                    spPlaylistSongs.Children.Add(CreateSongUI(MyMusic.Songs[i], i));
                }
            }
        }

        private void AddPlaylistClick(object sender, RoutedEventArgs e)
        {
            // Create sample template to add a new playlist
            Playlist playlist = new Playlist(Playlists.Count, new List<Song>(), "New Playlist", "New Playlist");

            Playlists.Add(playlist);
            spPlaylists.Children.Add(CreatePlaylistTabUI(playlist));
            LoadPlaylist(playlist);
        }

        private void DeletePlaylistClick(object sender, RoutedEventArgs e)
        {
            Playlists.Remove(Playlists.Where(x => x.Id == int.Parse((sender as Button).Tag.ToString())).First());

            for (int i = 0; i < spPlaylists.Children.Count; i++)
            {
                string tag1 = (spPlaylists.Children[i] as Border).Tag.ToString();
                string tag2 = (sender as Button).Tag.ToString();
                if ((spPlaylists.Children[i] as Border).Tag.ToString() == (sender as Button).Tag.ToString())
                {
                    spPlaylists.Children.RemoveAt(i);
                }
            }

            LoadPlaylist(MyMusic);
        }
    }
}
