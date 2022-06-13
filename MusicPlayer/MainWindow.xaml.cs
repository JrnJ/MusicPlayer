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
using MusicPlayer.Models;
using System.Runtime.CompilerServices;

namespace MusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // Classes
        public SystemMediaTransportControls MediaControls;

        private AudioPlayer _audioPlayer;

        public AudioPlayer AudioPlayer
        {
            get { return _audioPlayer; }
            set { _audioPlayer = value; }
        }

        private ObservableCollection<Playlist> _playlists;

        public ObservableCollection<Playlist> Playlists
        {
            get { return _playlists; }
            set { _playlists = value; OnPropertyChanged(); }
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

        public RadioButton CurrentUISong = new RadioButton();
        public int CurrentSongIndex = 0;

        public double SongTotalMs;

        public bool LeftMouseDownOnSlider = false;

        public MainWindow()
        {
            InitializeComponent();

            ConfigureAudioPlayer();
            ConfigurePlaylists();
            ConfigureSettings();

            // Enable this if you still want music, will break MVVM
            DataContext = this;
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
            }
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
            AudioPlayer.Pause();

            if (SelectedPlaylist.Songs.Count - 1 > CurrentSongIndex)
            {
                CurrentSongIndex += 1;
                OpenMedia(SelectedPlaylist.Songs[CurrentSongIndex]);
                AudioPlayer.Play();
            }
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
            UIElement uiElement = (UIElement)icSongs.ItemContainerGenerator.ContainerFromIndex(CurrentSongIndex);
            CurrentUISong = VisualTreeHelper.GetChild(uiElement, 0) as RadioButton;
            CurrentUISong.IsChecked.Equals(true);

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

        public void LoadPlaylist(Playlist playlist)
        {
            SelectedPlaylist = playlist;

            // Loop thru all songs
            // ?!? => CurrentSongUi, I dont think this even has to be done
            //for (int i = 0; i < playlist.Songs.Count; i++)
            //{
            //    spPlaylistSongs.Children.Add(CreateSongUI(playlist.Songs[i], i));

            //    // Add song to screen
            //    if (playlist.Songs[i] == CurrentSong)
            //    {
            //        CurrentSongIndex = i;

            //        // Change new background
            //        CurrentUISong = FindName($"Song{CurrentSongIndex}") as Button;
            //        CurrentUISong.Style = Application.Current.FindResource("SelectedAlbumSong") as Style;
            //    }
            //}
        }

        #region CreateUI
        //public Button CreateSongUI(Song song, int index)
        //{
        //    SongPlaylists songPlaylists = new()
        //    {
        //        Song = song,
        //        Playlists = Playlists
        //    };

        //    // Create UI Element
        //    Button btn = new()
        //    {
        //        Style = Application.Current.FindResource("AlbumSong") as Style,
        //        DataContext = songPlaylists,
        //        Tag = song.Id
        //    };
        //    btn.Click += AlbumSongClick;
        //    btn.Name = RegisterAndOrSetName($"Song{index}", btn);

        //    ContextMenu cm = new()
        //    {
        //        Tag = songPlaylists.Song.Id
        //    };

        //    MenuItem miPlay = new()
        //    {
        //        Header = "Play"
        //    };
        //    miPlay.Click += MiPlayClick;

        //    MenuItem miPlaylists = new()
        //    {
        //        Style = Application.Current.FindResource("Test") as Style,

        //        Header = "Add to",
        //        ItemsSource = songPlaylists.Playlists,
        //        //DisplayMemberPath = "Name",
        //        //Tag = "{Binding Path=Id}"
        //    };
        //    Binding binding = new Binding("Name");
        //    binding.Source = miPlaylists;
        //    miPlaylists.SetBinding(TagProperty, binding);
        //    miPlaylists.Click += MiAddToPlaylistClick;

        //    MenuItem miDeleteFromPlaylist = new()
        //    {
        //        Header = "Remove"
        //    };

        //    MenuItem miProperties = new()
        //    {
        //        Header = "Properties"
        //    };

        //    cm.Items.Add(miPlay);
        //    cm.Items.Add(miPlaylists);
        //    cm.Items.Add(miDeleteFromPlaylist);

        //    cm.Items.Add(miProperties);
        //    btn.ContextMenu = cm;

        //    return btn;
        //}
        #endregion CreateUI

        #region CreateUIEvents
        private void PlaylistItemClick(object sender, RoutedEventArgs e)
        {
            int playlistId = int.Parse((sender as FrameworkElement).Tag.ToString());

            // ehm maybe not :)
            Playlist playlist = Playlists.ToList().Find(x => x.Id == playlistId);
            LoadPlaylist(playlist);
        }

        private void AlbumSongClick(object sender, RoutedEventArgs e)
        {
            int songId = int.Parse((sender as FrameworkElement).Tag.ToString());
            Song song = SelectedPlaylist.Songs.Where(x => x.Id == songId).First();
            CurrentSongIndex = SelectedPlaylist.Songs.IndexOf(song);

            OpenMedia(SelectedPlaylist.Songs[CurrentSongIndex]);
        }

        // ContextMenuEvents
        private void MiPlayClick(object sender, RoutedEventArgs e)
        {
            int songId = int.Parse(((sender as FrameworkElement).Parent as FrameworkElement).Tag.ToString());

            for (int i = 0; i < SelectedPlaylist.Songs.Count; i++)
            {
                if (SelectedPlaylist.Songs[i].Id == songId)
                {
                    CurrentSongIndex = SelectedPlaylist.Songs[i].Id;
                    OpenMedia(SelectedPlaylist.Songs[CurrentSongIndex]);
                    break;
                }
            }
        }

        private void MiAddToPlaylistClick(object sender, RoutedEventArgs e)
        {
            // Add item to playlist
            int songId = int.Parse(((sender as FrameworkElement).Parent as FrameworkElement).Tag.ToString());
            string playlistId = (sender as FrameworkElement).Tag.ToString();
            FrameworkElement parent = sender as FrameworkElement;

            MessageBox.Show(songId.ToString() + " | " + playlistId.ToString());
        }
        #endregion CreateUIEvents

        #region VolumeSlider
        private void MyMusicClick(object sender, RoutedEventArgs e)
        {
            LoadPlaylist(MyMusic);
        }

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

        #region PlaylistSettings
        private void DeletePlaylistClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Playlists.Remove(Playlists.Where(x => x.Id == int.Parse((sender as Button).Tag.ToString())).First());
                FileHandler.SavePlaylists(Playlists);

                LoadPlaylist(MyMusic);
            }
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            tblSearch.Text = (sender as TextBox).Text == "" ? "Search..." : "";

            // Check textbox
            // ?!? => Need names and stuff, fix later
            //if (!string.IsNullOrWhiteSpace((sender as TextBox).Text))
            //{
            //    // Clear playlist
            //    spPlaylistSongs.Children.Clear();

            //    for (int i = 0; i < MyMusic.Songs.Count; i++)
            //    {
            //        // Check if song contains them
            //        if (!string.IsNullOrWhiteSpace(MyMusic.Songs[i].Title))
            //            if (MyMusic.Songs[i].Title.ToLower().Contains((sender as TextBox).Text.ToLower()) || 
            //                MyMusic.Songs[i].ContributingArtists.ToLower().Contains((sender as TextBox).Text.ToLower()))
            //                spPlaylistSongs.Children.Add(CreateSongUI(MyMusic.Songs[i], i));
            //    }
            //}
            //else
            //{
            //    // Clear and fill the search playlist
            //    spPlaylistSongs.Children.Clear();
            //    for (int i = 0; i < MyMusic.Songs.Count; i++)
            //    {
            //        spPlaylistSongs.Children.Add(CreateSongUI(MyMusic.Songs[i], i));
            //    }
            //}
        }
        #endregion PlaylistSettings

        #region PlaylistItemsMenu
        private void AddPlaylistClick(object sender, RoutedEventArgs e)
        {
            // Create sample template to add a new playlist
            Playlist playlist = new Playlist(Playlists.Count, new ObservableCollection<Song>(), "New Playlist", "New Playlist");

            Playlists.Add(playlist);
            LoadPlaylist(playlist);

            // Save Playlists
            FileHandler.SavePlaylists(Playlists);
        }
        #endregion PlaylistsTab

        // Keep at bottom
        #region HelperMethods

        // < ! ! ! ! ! ! ! ! ! ! ! ! ! ! ! ! > //
        //
        // Place in HelperMethods.cs if possible
        //
        // < ! ! ! ! ! ! ! ! ! ! ! ! ! ! ! ! > //

        /// <summary>
        /// Registers an object with a name
        /// Unregisters if object already exists
        /// </summary>
        /// <param name="name">Object's x:Name</param>
        /// <param name="obj">Object</param>
        /// <returns></returns>
        private string RegisterAndOrSetName(string name, object obj)
        {
            if (FindName(name) != null)
                UnregisterName(name);

            RegisterName(name, obj);

            return name;
        }
        #endregion HelperMethods

        #region Interface Implementations
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion Interface Implementations

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("UwU");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string tag = (sender as FrameworkElement).Tag.ToString();

            switch (tag)
            {
                case "":
                    break;
            }
        }
    }
}
