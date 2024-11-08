using Microsoft.EntityFrameworkCore;
using MusicPlayer.Classes;
using MusicPlayer.Core;
using MusicPlayer.Core.Searching;
using MusicPlayer.Database;
using MusicPlayer.DiscordGameSDK;
using MusicPlayer.MVVM.Model;
using MusicPlayer.Popups;
using MusicPlayer.Shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.Media;
using Windows.Media.Playlists;
using Windows.Storage;
using Windows.Storage.FileProperties;

using Playlist = MusicPlayer.Shared.Models.Playlist;

namespace MusicPlayer.MVVM.ViewModel
{
    internal class GlobalViewModel : ObservableObject
    {
        #region Properties
        public static GlobalViewModel Instance { get; } = new();

        // Titlebar
        private AppWindowTitlebarManager _appWindowTitlebarManager;

        public AppWindowTitlebarManager AppWindowTitlebarManager
        {
            get { return _appWindowTitlebarManager; }
            set { _appWindowTitlebarManager = value; }
        }

        // Current View
        private object? _currentView;

        public object? CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(); }
        }

        // ViewModels
        public PlaylistViewModel PlaylistVM { get; set; }
        public PlaylistsViewModel PlaylistsVM { get; set; }

        // Settings
        private SettingsModel _settings;

        public SettingsModel Settings
        {
            get { return _settings; }
            set { _settings = value; OnPropertyChanged(); }
        }

        private ObservableCollection<SongsFolderModel> _songsFolders = [];

        public ObservableCollection<SongsFolderModel> SongsFolders
        {
            get { return _songsFolders; }
            set { _songsFolders = value; OnPropertyChanged(); }
        }

        // MusicPlayer
        private AudioPlayer _audioPlayer;

        public AudioPlayer AudioPlayer
        {
            get { return _audioPlayer; }
            set { _audioPlayer = value; OnPropertyChanged(); }
        }

        // Data
        private ObservableCollection<ArtistModel> _artists = [];

        public ObservableCollection<ArtistModel> Artists
        {
            get { return _artists; }
            set { _artists = value; OnPropertyChanged(); }
        }

        private ObservableCollection<GenreModel> _genres = [];

        public ObservableCollection<GenreModel> Genres
        {
            get { return _genres; }
            set { _genres = value; OnPropertyChanged(); }
        }

        // Playlists
        private PlaylistModel _myMusic = new();

        public PlaylistModel MyMusic
        {
            get => _myMusic;
            set { _myMusic = value; OnPropertyChanged(); }
        }

        private PlaylistsManager _playlistsManager;

        public PlaylistsManager PlaylistsManager
        {
            get { return _playlistsManager; }
            set { _playlistsManager = value; }
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

        private DiscordGameSDKWrapper _discordGameSDKWrapper;

        public DiscordGameSDKWrapper DiscordGameSDKWrapper
        {
            get { return _discordGameSDKWrapper; }
            set { _discordGameSDKWrapper = value; }
        }

        public SystemVolumeChanger SystemVolumeChanger { get; set; }

        private GlobalSearch _globalSearch;

        public GlobalSearch GlobalSearch
        {
            get { return _globalSearch; }
            set { _globalSearch = value; OnPropertyChanged(); }
        }
        #endregion Properties

        // Popups
        private SongEditorPopup _songEditorPopup = new();

        public SongEditorPopup SongEditorPopup
        {
            get { return _songEditorPopup; }
            set { _songEditorPopup = value; }
        }

        public GlobalViewModel()
        {
            // AppTitlebar
            _appWindowTitlebarManager = new();

            // Before Others
            _audioPlayer = new();
            ConfigureAudioPlayer();

            _playlistsManager = new();

            // Create ViewModels
            PlaylistsVM = new();
            PlaylistVM = new();

            // Search Bar
            _globalSearch = new();

            // Configuration
            AsyncGlobalViewModel();
            // Configure();

            SystemVolumeChanger = new();

            // Set BoxModels :: This isnt mandatory btw
            ConfirmBox = new();
            EditPlaylistBox = new();

            //DiscordGameSDKWrapper = new("1035920401445957722");
        }

        #region SortThis
        // TODO2:
        public async void AddSongsFolder(StorageFolder folder)
        {
            SongsFolder songsFolder = await DbHelper.CreateSongsFolder(folder.Path, Settings.Id);

            // Update Client
            SongsFolderModel songsFolderModel = new(songsFolder);
            SongsFolders.Add(songsFolderModel);
            Settings.SongsFolders.Add(songsFolderModel);

            List<Song> folderSongs = [];

            using (DomainContext context = new())
            {
                IReadOnlyList<StorageFile> files = await folder.GetFilesAsync();
                foreach (StorageFile file in files)
                {
                    // If file is not music, continue
                    if (!HelperMethods.IsMusicFile(file.Path)) continue;

                    // Create a Song
                    Windows.Storage.FileProperties.MusicProperties musicProperties = await file.Properties.GetMusicPropertiesAsync();
                    Song song = new()
                    {
                        Path = file.Path,
                        Title = musicProperties.Title,
                        Year = unchecked((int)musicProperties.Year),
                        Duration = musicProperties.Duration,
                        SongsFolderId = songsFolder.Id,

                        Artists = [],
                        Genres = [],
                        Playlists = []
                    };

                    // Set Song artists
                    Artist? artist = context.Artists.FirstOrDefault(a => a.Name == musicProperties.Artist);
                    if (artist == null)
                    {
                        if (string.IsNullOrEmpty(musicProperties.Artist))
                        {
                            await DbHelper.CreateArtist(musicProperties.Artist, context);
                        }
                    }
                    else
                    {
                        song.Artists.Add(new()
                        {
                            ArtistId = artist.Id,
                            SongId = song.Id
                        });
                    }

                    // Set Song Genres
                    foreach (string genreName in musicProperties.Genre)
                    {
                        Genre? genre = context.Genres.FirstOrDefault(g => g.Name == genreName);
                        if (genre == null)
                        {
                            if (string.IsNullOrEmpty(genreName))
                            {
                                await DbHelper.CreateGenre(genreName, context);
                            }
                        }
                        else
                        {
                            song.Genres.Add(new()
                            {
                                SongId = song.Id,
                                GenreName = genreName,
                            });
                        }
                    }

                    // 
                    context.Songs.Add(song);
                    folderSongs.Add(song);
                }

                await context.SaveChangesAsync();
            }

            // TODO3
            // Update Artists to Client


            // Update Genres to Client


            // Update Songs to Client
            foreach (Song folderSong in folderSongs)
            {
                MyMusic.Songs.Add(new(folderSong, _artists, _genres));
            }
        }

        // TODO2: after AddSongsFolder is finished properly
        public async void RemoveSongsFolder(int songsFolderId)
        {
            // Update Database
            using (DomainContext context = new())
            {
                SongsFolder songsFolder = await context.SongsFolders.Where(sf => sf.Id == songsFolderId).FirstOrDefaultAsync();

                if (songsFolder == null) 
                    return;

                // Remove Songs in Folder
                foreach (Song song in context.Songs.Where(s => s.SongsFolderId == songsFolder.Id))
                {
                    context.Remove(song);
                }

                // Remove Folder
                context.SongsFolders.Remove(songsFolder);

                context.SaveChanges();
            }

            // Update Client
            List<SongModel> songsToRemove = MyMusic.Songs.Where(s => s.SongsFolderId == songsFolderId).ToList();
            foreach (SongModel song in songsToRemove)
            {
                MyMusic.Songs.Remove(song);

                foreach (PlaylistModel playlist in PlaylistsManager.Playlists)
                {
                    playlist.Songs.Remove(song);
                }
            }
        }
        #endregion SortThis

        #region Configuration
        public async void AsyncGlobalViewModel()
        {
            await Configure();
        }

        public async Task Configure()
        {
            LoadSettings();
            LoadFromDatabase();

            AudioPlayer.Volume = Settings.Volume;
            // TODO3
            // Method for reading all files to check if they still exist
            // and to check if the data is still correct
            // only do this for Songs that are on screen
            // if a song is clicked and the path is not found
            // you can for now throw an error

            PopupVisibility = Visibility.Collapsed;
            SingleSongVisibility = Visibility.Collapsed;
            PlaylistsManager.PlaylistViewing = MyMusic;

            // CLA
            string[] cmdLine = Environment.GetCommandLineArgs();
            if (cmdLine.Length > 1)
            {
                Song song = new()
                {
                    Id = 0,
                    Path = cmdLine[1]
                };
                StorageFile storageFile = await StorageFile.GetFileFromPathAsync(cmdLine[1]);
                OpenMedia(new(song, _artists, _genres), true);
            }
        }

        private void ConfigureAudioPlayer()
        {
            AudioPlayer.AudioEnded += MediaPlayerMediaEnded;
            AudioPlayer.VolumeChanged += MediaPlayerVolumeChanged;

            // SMTC
            AudioPlayer.OnAudioKeyPress += AudioPlayer_OnAudioKeyPress;

            // Timer Tick Event
            AudioPlayer.Timer.Tick += (sender, e) => UpdateTime();

            // Slider Values
            CurrentTime = "0:00";
            FinalTime = "0:00";
        }

        #region Loading
        public async void LoadSettings()
        {
            List<SongsFolder> songsFolders = await DbHelper.GetSongsFolders();
            foreach (SongsFolder songsFolder in songsFolders)
            {
                _songsFolders.Add(new(songsFolder));
            }

            Settings? settings = await DbHelper.GetSettings(1) ?? throw new Exception("No settings found with id: 1!");
            _settings = new(settings, _songsFolders);
        }

        public async void LoadFromDatabase()
        {
            using DomainContext context = new();

            // Artists & Genres
            var loadArtistsTask = LoadArtists(context);
            var loadGenresTask = LoadGenres(context);

            // Run async, but wait until both are done
            await Task.WhenAll(loadArtistsTask, loadGenresTask);

            // Read Songs
            List<Song> dbSongs = await context.Songs
                .Include(s => s.Artists)
                .ThenInclude(a => a.Artist)
                .Include(g => g.Genres)
                .ToListAsync();

            // Read Playlists
            List<Playlist> dbPlaylists = await DbHelper.GetPlaylists(context);

            // 4. Fill Songs
            foreach (Song song in dbSongs)
            {
                _myMusic.Songs.Add(new(song, _artists, _genres));
            }

            // 5. Fill Playlists
            foreach (Playlist playlist in dbPlaylists)
            {
                PlaylistsManager.Add(new(playlist, _myMusic.Songs));
            }
        }

        private async Task LoadArtists(DomainContext context)
        {
            List<Artist> dbArtists = await DbHelper.GetArtists(context);
            foreach (Artist artist in dbArtists)
            {
                _artists.Add(new(artist));
            }
        }

        private async Task LoadGenres(DomainContext context)
        {
            List<Genre> dbGenres = await DbHelper.GetGenres(context);
            foreach (Genre genre in dbGenres)
            {
                _genres.Add(new(genre));
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
        #endregion Loading

        #region MediaPlayerEvents
        private void AudioPlayer_OnAudioKeyPress(object? sender, MediaControlsButton button)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                switch (button)
                {
                    case MediaControlsButton.Play:
                    case MediaControlsButton.Pause:
                        AudioPlayer.PausePlay();
                        break;
                    case MediaControlsButton.Previous:
                        {
                            // 0x11:    CTRL / VK_MENU
                            // 0x12:    ALT / VK_MENU
                            float amount = ExternalInputHelper.IsKeyDown(0x12) ? (ExternalInputHelper.IsKeyDown(0x11) ? 0.01f : 0.10f) : 0.0f;

                            if (amount == 0.0f)
                            {
                                if (ExternalInputHelper.IsKeyDown(0x11))
                                {
                                    SystemVolumeChanger.DecreaseSystemVolume(0.01f);
                                    SaveVolumeToDatabase();
                                }
                                else
                                {
                                    PreviousSong();
                                }
                            }
                            else
                            {
                                AudioPlayer.SetVolume(_settings.Volume - amount);
                                SaveVolumeToDatabase();
                            }
                        }

                        break;

                    case MediaControlsButton.Next:
                        {
                            // 0x11:    CTRL / VK_MENU
                            // 0x12:    ALT / VK_MENU
                            float amount = ExternalInputHelper.IsKeyDown(0x12) ? (ExternalInputHelper.IsKeyDown(0x11) ? 0.01f : 0.10f) : 0.0f;

                            if (amount == 0.0f)
                            {
                                if (ExternalInputHelper.IsKeyDown(0x11))
                                {
                                    SystemVolumeChanger.IncreaseSystemVolume(0.01f);
                                    SaveVolumeToDatabase();
                                }
                                else
                                {
                                    NextSong();
                                }
                            }
                            else
                            {
                                AudioPlayer.SetVolume(_settings.Volume + amount);
                                SaveVolumeToDatabase();
                            }
                        }
                        break;
                }
            });
        }

        private void MediaPlayerMediaEnded(object? sender, EventArgs e)
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

        private void MediaPlayerVolumeChanged(object? sender, double volume)
        {
            // Apply volume change to settings
            Settings.Volume = volume;
        }

        #endregion MediaPlayerEvents

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

        public void OpenMedia(SongModel song, bool singleSongMode = false)
        {
            AudioPlayer.OpenMedia(song);
            if (song.Image == null)
            {
                BitmapImage? bitmap = AudioProperties.GetAlbumArt(song.Path);
                if (bitmap != null)
                {
                    song.Image = bitmap;
                }
            }

            // TODO: mode can be SingleSong
            if (PlaylistsManager.PlaylistViewing != null)
            {
                PlaylistsManager.SetPlayingPlaylist(PlaylistsManager.PlaylistViewing);

                if (singleSongMode)
                {
                    SingleSongVisibility = Visibility.Visible;
                }
            }

            // Update UI
            CurrentTime = "0:00";
            SliderValue = 0.0;
            FinalTime = HelperMethods.MsToTime(song.Duration.TotalMilliseconds);

            // Remove whenever this mess is fixed
            AudioPlayer.Play();
        }

        public void PreviousSong()
        {
            if (AudioPlayer.CurrentSong == null)
                return;

            AudioPlayer.Pause();

            // Get song Index
            if (PlaylistsManager.PlaylistPlaying == null) return;
            int index = PlaylistsManager.PlaylistPlaying.Songs.IndexOf(AudioPlayer.CurrentSong);

            // Play previous song if there is one
            if (index > 0)
            {
                OpenMedia(PlaylistsManager.PlaylistPlaying.Songs[index - 1]);
            }
        }

        public void NextSong()
        {
            if (AudioPlayer.CurrentSong == null)
                return;

            AudioPlayer.Pause();

            // Get song Index
            if (PlaylistsManager.PlaylistPlaying == null) return;
            int index = PlaylistsManager.PlaylistPlaying.Songs.IndexOf(AudioPlayer.CurrentSong);

            // Play next song if there is one
            if (index < PlaylistsManager.PlaylistPlaying.Songs.Count - 1)
            {
                OpenMedia(PlaylistsManager.PlaylistPlaying.Songs[index + 1]);
            }
            // If not, play first song
            else
            {
                if (LoopPlaylistEnabled)
                {
                    OpenMedia(PlaylistsManager.PlaylistPlaying.Songs[0]);
                }
            }
        }

        // TODO: what?
        public void AutoPlayNextSong()
        {
            AudioPlayer.Pause();

            // Generate random
            Random random = new();
            int rndIndex = random.Next(0, PlaylistsManager.PlaylistPlaying.Songs.Count);
            OpenMedia(PlaylistsManager.PlaylistPlaying.Songs[rndIndex]);
        }

        public void ShowPlaylist(int playlistId)
        {
            PlaylistModel? playlist = PlaylistsManager.GetPlaylist(playlistId);
            if (playlist == null) return;

            // Select Playlist with Id playlistId
            PlaylistsManager.SetViewingPlaylist(playlist);
            CurrentView = PlaylistVM;
        }

        #region Volume CRUD
        public async void SaveVolumeToDatabase()
        {
            await DbHelper.SetVolume(AudioPlayer.Volume, Settings.Id);
        }
        #endregion Volume CRUD

        #region Playlist CRUD
        public async void CreatePlaylist(string name = "New Playlist", string description = "")
        {
            // Update Database
            Playlist playlist = await DbHelper.CreatePlaylist(name, description);

            // Update Client
            PlaylistModel playlistModel = new(playlist);
            PlaylistsManager.Add(playlistModel);
        }

        public async void DeletePlaylist(int playlistId)
        {
            // Update Database
            await DbHelper.DeletePlaylist(playlistId);

            // Update Client
            PlaylistsManager.Remove(playlistId);
        }

        public async void UpdatePlaylist(int playlistId, PlaylistModel playlist)
        {
            // Intended: Does not Update songs releated to Playist
            using (DomainContext context = new())
            {
                Playlist? playlistToUpdate = await context.Playlists.Where(p => p.Id == playlistId).FirstOrDefaultAsync();
                PlaylistModel? playlistModelToUpdate = PlaylistsManager.Playlists.Where(p => p.Id == playlistId).FirstOrDefault();

                if (playlistToUpdate != null)
                {
                    playlistToUpdate.Name = playlist.Name;
                    playlistToUpdate.Description = playlist.Description;
                    playlistToUpdate.ImagePath = playlist.ImagePath;

                    await context.SaveChangesAsync();

                    if (playlistModelToUpdate != null)
                    {
                        playlistModelToUpdate.Name = playlist.Name;
                        playlistModelToUpdate.Description = playlist.Description;
                        playlistModelToUpdate.ImagePath = playlist.ImagePath;
                    }
                }
            }
        }

        public async void AddSongToPlaylist(SongModel song, PlaylistModel playlist)
        {
            // Update Database
            await DbHelper.AddSongToPlaylist(song.Id, playlist.Id, playlist.Songs.Count);

            // Update Client
            playlist.Songs.Add(song);
        }

        public async void RemoveSongFromPlaylist(SongModel song, PlaylistModel playlist)
        {
            // Update Database
            await DbHelper.RemoveSongFromPlaylist(song.Id, playlist.Id);

            // Update Client
            playlist.Songs.Remove(song);
        }

        public async void SwapSongInPlaylist(PlaylistModel playlist, int oldIndex, int newIndex)
        {
            // Update Database
            await DbHelper.SwapSongInPlaylist(playlist.Id, oldIndex, newIndex);

            // Update Client
            playlist.Songs.Move(oldIndex, newIndex);
        }
        #endregion Playlist CRUD

        #region Searching
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
                PlaylistsManager.SetSearchingPlaylist(new()
                {
                    Id = PlaylistsManager.PlaylistViewing.Id,
                    Name = PlaylistsManager.PlaylistViewing.Name,
                    Songs = [],
                    Description = PlaylistsManager.PlaylistViewing.Description,
                });
                ObservableCollection<SongModel> songs = [];

                for (int i = 0; i < PlaylistsManager.PlaylistViewing.Songs.Count; i++)
                {
                    if (PlaylistsManager.PlaylistViewing.Songs[i].Title.ToLower().Contains(value.ToLower()))
                    {
                        songs.Add(PlaylistsManager.PlaylistViewing.Songs[i]);
                    }
                }

                PlaylistsManager.SearchingPlaylist.Songs = songs;
                PlaylistsManager.SetViewingPlaylist(PlaylistsManager.SearchingPlaylist);
            }
            else
            {
                PlaylistsManager.SetViewingPlaylist(PlaylistsManager.Playlists.Where(x => x.Id == PlaylistsManager.PlaylistViewing.Id).FirstOrDefault());
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
                PlaylistsManager.SetSearchingPlaylist(new()
                {
                    Id = PlaylistsManager.PlaylistViewing.Id,
                    Name = PlaylistsManager.PlaylistViewing.Name,
                    Songs = [],
                    Description = PlaylistsManager.PlaylistViewing.Description,
                });
                ObservableCollection<SongModel> songs = [];

                for (int i = 0; i < PlaylistsManager.PlaylistViewing.Songs.Count; i++)
                {
                    if (PlaylistsManager.PlaylistViewing.Songs[i].Title.ToLower().Contains(value.ToLower()))
                    {
                        songs.Add(PlaylistsManager.PlaylistViewing.Songs[i]);
                    }
                }

                PlaylistsManager.SearchingPlaylist.Songs = songs;
                PlaylistsManager.SetViewingPlaylist(PlaylistsManager.SearchingPlaylist);
            }
            else
            {
                PlaylistsManager.SetViewingPlaylist(MyMusic);
            }
        }
        #endregion Searching

        public void ShowSongEditor(int songId)
        {

        }
    }
}

// used to be 1083
