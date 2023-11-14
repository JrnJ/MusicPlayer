using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Media.Imaging;
using MusicPlayer.Classes;
using MusicPlayer.Core;
using MusicPlayer.Core.Searching;
using MusicPlayer.DiscordGameSDK;
using MusicPlayer.MVVM.Model;
using MusicPlayer.Shared.Models;
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

using Playlist = MusicPlayer.Shared.Models.Playlist;

namespace MusicPlayer.MVVM.ViewModel
{
    internal class GlobalViewModel : ObservableObject
    {
        #region Properties
        public static GlobalViewModel Instance { get; } = new();
        // Globals

        // Titlebar
        private AppWindowTitlebarManager _appWindowTitlebarManager;

        public AppWindowTitlebarManager AppWindowTitlebarManager
        {
            get { return _appWindowTitlebarManager; }
            set { _appWindowTitlebarManager = value; }
        }

        // Current View
        private object _currentView;

        public object CurrentView
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

        private ObservableCollection<SongsFolderModel> _songsFolders;

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
        private ObservableCollection<ArtistModel> _artists;

        public ObservableCollection<ArtistModel> Artists
        {
            get { return _artists; }
            set { _artists = value; OnPropertyChanged(); }
        }

        private ObservableCollection<GenreModel> _genres;

        public ObservableCollection<GenreModel> Genres
        {
            get { return _genres; }
            set { _genres = value; OnPropertyChanged(); }
        }

        // Playlists
        private PlaylistModel _myMusic;

        public PlaylistModel MyMusic
        {
            get => _myMusic;
            set { _myMusic = value; OnPropertyChanged(); }
        }

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
        #endregion Properties

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

        public GlobalViewModel()
        {
            // AppTitlebar
            _appWindowTitlebarManager = new();

            // Create ViewModels
            PlaylistsVM = new();
            PlaylistVM = new();

            // Search Bar
            _globalSearch = new();

            // Configuration
            NewConfigure();
            // Configure();
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

        #region SortThis
        // TODO2:
        public async void AddSongsFolder(StorageFolder folder)
        {
            SongsFolder songsFolder = new()
            {
                Path = folder.Path,
                // TODO2: we also need to add the link to the Settings
                // and possibly the other way around too
                Settings = new List<SettingsSongsFolder>()
                {

                }
            };

            using (DomainContext context = new())
            {
                // Update Database
                context.SongsFolders.Add(songsFolder);
                await context.SaveChangesAsync();

                // Update Client
                SongsFolderModel songsFolderModel = new(songsFolder);
                SongsFolders.Add(songsFolderModel);
                Settings.SongsFolders.Add(songsFolderModel);
            }

            List<Song> folderSongs = new();
            List<Artist> artists = new();
            List<Genre> genres = new();
            IReadOnlyList<StorageFile> files = await folder.GetFilesAsync();
            foreach (StorageFile file in files)
            {
                // 1. Is file music
                if (!HelperMethods.IsMusicFile(file.Path))
                    continue;

                // 2. Set Song data
                MusicProperties musicProperties = await file.Properties.GetMusicPropertiesAsync();
                Song song = new()
                {
                    Path = file.Path,
                    Title = musicProperties.Title,
                    Year = unchecked((int)musicProperties.Year),
                    Duration = musicProperties.Duration,
                    SongsFolderId = songsFolder.Id, 

                    Artists = new List<ArtistSong>(),
                    Genres = new List<SongGenre>(),
                    Playlists = new List<PlaylistSong>()
                };
                folderSongs.Add(song);

                // 3. Handle Artists
                // TODO2
                Artist artist = new()
                {
                    Name = musicProperties.Artist
                };
                artists.Add(artist);

                // 4. Handle Genres
                // TODO2
                foreach (string genre in musicProperties.Genre)
                {
                    genres.Add(new Genre() { Name = genre });
                }
            }

            // Update Songs to Database
            using (DomainContext context = new())
            {
                // 1. Set Artists


                // 2. Set Genres


                // 3. Set Songs
                foreach (Song folderSong in folderSongs)
                {
                    context.Songs.Add(folderSong);
                }
                await context.SaveChangesAsync();

                // 4. Set ArtistSongs


                // 5. Set SongGenres


                await context.SaveChangesAsync();
            }

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

                
            }

            // Update Client


        }
        #endregion SortThis

        #region NewConfiguration
        public async void NewConfigure()
        {
            using (DomainContext context = new())
            {
                LoadSettings(context);
                LoadCache(context);
            }

            ConfigureAudioPlayer();
            AudioPlayer.Volume = Settings.Volume;
            // TODO3
            // Method for reading all files to check if they still exist
            // and to check if the data is still correct
            // only do this for Songs that are on screen
            // if a song is clicked and the path is not found
            // you can for now throw an error

            PopupVisibility = Visibility.Collapsed;
            SingleSongVisibility = Visibility.Collapsed;
            PlaylistViewing = MyMusic;
        }

        public async void LoadCache(DomainContext context)
        {
            // 1. Read from Database
            // 1.1 Read Artists
            List<Artist> dbArtists = await context.Artists.ToListAsync();

            // 1.2 Read Genres
            List<Genre> dbGenres = await context.Genres.ToListAsync();

            // 1.3 Read Songs
            List<Song> dbSongs = await context.Songs
                .Include(s => s.Artists)
                .ThenInclude(a => a.Artist)
                .Include(g => g.Genres)
                .ToListAsync();

            // 1.4 Read Playlists
            List<Playlist> dbPlaylists = await context.Playlists
                .Include(p => p.Songs)
                .ToListAsync();

            // 2. Fill Artists
            _artists = new();
            foreach (Artist artist in dbArtists)
            {
                _artists.Add(new(artist));
            }

            // 3. Fill Genres
            _genres = new();
            foreach (Genre genre in dbGenres)
            {
                _genres.Add(new(genre));
            }

            // 4. Fill Songs
            _myMusic = new();
            foreach (Song song in dbSongs)
            {
                _myMusic.Songs.Add(new(song, _artists, _genres));
            }

            // 5. Fill Playlists
            _playlists = new();
            foreach (Playlist playlist in dbPlaylists)
            {
                _playlists.Add(new(playlist, _myMusic.Songs));
            }
        }

        public async void LoadSettings(DomainContext context)
        {
            // 1. Read from Database
            List<Settings> dbSettings = await context.Settings
                .Include(s => s.SongsFolders)
                .ToListAsync();
            List<SongsFolder> dbSongsFolders = await context.SongsFolders
                .ToListAsync();

            // 2. Fill Models
            _songsFolders = new();
            foreach (SongsFolder songsFolder in dbSongsFolders)
            {
                _songsFolders.Add(new(songsFolder));
            }

            _settings = new(dbSettings[0], SongsFolders);
        }
        #endregion NewConfiguration

        #region Configuration
        //public async void Configure()
        //{
        //    // Validate
        //    await Validate();

        //    // Configure
        //    //await ConfigureSettings();
        //    ConfigureMusic();

        //    ConfigureAudioPlayer();
        //    PopupVisibility = Visibility.Collapsed;
        //    SingleSongVisibility = Visibility.Collapsed;
        //    PlaylistViewing = MyMusic;

        //    // CLA
        //    string[] cmdLine = Environment.GetCommandLineArgs();
        //    if (cmdLine.Length > 1)
        //    {
        //        AlbumSongModel song = new()
        //        {
        //            Id = 0,
        //            Path = cmdLine[1]
        //        };
        //        StorageFile storageFile = await StorageFile.GetFileFromPathAsync(cmdLine[1]);
        //        await song.Init(storageFile);

        //        OpenMedia(song, true);
        //    }
        //}

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

        //private async Task ConfigureSettings()
        //{
        //    // Load Settings
        //    AppSettinggs = new();
        //    await AppSettinggs.GetSettingsFromFile();
        //}

        private async void ConfigureMusic()
        {
            //// Create Home Playlist
            //MyMusic = new();

            //// Create Temporary collections
            //List<StorageFile> storageFiles = new();
            //List<StorageFile> nonCachedSongs = new();

            //// Load Cached Music
            //ObservableCollection<CachedSong> cachedSongs = await FileHandler<ObservableCollection<CachedSong>>.GetJSON(CachedSongsFilePath);
            //cachedSongs ??= new();

            //AppSettinggs.CachedSongs = cachedSongs;

            //// Load UI
            //// Get All Files in All Folders
            //for (int i = 0; i < AppSettinggs.MusicFolders.Count; i++)
            //{
            //    StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(AppSettinggs.MusicFolders[i].Path);
            //    IReadOnlyList<StorageFile> files = await folder.GetFilesAsync();
            //    ObservableCollection<StorageFile> musicFiles = new();
            //    foreach (StorageFile file in files)
            //    {
            //        if (HelperMethods.IsMusicFile(file.Path))
            //        {
            //            musicFiles.Add(file);
            //        }
            //    }

            //    // Loop over Files in Folder
            //    //foreach (StorageFile file in files)
            //    //{
            //    //    // Check if File is Music
            //    //    if (HelperMethods.IsMusicFile(file.Path))
            //    //    {
            //    //        bool isFileInCache = false;

            //    //        // Check if File exists in the cache
            //    //        for (int j = 0; j < cachedSongs.Count; j++)
            //    //        {
            //    //            if (cachedSongs[j].Path == file.Path)
            //    //            {
            //    //                // Add to MyMusic
            //    //                storageFiles.Add(file);
            //    //                MyMusic.AddSong(file);

            //    //                // Remove and Break
            //    //                cachedSongs.RemoveAt(j);
            //    //                isFileInCache = true;
            //    //                j--;
            //    //                break;
            //    //            }
            //    //        }

            //    //        if (!isFileInCache)
            //    //        {
            //    //            // Do Something
            //    //            nonCachedSongs.Add(file);
            //    //        }
            //    //    }
            //    //}

            //    // Loop over Songs in Cache
            //    for (int j = 0; j < cachedSongs.Count; j++)
            //    {
            //        // Check if File in Cache exists in Folder
            //        for (int k = 0; k < musicFiles.Count; k++)
            //        {
            //            if (cachedSongs[j].Path == musicFiles[k].Path)
            //            {
            //                storageFiles.Add(musicFiles[k]);
            //                MyMusic.AddSong(musicFiles[k], cachedSongs[j].Id);

            //                // Remove and break
            //                musicFiles.RemoveAt(k);
            //                k--;
            //                break;
            //            }
            //        }
            //    }

            //    // Add remaining files to nonCachedSongs
            //    for (int j = 0; j < musicFiles.Count; j++)
            //    {
            //        nonCachedSongs.Add(musicFiles[j]);
            //    }
            //}

            // Load Playlists
            //GetPlaylists();

            //await ConfigureFullSongs(nonCachedSongs);

            //await CreateNewCache();
        }

        //private async Task ConfigureFullSongs(List<StorageFile> nonCachedSongs)
        //{
        //    // Load Full Songs
        //    for (int i = 0; i < MyMusic.Songs.Count; i++)
        //    {
        //        await MyMusic.Songs[i].Init(await StorageFile.GetFileFromPathAsync(MyMusic.Songs[i].Path));
        //    }

        //    // Load Non Cached Songs
        //    for (int i = 0; i < nonCachedSongs.Count; i++)
        //    {
        //        await MyMusic.AddSong(nonCachedSongs[i]).Init(await StorageFile.GetFileFromPathAsync(nonCachedSongs[i].Path));
        //    }
        //}

        //private async Task CreateNewCache()
        //{
        //    // Create new Cache
        //    ObservableCollection<CachedSong> newCache = new();
        //    for (int i = 0; i < MyMusic.Songs.Count; i++)
        //    {
        //        newCache.Add(new()
        //        {
        //            Id = MyMusic.Songs[i].Id,
        //            Path = MyMusic.Songs[i].Path,
        //            Title = MyMusic.Songs[i].MusicProperties.Title,
        //            Artist = MyMusic.Songs[i].MusicProperties.Artist
        //        });
        //    }

        //    // Save new cache if something changed
        //    if (newCache != AppSettinggs.CachedSongs)
        //        await FileHandler<ObservableCollection<CachedSong>>.SaveJSON(CachedSongsFilePath, newCache);
        //}

        //public async void GetPlaylists()
        //{
        //    Playlists = new();

        //    // Get all Playlists and make new if none exists
        //    ObservableCollection<PlaylistSongsModel> playlists = await FileHandler<ObservableCollection<PlaylistSongsModel>>.GetJSON(PlaylistsFilePath);
        //    playlists ??= new();

        //    for (int i = 0; i < playlists.Count; i++)
        //    {
        //        // Add Songs
        //        for (int j = 0; j < playlists[i].Songs.Count; j++)
        //        {
        //            if (MyMusic.Songs.FirstOrDefault(x => x.Id == playlists[i].Songs[j].Id) != null)
        //            {
        //                playlists[i].Songs[j] = MyMusic.Songs.FirstOrDefault(x => x.Id == playlists[i].Songs[j].Id);
        //            }
        //            else
        //            {
        //                playlists[i].Songs.RemoveAt(j);
        //                j--;
        //            }
        //        }

        //        // Add Playlist
        //        Playlists.Add(playlists[i]);
        //    }
        //}

        private void ConfigureAudioPlayer()
        {
            // AudioPlayer
            AudioPlayer = new()
            {
                // Volume = AppSettinggs.Volume
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
                    AudioPlayer.SetVolume(_settings.Volume - amount);
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
                    AudioPlayer.SetVolume(_settings.Volume + amount);
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
            Settings.Volume = sender.Volume;
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

        // TODO2
        public PlaylistModel CreatePlaylist(string name = "New Playlist", string description = "")
        {
            //// Create an id 
            //int id = 0;
            //for (int i = 0; i < Playlists.Count; i++)
            //{
            //    if (Playlists[i].Id > id)
            //        id = Playlists[i].Id;
            //}

            //PlaylistModel playlist = new() { Id = id + 1 };
            ////Playlists.Add(playlist);

            //SavePlaylists();

            //return playlist;

            Playlist playlist = new()
            {
                Name = name,
                Description = description,
                ImagePath = "",
                Songs = new List<PlaylistSong>(),
                // Id = ... SaveChanges adds this
            };

            using (DomainContext context = new())
            {
                // Add to Database
                context.Playlists.Add(playlist);
                context.SaveChangesAsync();
            }

            // Add to Memory
            PlaylistModel playlistModel = new(playlist, MyMusic.Songs);
            Playlists.Add(playlistModel);

            return playlistModel;
        }

        // TODO2
        public void DeletePlaylist(int playlistId)
        {
            using (DomainContext context = new())
            {
                Playlist playlistToDelete = context.Playlists.Where(p => p.Id == playlistId).FirstOrDefault();

                if (playlistToDelete != null)
                {
                    // Delete from Database
                    context.Playlists.Remove(playlistToDelete);
                    context.SaveChanges();

                    // Delete from Memory
                    PlaylistModel playlistModelToDelete = Playlists.Where(x => x.Id == playlistId).FirstOrDefault();
                    if (playlistModelToDelete != null)
                        Playlists.Remove(playlistModelToDelete);
                }
            }
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

        // TODO2
        public async void UpdatePlaylist(int playlistId, PlaylistModel playlist)
        {
            // Does not Update songs releated
            using (DomainContext context = new())
            {
                Playlist playlistToUpdate = await context.Playlists.Where(p => p.Id == playlistId).FirstOrDefaultAsync();
                PlaylistModel playlistModelToUpdate = Playlists.Where(p => p.Id == playlistId).FirstOrDefault();

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

        // TODO2
        public async void AddSongToPlaylist(SongModel song, PlaylistModel playlist)
        {
            // Update Database
            using (DomainContext context = new())
            {
                PlaylistSong playlistSong = new()
                {
                    SongId = song.Id,
                    PlaylistId = playlist.Id,
                };

                context.PlaylistSongs.Add(playlistSong);
                await context.SaveChangesAsync();
            }

            // Update Client
            playlist.Songs.Add(song);
        }

        // TODO2
        public async void RemoveSongFromPlaylist(SongModel song, PlaylistModel playlist)
        {
            // Update Database
            using (DomainContext context = new())
            {
                PlaylistSong playlistSong = await context.PlaylistSongs
                    .Where(p => p.PlaylistId == playlist.Id)
                    .Where(s => s.SongId == song.Id)
                    .FirstOrDefaultAsync();

                if (playlistSong == null) return;

                context.PlaylistSongs.Remove(playlistSong);
                
                await context.SaveChangesAsync();
            }

            // Update Client
            playlist.Songs.Remove(song);
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
                ObservableCollection<SongModel> songs = new();

                for (int i = 0; i < PlaylistViewing.Songs.Count; i++)
                {
                    if (PlaylistViewing.Songs[i].Title.ToLower().Contains(value.ToLower()))
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
                ObservableCollection<SongModel> songs = new();

                for (int i = 0; i < PlaylistViewing.Songs.Count; i++)
                {
                    if (PlaylistViewing.Songs[i].Title.ToLower().Contains(value.ToLower()))
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
