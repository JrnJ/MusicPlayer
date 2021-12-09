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

namespace MusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // Classes
        public Windows.Media.Playback.MediaPlayer MediaPlayer;
        public DispatcherTimer Timer = new DispatcherTimer();
        public SystemMediaTransportControls MediaControls;
        public Debugger Debugger;
        //public MusicEvents MusicEvents = new MusicEvents();

        private double _volume;

        public double Volume
        {
            get { return _volume; }
            set { _volume = value; OnPropertyChanged(); MediaPlayer.Volume = value; }
        }

        private ObservableCollection<Playlist> _playlists;

        public ObservableCollection<Playlist> Playlists
        {
            get { return _playlists; }
            set { _playlists = value; }
        }


        public Playlist CurrentPlaylist = new Playlist(0, null, null);
        public Border CurrentUISong = new Border();
        public Song CurrentSong;
        public int CurrentSongIndex = 0;

        public double SongTotalMs;
        public bool IsPlaying = false;

        public bool LeftMouseDownOnSlider = false;
        public string[] LookForMusic = { 
            "C:/Users/jeroe/OneDrive - Summacollege/Overzetten/Music/Random",
            "C:/Users/jeroe/OneDrive - Summacollege/Overzetten/Music/Trap Electro Etc"
        };

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
            //Debugger.Title = "Console";

            //MediaControls = SystemMediaTransportControls.GetForCurrentView();
            //MediaControls.ButtonPressed += MediaControls_ButtonPressed;

            ConfigureMediaPlayer();

            // Load Playlist
            // C:/Users/jeroe/OneDrive - Summacollege/Overzetten/Music/Random
            // D:/Music/Anime

            // Load Playlists
            Playlists = new ObservableCollection<Playlist>();
            Playlists = FileHandler.GetPlaylistsLocation();

            for (int i = 0; i < LookForMusic.Length; i++)
            {
                Playlists.Add(MusicHandler.GetMusicFromFolder(LookForMusic[i]));
            }

            CurrentPlaylist = Playlists[0];
            if (CurrentPlaylist == null)
            {
                MessageBox.Show("Folder location not found!");
            }
            else
            {
                LoadPlaylist(CurrentPlaylist);
            }

            // Instantiate Track Controls
            //sliderSongTimePlayed.ApplyTemplate();

            DataContext = this;
        }

        private void ConfigureMediaPlayer()
        {
            MediaPlayer = new Windows.Media.Playback.MediaPlayer();

            // Mediaplayer events
            MediaPlayer.MediaOpened += MediaPlayerMediaOpened;
            MediaPlayer.MediaEnded += MediaPlayerMediaEnded;
            MediaPlayer.CommandManager.IsEnabled = true;

            // try get next song to work
            MediaPlayer.AutoPlay = true;

            // Instantiate Timer
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Tick += Timer_Tick;

            // Specific Controls
            Volume = 0.5;
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
            tblCurrentTime.Text = MsToTime(MediaPlayer.Position.TotalMilliseconds);
        }
        #endregion MusicThings

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateTime();
        }

        private static string MsToTime(double ms)
        { 
            string time = new DateTime().AddMilliseconds(ms).ToString("H:mm:ss");
            string finalTime = "";

            string[] timeStamps = time.Split(':');

            // Check if strin contains something other than 0
            for (int i = 0; i < timeStamps.Length; i++)
            {
                foreach (char c in timeStamps[i])
                {
                    if (c != '0')
                    {
                        finalTime += timeStamps[i] + ":";
                        break;
                    }
                }
            }

            if (finalTime.Length > 0)
            {
                // Remove last :
                finalTime = finalTime.Remove(finalTime.Length - 1);

                if (finalTime.Length < 4)
                {
                    finalTime = "0:" + finalTime;
                }
                else if (finalTime[0].ToString() == "0")
                {
                    finalTime = finalTime.Remove(0, 1);
                }
            }
            else
            {
                finalTime = "0:00";
            }

            return finalTime;
        }

        private void OpenMedia(Song song)
        {
            // Change this fast lmfao
            CurrentSong = song;

            MediaPlayer.Pause();

            //MediaPlayer.Open();
            MediaPlayer.SetUriSource(song.Path);

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
            tblFinalTime.Text = MsToTime(SongTotalMs);
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
                    Volume += 0.05;
                    break;
                case Key.NumPad2:
                    Volume -= 0.05;
                    break;
                #endregion ChangeVolume
                default:
                    break;
            }
        }

        private void sliderSongTimePlayed_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LeftMouseDownOnSlider = true;
            double value = sliderSongTimePlayed.Maximum / sliderSongTimePlayed.ActualWidth * e.GetPosition(sliderSongTimePlayed).X;
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
            tblPlaylistTitle.Text = playlist.Name;

            // Loop thru all songs
            for (int i = 0; i < playlist.Songs.Count; i++)
            {
                // Add song to screen
                AlbumSongs.Children.Add(CreateSongUI(playlist.Songs[i], i));
            }
        }

        private void AddPlaylistClick(object sender, RoutedEventArgs e)
        {
            spAlbums.Children.Add(CreateAlbumUI());
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
                Text = MsToTime(song.Length),
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

        public Border CreateAlbumUI()
        {
            Border border = new()
            {
                Height = 40,
            };

            Grid grid = new()
            {

            };

            Button button = new()
            {
                Content = "Playlist",
            };
            button.Click += SelectPlaylistClick;

            // Add Children
            grid.Children.Add(button);

            border.Child = grid;

            return border;
        }

        private void SelectPlaylistClick(object sender, RoutedEventArgs e)
        {
            GridAlbumViewer.Visibility = Visibility.Collapsed;
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

        // At Bottom pls
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
