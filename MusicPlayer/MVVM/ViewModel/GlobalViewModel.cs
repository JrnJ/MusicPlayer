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

        private PlaylistModel _playlistViewing;

        public PlaylistModel PlaylistViewing
        {
            get => _playlistViewing;
            set 
            { 
                _playlistViewing = value;
                OnPropertyChanged();
            }
        }

        private PlaylistModel _playlistPlaying;

        public PlaylistModel PlaylistPlaying
        {
            get => _playlistPlaying; 
            set 
            { 
                _playlistPlaying = value;
                OnPropertyChanged();
            }
        }

        private PlaylistModel _myMusic;

        public PlaylistModel MyMusic
        {
            get => _myMusic;
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

        // ViewModels
        public PlaylistsViewModel PlaylistsVM { get; set; }

        // Commands ?
        public RelayCommand SelectPlaylistCommand { get; set; }

        public RelayCommand DeletePlaylistCommand { get; set; }

        public string PlaylistsFilePath => "C:/Users/jeroe/AppData/Roaming/.jeroenj/MusicPlayer/playlists.json";
        public string CachedSongsFilePath => "C:/Users/jeroe/AppData/Roaming/.jeroenj/MusicPlayer/cached_songs.json";
        #endregion Properties

        public GlobalViewModel()
        {
            // Create ViewModels
            PlaylistsVM = new();
            PlaylistVM = new();

            // Configuration
            Configure();

            // Load Playlists
            // TODO: Loading screen until 25 songs have been loaded
            GetPlaylists();

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
        public async void Configure()
        {
            await ConfigureSettings();
            ConfigureMusic();
            ConfigureAudioPlayer();
            PopupVisibility = Visibility.Collapsed;
            SingleSongVisibility = Visibility.Collapsed;
        }

        private async Task ConfigureSettings()
        {
            // Load Settings
            AppSettinggs = new();
            await AppSettinggs.GetSettingsFromFile();
        }

        private async void ConfigureMusic()
        {
            // Create Home Playlist
            MyMusic = new();
            List<StorageFile> storageFiles = new();

            // Load Cached Music
            AppSettinggs.CachedSongs = await FileHandler<ObservableCollection<CachedSong>>.GetJSON(CachedSongsFilePath);

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

            // Load Full Songs
            for (int i = 0; i < MyMusic.Songs.Count; i++)
            {
                await MyMusic.Songs[i].Init(storageFiles[i]);
            }

            // Create new Cache
            ObservableCollection<CachedSong> newCache = new();
            for (int i = 0; i < MyMusic.Songs.Count; i++)
            {
                newCache.Add(new()
                {
                    Id = MyMusic.Songs[i].Id,
                    Path = MyMusic.Songs[i].Path,
                    Title = MyMusic.Songs[i].MusicProperties.Title,
                    Artist = MyMusic.Songs[i].MusicProperties.Artist
                });
            }

            // Save new cache if something changed
            if (newCache != AppSettinggs.CachedSongs)
                await FileHandler<ObservableCollection<CachedSong>>.SaveJSON(CachedSongsFilePath, newCache);
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
            PlaylistPlaying = Playlists.FirstOrDefault(x => x.Id == PlaylistViewing.Id);

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

        public bool WasMouseOnSliderDown { get; private set; }

        public void Thing(bool mousedown)
        {
            if (mousedown)
            {
                WasMouseOnSliderDown = true;

                // Pause Timer
                AudioPlayer.Timer.Stop();

            }
            else
            {
                // Make sure mouse was down
                if (WasMouseOnSliderDown)
                {
                    WasMouseOnSliderDown = false;

                    // Change where song is at
                    //AudioPlayer.MediaPlayer.Position = 

                    // Start Timer
                    AudioPlayer.Timer.Start();
                }
            }
        }

        public void PreviousSong()
        {
            AudioPlayer.Pause();

            // Get song Index
            int index = PlaylistPlaying.Songs.IndexOf(AudioPlayer.CurrentSong);

            // Play previous song if there is one
            if (index > 0)
            {
                OpenMedia(PlaylistPlaying.Songs[index - 1]);
            }
        }

        public void NextSong()
        {
            AudioPlayer.Pause();

            // Get song Index
            int index = PlaylistPlaying.Songs.IndexOf(AudioPlayer.CurrentSong);

            // Play next song if there is one
            if (index < PlaylistPlaying.Songs.Count - 1)
            {
                OpenMedia(PlaylistPlaying.Songs[index + 1]);
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
            //SelectedPlaylist = Playlists.FirstOrDefault(x => x.Id == playlistId);

            // TODO: This wont work with async, I think
            int playlistIndex = Playlists.IndexOf(Playlists.FirstOrDefault(x => x.Id == playlistId));
            for (int i = 0; i < Playlists[playlistIndex].Songs.Count; i++)
            {
                Playlists[playlistId].Songs[i] = MyMusic.Songs.FirstOrDefault(x => x.Id == Playlists[playlistId].Songs[i].Id);
            }
            PlaylistViewing = Playlists[playlistId];

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

            for (int i = 0; i < Playlists.Count; i++)
            {
                //for (int i = 0; i < length; i++)
                //{

                //}
            }
        }

        public async void SavePlaylists()
        {
            await FileHandler<ObservableCollection<PlaylistModel>>.SaveJSON(PlaylistsFilePath, Playlists);
        }

        public void AddSongToPlaylist(AlbumSongModel song, PlaylistModel playlist)
        {
            // Add Song to Playlist
            Playlists[Playlists.IndexOf(playlist)].Songs.Add(song);

            // Save Playlists
            SavePlaylists();
        }

        public void RemoveSongFromPlaylist(AlbumSongModel song, PlaylistModel playlist)
        {
            // Remove song from Playlist
            Playlists[Playlists.IndexOf(playlist)].Songs.Remove(song);

            // Save Playlists
            SavePlaylists();
        }
    }
}
