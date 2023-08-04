using Microsoft.UI.Xaml.Media.Imaging;
using MusicPlayer.Classes;
using MusicPlayer.Core;
using MusicPlayer.DiscordGameSDK;
using MusicPlayer.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Windows.ApplicationModel.Activation;
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

        private PlaylistModel _searchingPlaylist;

        public PlaylistModel SearchingPlaylist
        {
            get { return _searchingPlaylist; }
            set { _searchingPlaylist = value; OnPropertyChanged(); }
        }

        // Stupidity
        public bool ShufflePlaylistEnabled { get; set; }

        public bool LoopPlaylistEnabled { get; set; }

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

        public string PlaylistsFilePath => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\JeroenJ\\MusicPlayer\\playlists.json";

        public string CachedSongsFilePath => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\JeroenJ\\MusicPlayer\\cached_songs.json";
        #endregion Properties

        private DiscordGameSDKWrapper _discordGameSDKWrapper;

        public DiscordGameSDKWrapper DiscordGameSDKWrapper
        {
            get { return _discordGameSDKWrapper; }
            set { _discordGameSDKWrapper = value; }
        }

        public SystemVolumeChanger SystemVolumeChanger { get; set; }

        // TODO: caching breaks when a song is added to a Folder
        public GlobalViewModel()
        {
            // Create ViewModels
            PlaylistsVM = new();
            PlaylistVM = new();

            // Configuration
            Configure();
            SystemVolumeChanger = new();

            // Set BoxModels :: This isnt mandatory btw
            ConfirmBox = new();
            EditPlaylistBox = new();

            //AudioPlayer.MediaPlayer.AudioCategory
            //AudioPlayer.MediaPlayer.AudioStateMonitor
            //AudioPlayer.MediaPlayer.PlaybackMediaMarkers
            //AudioPlayer.MediaPlayer.PlaybackSession

            //DiscordGameSDKWrapper = new("1035920401445957722");
        }

        #region Configuration
        public async void Configure()
        {
            // Validate
            await Validate();

            // Configure
            await ConfigureSettings();
            ConfigureMusic();

            ConfigureAudioPlayer();
            PopupVisibility = Visibility.Collapsed;
            SingleSongVisibility = Visibility.Collapsed;
            PlaylistViewing = MyMusic;

            // CLA
            string[] cmdLine = Environment.GetCommandLineArgs();
            if (cmdLine.Length > 1)
            {
                AlbumSongModel song = new()
                {
                    Id = 0,
                    Path = cmdLine[1]
                };
                StorageFile storageFile = await StorageFile.GetFileFromPathAsync(cmdLine[1]);
                await song.Init(storageFile);

                OpenMedia(song, true);
            }
        }

        public async Task Validate()
        {
            // Check for JeroenJ folder
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            try
            {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(path);
                await folder.CreateFolderAsync("JeroenJ", CreationCollisionOption.FailIfExists);
            }
            catch (Exception)
            {

            }

            try
            {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(path + "\\JeroenJ");
                await folder.CreateFolderAsync("MusicPlayer", CreationCollisionOption.FailIfExists);
            }
            catch (Exception)
            {

            }

            try
            {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(path + "\\JeroenJ\\MusicPlayer");
                await folder.CreateFolderAsync("Themes", CreationCollisionOption.FailIfExists);
            }
            catch (Exception)
            {
                
            }

            try
            {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(path + "\\JeroenJ\\MusicPlayer");
                await folder.CreateFolderAsync("Profiles", CreationCollisionOption.FailIfExists);
            }
            catch (Exception)
            {

            }
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

            // Create Temporary collections
            List<StorageFile> storageFiles = new();
            List<StorageFile> nonCachedSongs = new();

            // Load Cached Music
            ObservableCollection<CachedSong> cachedSongs = await FileHandler<ObservableCollection<CachedSong>>.GetJSON(CachedSongsFilePath);
            cachedSongs ??= new();

            AppSettinggs.CachedSongs = cachedSongs;

            // Load UI
            // Get All Files in All Folders
            for (int i = 0; i < AppSettinggs.MusicFolders.Count; i++)
            {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(AppSettinggs.MusicFolders[i].Path);
                IReadOnlyList<StorageFile> files = await folder.GetFilesAsync();
                ObservableCollection<StorageFile> musicFiles = new();
                foreach (StorageFile file in files)
                {
                    if (HelperMethods.IsMusicFile(file.Path))
                    {
                        musicFiles.Add(file);
                    }
                }

                // Loop over Files in Folder
                //foreach (StorageFile file in files)
                //{
                //    // Check if File is Music
                //    if (HelperMethods.IsMusicFile(file.Path))
                //    {
                //        bool isFileInCache = false;

                //        // Check if File exists in the cache
                //        for (int j = 0; j < cachedSongs.Count; j++)
                //        {
                //            if (cachedSongs[j].Path == file.Path)
                //            {
                //                // Add to MyMusic
                //                storageFiles.Add(file);
                //                MyMusic.AddSong(file);

                //                // Remove and Break
                //                cachedSongs.RemoveAt(j);
                //                isFileInCache = true;
                //                j--;
                //                break;
                //            }
                //        }

                //        if (!isFileInCache)
                //        {
                //            // Do Something
                //            nonCachedSongs.Add(file);
                //        }
                //    }
                //}

                // Loop over Songs in Cache
                for (int j = 0; j < cachedSongs.Count; j++)
                {
                    // Check if File in Cache exists in Folder
                    for (int k = 0; k < musicFiles.Count; k++)
                    {
                        if (cachedSongs[j].Path == musicFiles[k].Path)
                        {
                            storageFiles.Add(musicFiles[k]);
                            MyMusic.AddSong(musicFiles[k], cachedSongs[j].Id);

                            // Remove and break
                            musicFiles.RemoveAt(k);
                            k--;
                            break;
                        }
                    }
                }

                // Add remaining files to nonCachedSongs
                for (int j = 0; j < musicFiles.Count; j++)
                {
                    nonCachedSongs.Add(musicFiles[j]);
                }
            }

            // Load Playlists
            // TODO: Loading screen until 25 songs have been loaded
            GetPlaylists();

            await ConfigureFullSongs(nonCachedSongs);

            await CreateNewCache();
        }

        private async Task ConfigureFullSongs(List<StorageFile> nonCachedSongs)
        {
            // Load Full Songs
            for (int i = 0; i < MyMusic.Songs.Count; i++)
            {
                await MyMusic.Songs[i].Init(await StorageFile.GetFileFromPathAsync(MyMusic.Songs[i].Path));
            }

            // Load Non Cached Songs
            for (int i = 0; i < nonCachedSongs.Count; i++)
            {
                await MyMusic.AddSong(nonCachedSongs[i]).Init(await StorageFile.GetFileFromPathAsync(nonCachedSongs[i].Path));
            }
        }

        private async Task CreateNewCache()
        {
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

        public async void GetPlaylists()
        {
            Playlists = new();

            // Get all Playlists and make new if none exists
            ObservableCollection<PlaylistModel> playlists = await FileHandler<ObservableCollection<PlaylistModel>>.GetJSON(PlaylistsFilePath);
            playlists ??= new();

            for (int i = 0; i < playlists.Count; i++)
            {
                // Add Songs
                for (int j = 0; j < playlists[i].Songs.Count; j++)
                {
                    if (MyMusic.Songs.FirstOrDefault(x => x.Id == playlists[i].Songs[j].Id) != null)
                    {
                        playlists[i].Songs[j] = MyMusic.Songs.FirstOrDefault(x => x.Id == playlists[i].Songs[j].Id);
                    }
                    else
                    {
                        playlists[i].Songs.RemoveAt(j);
                        j--;
                    }
                }

                // Add Playlist
                Playlists.Add(playlists[i]);
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

            // Slider Values
            CurrentTime = "0:00";
            FinalTime = "0:00";
        }

        #region MediaPlayerEvents
        private void SystemMediaTransportControlsButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            if (args.Button == SystemMediaTransportControlsButton.Previous)
            {
                // 0x11:    CTRL / VK_MENU
                // 0x12:    ALT / VK_MENU
                float amount = ExternalInputHelper.IsKeyDown(0x12) ? (ExternalInputHelper.IsKeyDown(0x11) ? 0.01f : 0.10f) : 0.0f;

                if (amount == 0.0f)
                {
                    if (ExternalInputHelper.IsKeyDown(0x11))
                        SystemVolumeChanger.DecreaseSystemVolume(0.01f);
                    else
                        PreviousSong();
                }
                else
                {
                    AudioPlayer.SetVolume(AppSettinggs.Volume - amount);
                }
            }

            if (args.Button == SystemMediaTransportControlsButton.Next)
            {
                // 0x11:    CTRL / VK_MENU
                // 0x12:    ALT / VK_MENU
                float amount = ExternalInputHelper.IsKeyDown(0x12) ? (ExternalInputHelper.IsKeyDown(0x11) ? 0.01f : 0.10f) : 0.0f;

                if (amount == 0.0f)
                {
                    if (ExternalInputHelper.IsKeyDown(0x11))
                        SystemVolumeChanger.IncreaseSystemVolume(0.01f);
                    else
                        NextSong();
                }
                else
                {
                    AudioPlayer.SetVolume(AppSettinggs.Volume + amount);
                }
            }
        }

        private void MediaPlayerMediaEnded(Windows.Media.Playback.MediaPlayer sender, object args)
        {
            if (ShufflePlaylistEnabled)
            {
                AutoPlayNextSong();
            }
            else
            {
                NextSong();
            }
        }

        private void MediaPlayerVolumeChanged(Windows.Media.Playback.MediaPlayer sender, object args)
        {
            // Apply volume change to settings
            AppSettinggs.Volume = sender.Volume;
        }

        #endregion MediaPlayerEvents

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateTime();
        }

        private void UpdateTime()
        {
            // Set Slider Accordingly
            SliderValue = AudioPlayer.Position.TotalMilliseconds;

            // Update UI
            CurrentTime = HelperMethods.MsToTime(AudioPlayer.Position.TotalMilliseconds);
        }
        #endregion Configuration

        #region Slider
        public bool WasMouseOnSliderDown { get; private set; }

        public bool WasMusicPlaying { get; private set; }

        public void SliderMouseDownOrUpEvent(bool mousedown)
        {
            if (mousedown)
            {
                WasMouseOnSliderDown = true;

                // Pause Timer
                // TODO:
                WasMusicPlaying = AudioPlayer.CurrentState == AudioPlayerState.Playing;
                AudioPlayer.Pause();
                AudioPlayer.Timer.Stop();
            }
            else
            {
                // Make sure mouse was down
                if (WasMouseOnSliderDown)
                {
                    WasMouseOnSliderDown = false;

                    // Change where song is at
                    AudioPlayer.Position = TimeSpan.FromMilliseconds(SliderValue);
                    UpdateTime();

                    // Start Timer if it was playing
                    if (WasMusicPlaying)
                    {
                        AudioPlayer.Play();
                        AudioPlayer.Timer.Start();
                    }
                }
            }
        }
        #endregion Slider

        public void OpenMedia(AlbumSongModel song, bool singleSongMode = false)
        {
            AudioPlayer.OpenMedia(song);

            // TODO: mode can be SingleSong
            if (PlaylistViewing != null)
            {
                PlaylistPlaying = PlaylistViewing;

                if (singleSongMode)
                {
                    SingleSongVisibility = Visibility.Visible;
                }
            }

            // Update UI
            CurrentTime = "0:00";
            SliderValue = 0.0;
            FinalTime = HelperMethods.MsToTime(song.MusicProperties.Duration.TotalMilliseconds);

            // Remove whenever this mess is fixed
            AudioPlayer.Play();
        }

        public void PreviousSong()
        {
            if (AudioPlayer.CurrentSong == null)
                return;

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
            if (AudioPlayer.CurrentSong == null)
                return;

            AudioPlayer.Pause();

            // Get song Index
            int index = PlaylistPlaying.Songs.IndexOf(AudioPlayer.CurrentSong);

            // Play next song if there is one
            if (index < PlaylistPlaying.Songs.Count - 1)
            {
                OpenMedia(PlaylistPlaying.Songs[index + 1]);
            }
            // If not, play first song
            else
            {
                if (LoopPlaylistEnabled)
                {
                    OpenMedia(PlaylistPlaying.Songs[0]);
                }
            }
        }

        public void AutoPlayNextSong()
        {
            AudioPlayer.Pause();

            // Generate random
            Random random = new();
            int rndIndex = random.Next(0, PlaylistPlaying.Songs.Count);
            OpenMedia(PlaylistPlaying.Songs[rndIndex]);
        }

        public PlaylistModel CreatePlaylist()
        {
            // Create an id 
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

        public void DeletePlaylist(int playlistId)
        {
            Playlists.Remove(Playlists.Where(x => x.Id == playlistId).FirstOrDefault());
            SavePlaylists();
        }

        public void ShowPlaylist(int playlistId)
        {
            if (PlaylistViewing != null)
            {
                PlaylistViewing.IsSelected = false;
            }

            PlaylistViewing = Playlists.FirstOrDefault(x => x.Id == playlistId);
            PlaylistViewing.IsSelected = true;
            CurrentView = PlaylistVM;
        }

        public void UpdatePlaylist(int playlistId, PlaylistModel playlist)
        {
            Playlists[Playlists.IndexOf(Playlists.Where(x => x.Id == playlistId).FirstOrDefault())] = new()
            {
                Id = playlistId,
                Songs = playlist.Songs,
                Name = playlist.Name,
                Description = playlist.Description,
                ImagePath = playlist.ImagePath,
            };
            SavePlaylists();
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

        /// <summary>
        /// Uses PlaylistViewing to look for a song
        /// </summary>
        /// <param name="value">Text to search on</param>
        public void SearchSongInPlaylist(string value)
        {
            // TODO: This is very inefficient
            // If empty, set back to PlaylistViewing
            if (!string.IsNullOrEmpty(value))
            {
                SearchingPlaylist = new()
                {
                    Id = PlaylistViewing.Id,
                    Name = PlaylistViewing.Name,
                    Songs = new(),
                    Description = PlaylistViewing.Description,
                };
                ObservableCollection<AlbumSongModel> songs = new();

                for (int i = 0; i < PlaylistViewing.Songs.Count; i++)
                {
                    if (PlaylistViewing.Songs[i].MusicProperties.Title.ToLower().Contains(value.ToLower()))
                    {
                        songs.Add(PlaylistViewing.Songs[i]);
                    }
                }

                SearchingPlaylist.Songs = songs;
                PlaylistViewing = SearchingPlaylist;
            }
            else
            {
                PlaylistViewing = Playlists.Where(x => x.Id == PlaylistViewing.Id).FirstOrDefault();
            }
        }

        /// <summary>
        /// Uses MyMusic to look for a song
        /// </summary>
        /// <param name="value">Text to search on</param>
        public void SearchSongInMyMusic(string value)
        {
            // TODO: This is very inefficient
            // If empty, set back to PlaylistViewing
            if (!string.IsNullOrEmpty(value))
            {
                SearchingPlaylist = new()
                {
                    Id = PlaylistViewing.Id,
                    Name = PlaylistViewing.Name,
                    Songs = new(),
                    Description = PlaylistViewing.Description,
                };
                ObservableCollection<AlbumSongModel> songs = new();

                for (int i = 0; i < PlaylistViewing.Songs.Count; i++)
                {
                    if (PlaylistViewing.Songs[i].MusicProperties.Title.ToLower().Contains(value.ToLower()))
                    {
                        songs.Add(PlaylistViewing.Songs[i]);
                    }
                }

                SearchingPlaylist.Songs = songs;
                PlaylistViewing = SearchingPlaylist;
            }
            else
            {
                PlaylistViewing = MyMusic;
            }
        }
    }
}
