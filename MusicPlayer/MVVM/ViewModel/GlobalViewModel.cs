using Microsoft.UI.Xaml.Media.Imaging;
using MusicPlayer.Classes;
using MusicPlayer.Core;
using MusicPlayer.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Windows.Media;
using Windows.Media.Playlists;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace MusicPlayer.MVVM.ViewModel
{
    internal class GlobalViewModel : ObservableObject
    {
        #region Properties
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

        // AppSettings
        private AppSettingsModel _appSettings;

        public AppSettingsModel AppSettinggs
        {
            get { return _appSettings; }
            set { _appSettings = value; OnPropertyChanged(); }
        }

        // MusicPlayer
        private AudioPlayer _audioPlayer;

        public AudioPlayer AudioPlayer
        {
            get { return _audioPlayer; }
            set { _audioPlayer = value; OnPropertyChanged(); }
        }

        // Playlists
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

        // Stupidity
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

        // Single View Stuff
        private Visibility _singleSongVisibility;

        public Visibility SingleSongVisibility
        {
            get { return _singleSongVisibility; }
            set { _singleSongVisibility = value; OnPropertyChanged(); }
        }

        public int CurrentSongIndex { get; set; }

        // ViewModels
        public PlaylistsViewModel PlaylistsVM { get; set; }

        // Commands ?
        public RelayCommand SelectPlaylistCommand { get; set; }

        public RelayCommand DeletePlaylistCommand { get; set; }

        public static string PlaylistsFilePath => "C:/Users/jeroe/AppData/Roaming/.jeroenj/MusicPlayer/playlists.json";
        #endregion Properties

        public GlobalViewModel()
        {
            // Create ViewModels
            PlaylistsVM = new();
            PlaylistVM = new();

            // Load Playlists
            GetPlaylists();

            // Configuration
            ConfigureSettings();
            ConfigureAudioPlayer();
            PopupVisibility = Visibility.Collapsed;
            SingleSongVisibility = Visibility.Collapsed;

            // Set BoxModels :: This isnt mandatory btw
            ConfirmBox = new();
            EditPlaylistBox = new();

            //AudioPlayer.MediaPlayer.AudioCategory
            //AudioPlayer.MediaPlayer.AudioStateMonitor
            //AudioPlayer.MediaPlayer.PlaybackMediaMarkers
            //AudioPlayer.MediaPlayer.PlaybackSession

            // Assign Commands
            SelectPlaylistCommand = new(o =>
            {
                ShowPlaylist((int)o);
            });
        }

        #region Configuration
        private async void ConfigureSettings()
        {
            // Load Settings
            AppSettinggs = new();
            await AppSettinggs.GetSettingsFromFile();

            // Create Home Playlist
            MyMusic = new();
            List<StorageFile> storageFiles = new();

            // Load UI
            for (int i = 0; i < AppSettinggs.MusicFolders.Count; i++)
            {
                string path = AppSettinggs.MusicFolders[i].Path;

                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(path);
                IReadOnlyList<StorageFile> files = await folder.GetFilesAsync();
                
                foreach (StorageFile file in files)
                {
                    if (HelperMethods.IsMusicFile(file.Path))
                    {
                        storageFiles.Add(file);
                        MyMusic.AddSong(file);
                    }
                }
            }

            // Load Other
            for (int i = 0; i < MyMusic.Songs.Count; i++)
            {
                await MyMusic.Songs[i].Init(storageFiles[i]);
            }
        }

        private void ConfigureAudioPlayer()
        {
            // AudioPlayer
            AudioPlayer = new()
            {
                Volume = AppSettinggs.Volume
            };

            AudioPlayer.MediaPlayer.MediaEnded += MediaPlayerMediaEnded;
            AudioPlayer.MediaPlayer.VolumeChanged += MediaPlayerVolumeChanged;

            // SMTC
            AudioPlayer.MediaPlayer.SystemMediaTransportControls.ButtonPressed += SystemMediaTransportControlsButtonPressed;

            // Timer Tick Event
            AudioPlayer.Timer.Tick += Timer_Tick;
        }

        #region MediaPlayerEvents
        private void SystemMediaTransportControlsButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            if (args.Button == SystemMediaTransportControlsButton.Previous)
            {
                PreviousSong();
            }

            if (args.Button == SystemMediaTransportControlsButton.Next)
            {
                NextSong();
            }
        }

        private void MediaPlayerMediaEnded(Windows.Media.Playback.MediaPlayer sender, object args)
        {
            NextSong();
        }

        private void MediaPlayerVolumeChanged(Windows.Media.Playback.MediaPlayer sender, object args)
        {
            // Apply volume change to settings
            AppSettinggs.Volume = sender.Volume;
        }

        #endregion MediaPlayerEvents

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
            FinalTime = HelperMethods.MsToTime(song.MusicProperties.Duration.TotalMilliseconds);

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


        public void PreviousSong()
        {
            AudioPlayer.Pause();

            if (CurrentSongIndex > 0)
            {
                CurrentSongIndex -= 1;
                OpenMedia(SelectedPlaylist.Songs[CurrentSongIndex]);
            }
        }

        public void NextSong()
        {
            AudioPlayer.Pause();

            if (CurrentSongIndex < SelectedPlaylist.Songs.Count - 1)
            {
                CurrentSongIndex += 1;
                OpenMedia(SelectedPlaylist.Songs[CurrentSongIndex]);
            }
        }

        public PlaylistModel CreatePlaylist()
        {
            // Ccreate an id 
            int id = 0;
            for (int i = 0; i < Playlists.Count; i++)
            {
                if (Playlists[i].Id > id)
                    id = Playlists[i].Id;
            }

            PlaylistModel playlist = new() { Id = id + 1 };
            Playlists.Add(playlist);

            SavePlaylists();

            return playlist;
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
            SavePlaylists();
        }

        public void UpdatePlaylist(int playlistId, PlaylistModel playlist)
        {
            Playlists[Playlists.IndexOf(Playlists.Where(x => x.Id == playlistId).FirstOrDefault())] = new()
            {
                Id = playlistId,
                Songs = playlist.Songs,
                Name = playlist.Name,
                Description = playlist.Description
            };
            SavePlaylists();
        }

        public async void GetPlaylists()
        {
            Playlists = await FileHandler<ObservableCollection<PlaylistModel>>.GetJSON(PlaylistsFilePath);
        }

        public async void SavePlaylists()
        {
            await FileHandler<ObservableCollection<PlaylistModel>>.SaveJSON(PlaylistsFilePath, Playlists);
        }

        public void AddSongToPlaylist(AlbumSongModel song, PlaylistModel playlist)
        {
            // Add Song to Playlist
            Playlists[Playlists.IndexOf(playlist)].Songs.Add(song);
            FixPlaylistSongIds(playlist);

            // Save Playlists
            SavePlaylists();
        }

        public void RemoveSongFromPlaylist(AlbumSongModel song, PlaylistModel playlist)
        {
            // Remove song from Playlist
            Playlists[Playlists.IndexOf(playlist)].Songs.Remove(song);
            FixPlaylistSongIds(playlist);

            // Save Playlists
            SavePlaylists();
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
