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
    public partial class MainWindow : Window, IMultiValueConverter
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

        private Playlist _currentPlaylist;

        public Playlist CurrentPlaylist
        {
            get { return _currentPlaylist; }
            set { _currentPlaylist = value; }
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

        public Border CurrentUISong = new Border();
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

            if (CurrentPlaylist.Songs.Count - 1 > CurrentSongIndex)
            {
                CurrentSongIndex += 1;
                OpenMedia(CurrentPlaylist.Songs[CurrentSongIndex]);
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
            CurrentUISong = FindName($"Song{CurrentSongIndex}") as Border;
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
            int breakpoint = 0;
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
            CurrentPlaylist = playlist;
            spPlaylistSongs.Children.Clear();

            // Loop thru all songs
            for (int i = 0; i < playlist.Songs.Count; i++)
            {
                // Add song to screen
                spPlaylistSongs.Children.Add(CreateSongUI(playlist.Songs[i], i)); 
            }
        }

        private void AddPlaylistClick(object sender, RoutedEventArgs e)
        {
            
        }

        public Border CreateSongUI(Song song, int index = 0)
        {
            // Create UI Elements
            Border borderContainer = new Border()
            {
                Style = Application.Current.FindResource("AlbumSong") as Style
            };
            borderContainer.MouseDown += AlbumSongMouseDown;
            borderContainer.Name = RegisterAndOrSetName($"Song{index}", borderContainer);

            DockPanel dpContentDocker = new DockPanel()
            {
                Height = 50
            };

            TextBlock tblSongId = new TextBlock()
            {
                Text = song.Id.ToString(),
                TextAlignment = TextAlignment.Center,
                Width = 26
            };

            StackPanel spSongInfoDevider = new StackPanel()
            {
                VerticalAlignment = VerticalAlignment.Center
            };

            TextBlock tblSongTitle = new TextBlock()
            {
                Text = song.Title
            };

            TextBlock tblSongAuthor = new TextBlock()
            {
                Text = song.ContributingArtists,
                Foreground = new SolidColorBrush(Color.FromRgb(153, 153, 153)),
                FontSize = 14
            };

            DockPanel dpRightSide = new DockPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 16, 0)
            };

            TextBlock tblSongLength = new TextBlock()
            {
                Text = HelperMethods.MsToTime(song.Length),
                // ONLY IF ALL NUMBERS AFTER : ARE 0 REMOVE THEM 4:10 != 4:1
                Foreground = new SolidColorBrush(Color.FromRgb(153, 153, 153))
            };

            Button btnAction = new Button()
            {
                Content = "- - -",
                Width = 30,
                Height = 20,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(20, 0, 0, 0)
            };

            // Put UI elements together
            spSongInfoDevider.Children.Add(tblSongTitle);
            spSongInfoDevider.Children.Add(tblSongAuthor);

            dpContentDocker.Children.Add(tblSongId);
            dpContentDocker.Children.Add(spSongInfoDevider);

            dpRightSide.Children.Add(tblSongLength);
            dpRightSide.Children.Add(btnAction);

            dpContentDocker.Children.Add(dpRightSide);

            borderContainer.Child = dpContentDocker;

            // Return new UI element
            return borderContainer;
        }

        public Border CreatePlaylistTabUI(Playlist playlist)
        {  
            //                                     < TextBlock DataContext = "{Binding Playlists[0]}" FontSize = "12" >
        
            //                                            < TextBlock.Text >
        
            //                                                < MultiBinding StringFormat = "{}{0} Songs, {1}" >
         
            //                                                     < Binding Path = "Songs.Count" />
          
            //                                                      < Binding Path = "PlaylistDuration" />
           
            //                                                   </ MultiBinding >
           
            //                                               </ TextBlock.Text >
           
            //                                           </ TextBlock >

            Border border = new()
            {
                Padding = new Thickness(0, 8, 0, 8)
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

            DockPanel dp = new()
            {

            };

            TextBlock tblPlaylistInfo = new()
            {
                
            };
            MultiBinding tblPlaylistInfoMultiBinding = new MultiBinding();
            object[] idk = { Playlists[playlist.Id].Songs.Count, Playlists[playlist.Id].PlaylistDuration };
            //tblPlaylistInfoMultiBinding.Converter = Convert(idk, null, tblPlaylistInfo, null);

            tblPlaylistInfoMultiBinding.Bindings.Add(new Binding("Songs.Count") { Source =  Playlists[playlist.Id] });
            tblPlaylistInfoMultiBinding.Bindings.Add(new Binding("PlaylistDuration") { Source =  Playlists[playlist.Id] });
            //BindingOperations.SetBinding(tblPlaylistInfo, TextBlock.TextProperty, tblPlaylistInfoMultiBinding);

            // Add Children
            dp.Children.Add(tblPlaylistName);
            dp.Children.Add(tblPlaylistInfo);

            sp.Children.Add(dp);

            border.Child = sp;

            return border;
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

        private void AlbumSongMouseDown(object sender, MouseButtonEventArgs e)
        {
            CurrentSongIndex = int.Parse((sender as Border).Name.Replace("Song", ""));

            OpenMedia(CurrentPlaylist.Songs[CurrentSongIndex]);
            PlayMusic();
        }

        #region Playlist Controls
        private void PlaylistSongsClick(object sender, RoutedEventArgs e)
        {
            PlaylistSettings.Visibility = Visibility.Collapsed;
            PlaylistArtists.Visibility = Visibility.Collapsed;
            PlaylistSongs.Visibility = Visibility.Visible;
        }

        private void PlaylistArtistsClick(object sender, RoutedEventArgs e)
        {
            PlaylistSettings.Visibility = Visibility.Collapsed;
            PlaylistSongs.Visibility = Visibility.Collapsed;
            PlaylistArtists.Visibility = Visibility.Visible;
        }

        private void PlaylistSettingsClick(object sender, RoutedEventArgs e)
        {
            PlaylistArtists.Visibility = Visibility.Collapsed;
            PlaylistSongs.Visibility = Visibility.Collapsed;
            PlaylistSettings.Visibility = Visibility.Visible;
        }
        #endregion Playlist Controls

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LoadPlaylist(MyMusic);
        }

        private void Slider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Save settings
            FileHandler.SaveSettings(Settings);
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return String.Format("{0} {1}", values[0], values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
