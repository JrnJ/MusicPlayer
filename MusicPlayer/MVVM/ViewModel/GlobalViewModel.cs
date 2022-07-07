using MusicPlayer.Classes;
using MusicPlayer.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MusicPlayer.MVVM.ViewModel
{
    internal class GlobalViewModel : ObservableObject
    {
        public static GlobalViewModel Instance { get; } = new();

        // Globals

        // Current View
        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(); }
        }

        // ViewModels
        public PlaylistViewModel PlaylistVM { get; set; }

        // MusicPlayer
        private AudioPlayer _audioPlayer;

        public AudioPlayer AudioPlayer
        {
            get { return _audioPlayer; }
            set { _audioPlayer = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Playlist> _playlists;

        public ObservableCollection<Playlist> Playlists
        {
            get => _playlists;
            set { _playlists = value; OnPropertyChanged(); }
        }

        private Playlist _selectedPlaylist;

        public Playlist SelectedPlaylist
        {
            get => Playlists.FirstOrDefault(x => x.Id == _selectedPlaylist.Id);
            set
            {
                Playlist playlist = Playlists.FirstOrDefault(x => x.Id == value.Id);

                if (playlist != null)
                {
                    _selectedPlaylist = value;
                    Playlists[Playlists.IndexOf(playlist)] = playlist;
                    OnPropertyChanged();
                }
            }
        }

        private Playlist _myMusic;

        public Playlist MyMusic
        {
            get { return _myMusic; }
            set { _myMusic = value; OnPropertyChanged(); }
        }

        private string _finalTime;

        public string FinalTime
        {
            get { return _finalTime; }
            set { _finalTime = value; OnPropertyChanged(); }
        }

        private string _currentTime;

        public string CurrentTime
        {
            get { return _currentTime; }
            set { _currentTime = value; OnPropertyChanged(); }
        }

        private double _sliderValue;

        public double SliderValue
        {
            get { return _sliderValue; }
            set { _sliderValue = value; OnPropertyChanged(); }
        }

        public int CurrentSongIndex { get; set; }

        // ViewModels
        public PlaylistsViewModel PlaylistsVM { get; set; }

        // Commands ?
        public RelayCommand SelectPlaylistCommand { get; set; }

        public RelayCommand DeletePlaylistCommand { get; set; }

        public GlobalViewModel()
        {
            // Create ViewModels
            PlaylistsVM = new();
            PlaylistVM = new();

            // Load Playlists
            Playlists = FileHandler.GetPlaylists();

            // Assign Commands
            SelectPlaylistCommand = new(o =>
            {
                ShowPlaylist((int)o);
            });

            // Configuration
            ConfigureAudioPlayer();
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

        private void MediaPlayerMediaEnded(Windows.Media.Playback.MediaPlayer sender, object args)
        {
            // Call to MainWindow thread
            //Dispatcher.Invoke(() =>
            //{
                
            //});

            AudioPlayer.Pause();

            if (SelectedPlaylist.Songs.Count - 1 > CurrentSongIndex)
            {
                CurrentSongIndex += 1;
                OpenMedia(SelectedPlaylist.Songs[CurrentSongIndex]);
                AudioPlayer.Play();
            }
        }

        public void OpenMedia(Song song)
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
            

            // Update UI
            CurrentTime = "0:00";
            SliderValue = 0.0;
            FinalTime = HelperMethods.MsToTime(song.Length);

            // Remove whenever this mess is fixed
            AudioPlayer.Play();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateTime();
        }

        private void UpdateTime()
        {
            // Set Slider Accordingly
            SliderValue = AudioPlayer.MediaPlayer.Position.TotalMilliseconds;

            // Update UI
            CurrentTime = HelperMethods.MsToTime(AudioPlayer.MediaPlayer.Position.TotalMilliseconds);
        }
        #endregion Configuration

        public void CreatePlaylist()
        {
            int playlistId = Playlists.Count;
            while (Playlists.Where(x => x.Id == playlistId).FirstOrDefault() != null)
                playlistId++;

            Playlist playlist = new(playlistId, new(), "New Playlist", "None");
            Playlists.Add(playlist);
            FileHandler.SavePlaylists(Playlists);
        }

        public void ShowPlaylist(int playlistId)
        {
            SelectedPlaylist = Playlists.Where(x => x.Id == playlistId).FirstOrDefault();

            // Maybe do this whenever SelectedPlaylist is changed, might f up if a different view is made though
            CurrentView = PlaylistVM;
        }

        public void DeletePlaylist(int playlistId)
        {
            Playlists.Remove(Playlists.Where(x => x.Id == playlistId).FirstOrDefault());
            FileHandler.SavePlaylists(Playlists);
        }

        public void AddSongToPlaylist(Song song, Playlist playlist)
        {
            // Add Song to Playlist
            Playlists[Playlists.IndexOf(playlist)].Songs.Add(song);
            FixPlaylistSongIds(playlist);

            // Save Playlists
            FileHandler.SavePlaylists(Playlists);
        }

        public void RemoveSongFromPlaylist(Song song, Playlist playlist)
        {
            // Remove song from Playlist
            Playlists[Playlists.IndexOf(playlist)].Songs.Remove(song);
            FixPlaylistSongIds(playlist);

            // Save Playlists
            FileHandler.SavePlaylists(Playlists);
        }

        private void FixPlaylistSongIds(Playlist playlist)
        {
            Playlist temp = Playlists[Playlists.IndexOf(playlist)];

            for (int i = 0; i < temp.Songs.Count; i++)
            {
                Playlists[Playlists.IndexOf(playlist)].Songs[i].SetId(i);
            }
        }
    }
}
