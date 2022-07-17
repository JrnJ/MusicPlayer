using MusicPlayer.Classes;
using MusicPlayer.Core;
using MusicPlayer.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        private ObservableCollection<PlaylistModel> _playlists;

        public ObservableCollection<PlaylistModel> Playlists
        {
            get => _playlists;
            set { _playlists = value; OnPropertyChanged(); }
        }

        private PlaylistModel _selectedPlaylist;

        public PlaylistModel SelectedPlaylist
        {
            get => Playlists.FirstOrDefault(x => x.Id == _selectedPlaylist.Id);
            set
            {
                PlaylistModel playlist = Playlists.FirstOrDefault(x => x.Id == value.Id);

                if (playlist != null)
                {
                    _selectedPlaylist = value;
                    Playlists[Playlists.IndexOf(playlist)] = playlist;
                    OnPropertyChanged();
                }
            }
        }

        private PlaylistModel _myMusic;

        public PlaylistModel MyMusic
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

        // Popups
        private Visibility _popupVisiblity;

        public Visibility PopupVisibility
        {
            get { return _popupVisiblity; }
            set { _popupVisiblity = value; OnPropertyChanged(); }
        }

        private MessageBoxModel _confirmBox;

        public MessageBoxModel ConfirmBox
        {
            get { return _confirmBox; }
            set { _confirmBox = value; OnPropertyChanged(); }
        }

        private EditBoxModel _editPlaylistBox;

        public EditBoxModel EditPlaylistBox
        {
            get { return _editPlaylistBox; }
            set { _editPlaylistBox = value; OnPropertyChanged(); }
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
            PopupVisibility = Visibility.Collapsed;

            // Set BoxModels
            ConfirmBox = new();
            EditPlaylistBox = new();

            //AudioPlayer.MediaPlayer.AudioCategory
            //AudioPlayer.MediaPlayer.AudioStateMonitor
            //AudioPlayer.MediaPlayer.PlaybackMediaMarkers
            //AudioPlayer.MediaPlayer.PlaybackSession
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

        public void OpenMedia(AlbumSongModel song)
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

            PlaylistModel playlist = new(playlistId, new(), "New Playlist", "None");
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

        public void UpdatePlaylist(int playlistId, PlaylistModel playlist)
        {
            Playlists[Playlists.IndexOf(Playlists.Where(x => x.Id == playlistId).FirstOrDefault())] = new(
                playlistId, 
                playlist.Songs, 
                playlist.Name, 
                playlist.Description
            );
            FileHandler.SavePlaylists(Playlists);
        }

        public void AddSongToPlaylist(AlbumSongModel song, PlaylistModel playlist)
        {
            // Add Song to Playlist
            Playlists[Playlists.IndexOf(playlist)].Songs.Add(song);
            FixPlaylistSongIds(playlist);

            // Save Playlists
            FileHandler.SavePlaylists(Playlists);
        }

        public void RemoveSongFromPlaylist(AlbumSongModel song, PlaylistModel playlist)
        {
            // Remove song from Playlist
            Playlists[Playlists.IndexOf(playlist)].Songs.Remove(song);
            FixPlaylistSongIds(playlist);

            // Save Playlists
            FileHandler.SavePlaylists(Playlists);
        }

        public void FixPlaylistSongIds(PlaylistModel playlist)
        {
            PlaylistModel temp = Playlists[Playlists.IndexOf(playlist)];

            for (int i = 0; i < temp.Songs.Count; i++)
            {
                Playlists[Playlists.IndexOf(playlist)].Songs[i].SetId(i);
            }
        }
    }
}
