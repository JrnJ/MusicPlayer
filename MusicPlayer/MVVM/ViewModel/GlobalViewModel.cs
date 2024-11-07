using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Media.Imaging;
using MusicPlayer.Classes;
using MusicPlayer.Core;
using MusicPlayer.Core.Searching;
using MusicPlayer.Database;
using MusicPlayer.DiscordGameSDK;
using MusicPlayer.MVVM.Model;
using MusicPlayer.Shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows;
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

            // Before Others
            _playlistsManager = new();

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
            };

            SettingsSongsFolder settingsSongsFolder = new()
            {
                SettingsId = Settings.Id,
                SongsFolderId = songsFolder.Id
            };

            using (DomainContext context = new())
            {
                Settings settings = context.Settings
                    .Where(s => s.Id == Settings.Id)
                    .Include(s => s.SongsFolders)
                    .FirstOrDefault();

                settings.SongsFolders.Add(settingsSongsFolder);
                songsFolder.Settings.Add(settingsSongsFolder);

                // Update Database
                context.SongsFolders.Add(songsFolder);
                await context.SaveChangesAsync();

                // Update Client
                SongsFolderModel songsFolderModel = new(songsFolder);
                SongsFolders.Add(songsFolderModel);
                Settings.SongsFolders.Add(songsFolderModel);
            }

            List<Song> folderSongs = new();

            using (DomainContext context = new())
            {
                // Db Things
                List<Artist> dbArtists = await context.Artists
                    .Include(a => a.Songs)
                    .ToListAsync();

                List<Genre> dbGenres = await context.Genres
                    .Include(g => g.Songs)
                    .ToListAsync();

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

                    // 3. Handle Artists
                    ArtistSong artistSong = new()
                    {
                        SongId = song.Id
                    };

                    foreach (Artist dbArtist in dbArtists)
                    {
                        // FK
                        if (musicProperties.Artist == dbArtist.Name)
                        {
                            //
                            artistSong.ArtistId = dbArtist.Id;
                            dbArtist.Songs ??= new List<ArtistSong>();
                            dbArtist.Songs.Add(artistSong);
                            break;
                        }

                        // Avoid duplicates
                        if (dbArtists.Where(a => a.Name == musicProperties.Artist).FirstOrDefault() != null)
                            continue;

                        Artist artist = new()
                        {
                            Name = musicProperties.Artist,
                            Songs = new List<ArtistSong>()
                        };
                        context.Artists.Add(artist);
                        dbArtists.Add(artist);

                        //
                        artistSong.ArtistId = artist.Id;
                        artist.Songs.Add(artistSong);
                        break;
                    }
                    song.Artists.Add(artistSong);

                    // 4. Handle Genres
                    foreach (string genreName in musicProperties.Genre)
                    {
                        SongGenre songGenre = new()
                        {
                            SongId = song.Id
                        };

                        foreach (Genre dbGenre in dbGenres)
                        {
                            // Junction Table
                            if (genreName == dbGenre.Name)
                            {
                                //
                                songGenre.GenreName = dbGenre.Name;
                                dbGenre.Songs ??= new List<SongGenre>();
                                dbGenre.Songs.Add(songGenre);
                                break;
                            }

                            // Avoid duplicates
                            if (dbGenres.Where(g => g.Name == genreName).FirstOrDefault() != null)
                                continue;

                            Genre genre = new() 
                            { 
                                Name = genreName,
                                Songs = new List<SongGenre>()
                            };
                            context.Genres.Add(genre);
                            dbGenres.Add(genre);

                            // 
                            songGenre.GenreName = genre.Name;
                            genre.Songs.Add(songGenre);
                            break;
                        }
                        song.Genres.Add(songGenre);
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
            _artists = [];
            foreach (Artist artist in dbArtists)
            {
                _artists.Add(new(artist));
            }

            // 3. Fill Genres
            _genres = [];
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
            foreach (Playlist playlist in dbPlaylists)
            {
                PlaylistsManager.Add(new(playlist, _myMusic.Songs));
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

            if (args.Button == SystemMediaTransportControlsButton.Next)
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

        #region Volume
        // TODO: move this
        public async void SaveVolumeToDatabase()
        {
            using (DomainContext context = new())
            {
                Settings settings = context.Settings.FirstOrDefault(s => s.Id == Settings.Id);

                // Update Database
                settings.Volume = AudioPlayer.Volume;
                await context.SaveChangesAsync();
            }
        }
        #endregion Volume

        public void OpenMedia(SongModel song, bool singleSongMode = false)
        {
            AudioPlayer.OpenMedia(song);

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
            if (PlaylistsManager.PlaylistViewing != null)
            {
                PlaylistsManager.PlaylistViewing.IsSelected = false;
            }

            PlaylistsManager.SetViewingPlaylist(PlaylistsManager.Playlists.FirstOrDefault(x => x.Id == playlistId));
            PlaylistsManager.PlaylistViewing.IsSelected = true;
            CurrentView = PlaylistVM;
        }

        public PlaylistModel CreatePlaylist(string name = "New Playlist", string description = "")
        {
            Playlist playlist = new()
            {
                Name = name,
                Description = description,
                ImagePath = "",
                Songs = [],
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
            PlaylistsManager.Add(playlistModel);

            return playlistModel;
        }

        public void DeletePlaylist(int playlistId)
        {
            using (DomainContext context = new())
            {
                Playlist? playlistToDelete = context.Playlists.FirstOrDefault(p => p.Id == playlistId);

                if (playlistToDelete != null)
                {
                    // Delete from Database
                    context.Playlists.Remove(playlistToDelete);
                    context.SaveChanges();

                    // Delete from Memory
                    PlaylistsManager.Remove(playlistId);
                }
            }
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
            using (DomainContext context = new())
            {
                // Only add if it doesnt exist
                if (await context.PlaylistSongs.AnyAsync(ps => ps.SongId == song.Id && ps.PlaylistId == playlist.Id)) return;

                PlaylistSong playlistSong = new()
                {
                    SongId = song.Id,
                    PlaylistId = playlist.Id,
                    Index = playlist.Songs.Count
                };

                context.PlaylistSongs.Add(playlistSong);
                await context.SaveChangesAsync();
            }

            // Update Client
            playlist.Songs.Add(song);
        }

        public async void RemoveSongFromPlaylist(SongModel song, PlaylistModel playlist)
        {
            // Update Database
            using (DomainContext context = new())
            {
                PlaylistSong? newPlaylistSong = await context.PlaylistSongs
                    .Where(ps => ps.PlaylistId == playlist.Id && ps.SongId == song.Id)
                    .FirstOrDefaultAsync();

                if (newPlaylistSong == null) return;

                context.PlaylistSongs.Remove(newPlaylistSong);

                // Update PlaylistSong Indexes
                List<PlaylistSong> playlistSongs = await context.PlaylistSongs
                    .Where(ps => ps.PlaylistId == playlist.Id)
                    .ToListAsync();

                foreach (PlaylistSong playlistSong in playlistSongs)
                {
                    if (playlistSong.Index > newPlaylistSong.Index)
                    {
                        // If we add a delete many we can decrease more here
                        // If we add a delete random selection this wont work
                        playlistSong.Index -= 1;
                    }
                }

                await context.SaveChangesAsync();
            }

            // Update Client
            playlist.Songs.Remove(song);
        }

        public void SwapSongInPlaylistClient(PlaylistModel playlist, int oldIndex, int newIndex)
        {
            playlist.Songs.Move(oldIndex, newIndex);
        }

        public async void SwapSongInPlaylistDatabase(PlaylistModel playlist, int oldIndex, int newIndex)
        {
            // Update Database
            using (DomainContext context = new())
            {
                SongModel songDragging = playlist.Songs[oldIndex];
                SongModel songToSwapWith = playlist.Songs[newIndex];

                context.PlaylistSongs
                    .Where(ps => ps.SongId == songDragging.Id)
                    .Where(ps => ps.PlaylistId == playlist.Id)
                    .FirstOrDefault().Index = oldIndex;

                context.PlaylistSongs
                    .Where(ps => ps.SongId == songToSwapWith.Id)
                    .Where(ps => ps.PlaylistId == playlist.Id)
                    .FirstOrDefault().Index = newIndex;

                await context.SaveChangesAsync();
            }
            
            // Update Client
            //SwapSongInPlaylistClient(playlist, oldIndex, newIndex);
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
    }
}
