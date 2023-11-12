using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Media;
using MusicPlayer.Classes;
using MusicPlayer.Core;
using MusicPlayer.MVVM.Model;
using MusicPlayer.Shared.Models;

namespace MusicPlayer.MVVM.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        // <GlobalViewModel> //
        public GlobalViewModel Global { get; } = GlobalViewModel.Instance;
        // </GlobalViewModel> //

        // Commands
        #region Navigation
        public RelayCommand HomeViewCommand { get; set; }

        public RelayCommand PlaylistsViewCommand { get; set; }

        public RelayCommand DiscordViewCommand { get; set; }

        public RelayCommand SpotifyViewCommand { get; set; }

        public RelayCommand SettingsViewCommand { get; set; }
        #endregion Navigation

        public RelayCommand CreatePlaylistCommand { get; set; }

        public RelayCommand SelectPlaylistCommand { get; set; }

        // Control Commands
        public RelayCommand PausePlayCommand { get; set; }

        public RelayCommand PreviousSongCommand { get; set; }

        public RelayCommand NextSongCommand { get; set; }

        public RelayCommand ShuffleCommand { get; set; }

        public RelayCommand RepeatCommand { get; set; }

        // Other Commands
        public RelayCommand ShowSongCommand { get; set; }

        public RelayCommand SelectPlaylistImageCommand { get; set; }

        // ViewModels
        public HomeViewModel HomeVM { get; set; }

        public DiscordViewModel DiscordVM { get; set; }

        public SpotifyViewModel SpotifyVM { get; set; }

        public SettingsViewModel SettingsVM { get; set; }

        // Properties
        private bool _clickedInSliderr;

        public bool ClickedInSliderr
        {
            get => _clickedInSliderr; 
            set 
            { 
                _clickedInSliderr = value;
                Global.SliderMouseDownOrUpEvent(value);
                OnPropertyChanged();
            }
        }

        // Constructor
        public MainViewModel()
        {
            // Create ViewModels
            HomeVM = new();
            DiscordVM = new();
            SpotifyVM = new();
            SettingsVM = new();
            
            // Set a default
            Global.CurrentView = HomeVM;

            // Assign Commands
            HomeViewCommand = new(o =>
            {
                Global.CurrentView = HomeVM;
                Global.PlaylistViewing = Global.MyMusic;
            });

            PlaylistsViewCommand = new(o =>
            {
                Global.CurrentView = Global.PlaylistsVM;
            });

            DiscordViewCommand = new(o => 
            {
                Global.CurrentView = DiscordVM;
            });

            SpotifyViewCommand = new(o =>
            {
                Global.CurrentView = SpotifyVM;
            });

            SettingsViewCommand = new(o =>
            {
                Global.CurrentView = SettingsVM;
            });

            CreatePlaylistCommand = new(o =>
            {
                Global.CreatePlaylist();
            });

            SelectPlaylistCommand = new(o =>
            {
                Global.ShowPlaylist((int)o);
            });

            #region ManagerCommands
            PausePlayCommand = new(o =>
            {
                Global.AudioPlayer.PausePlay();
            });

            PreviousSongCommand = new(o =>
            {
                Global.PreviousSong();
            });

            NextSongCommand = new(o =>
            {
                Global.NextSong();
            });

            ShuffleCommand = new(o =>
            {

            });

            RepeatCommand = new(o =>
            {
                Global.AudioPlayer.IsLoopingEnabled = !Global.AudioPlayer.IsLoopingEnabled;
            });

            ShowSongCommand = new(o =>
            {
                if (Global.SingleSongVisibility == System.Windows.Visibility.Collapsed)
                {
                    Global.SingleSongVisibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    Global.SingleSongVisibility = System.Windows.Visibility.Collapsed;
                }
            });
            #endregion ManagerCommands

            // TODO:
            // remove test data
            Artist artist = new()
            {
                Name = "Jeroen",
                Songs = new List<ArtistSong>()
            };

            Genre genre = new()
            {
                Name = "Jeroenium",
                Songs = new List<SongGenre>()
            };

            Playlist playlist = new()
            {
                Name = "Jeroen's Playlist",
                Description = "Playlist made by Jeroen",
                ImagePath = "D:\\Music\\Images\\K-ON.png",
                Songs = new List<PlaylistSong>()
            };

            Song song = new()
            {
                Path = "D:\\Music\\Feelgood\\Earth Wind  Fire - September.mp3",
                Title = "Jeroen's Song!",
                Year = 2002,
                Duration = new TimeSpan(0, 3, 21),
                Artists = new List<ArtistSong>(),
                Playlists = new List<PlaylistSong>(),
                Genres = new List<SongGenre>()
            };

            SongGenre songGenre = new()
            {
                SongId = song.Id,
                GenreName = genre.Name
            };
            song.Genres.Add(songGenre);
            genre.Songs.Add(songGenre);

            ArtistSong artistSong = new()
            {
                ArtistId = artist.Id,
                SongId = song.Id,
            };
            artist.Songs.Add(artistSong);
            song.Artists.Add(artistSong);

            PlaylistSong playlistSong = new()
            {
                PlaylistId = playlist.Id,
                SongId = song.Id
            };
            playlist.Songs.Add(playlistSong);
            song.Playlists.Add(playlistSong);

            // Settings
            Settings settings = new()
            {
                Volume = 0.33,
                SongsFolders = new List<SettingsSongsFolder>()
            };

            SongsFolder songsFolder = new()
            {
                Path = "D:\\Music\\Feelgood",
                Settings = new List<SettingsSongsFolder>()
            };

            SettingsSongsFolder settingsSongsFolder = new()
            {
                SettingsId = settings.Id,
                SongsFolderId = songsFolder.Id,
            };
            settings.SongsFolders.Add(settingsSongsFolder);
            songsFolder.Settings.Add(settingsSongsFolder);

            using (DomainContext context = new())
            {
                // Remove if already exists
                if (context.Songs.ToList().Count > 0) return; 

                context.Artists.Add(artist);
                context.Genres.Add(genre);
                context.Songs.Add(song);
                context.Playlists.Add(playlist);

                context.Settings.Add(settings);
                context.SongsFolders.Add(songsFolder);

                context.SaveChanges();
            }
        }
    }
}
