using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MusicPlayer.Classes;
using MusicPlayer.Core;

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

        public RelayCommand SettingsViewCommand { get; set; }
        #endregion Navigation

        public RelayCommand CreatePlaylistCommand { get; set; }

        // Control Commands
        public RelayCommand PausePlayCommand { get; set; }

        public RelayCommand PreviousSongCommand { get; set; }

        public RelayCommand NextSongCommand { get; set; }

        public RelayCommand ShuffleCommand { get; set; }

        public RelayCommand RepeatCommand { get; set; }

        // ViewModels
        public HomeViewModel HomeVM { get; set; }

        public DiscordViewModel DiscordVM { get; set; }

        public SettingsViewModel SettingsVM { get; set; }

        // Constructor
        public MainViewModel()
        {
            // Create ViewModels
            HomeVM = new();
            DiscordVM = new();
            SettingsVM = new();
            
            // Set a default
            Global.CurrentView = HomeVM;

            // Assign Commands
            HomeViewCommand = new(o =>
            {
                Global.CurrentView = HomeVM;
            });

            PlaylistsViewCommand = new(o =>
            {
                Global.CurrentView = Global.PlaylistsVM;
            });

            DiscordViewCommand = new(o => 
            {
                Global.CurrentView = DiscordVM;
            });

            SettingsViewCommand = new(o =>
            {
                Global.CurrentView = SettingsVM;
            });

            CreatePlaylistCommand = new(o =>
            {
                // Create sample template to add a new playlist
                Playlist playlist = new(Global.Playlists.Count, new ObservableCollection<Song>(), "New Playlist", "New Playlist");

                Global.Playlists.Add(playlist);
                Global.ShowPlaylist(playlist.Id);

                // Save Playlists
                FileHandler.SavePlaylists(Global.Playlists);
            });

            #region ManagerCommands
            PausePlayCommand = new(o =>
            {
                if (Global.AudioPlayer.IsPlaying)
                {
                    Global.AudioPlayer.Pause();
                }
                else
                {
                    Global.AudioPlayer.Play();
                }
            });

            PreviousSongCommand = new(o =>
            {
                
            });

            NextSongCommand = new(o =>
            {
                
            });

            ShuffleCommand = new(o =>
            {

            });

            RepeatCommand = new(o =>
            {

            });
            #endregion ManagerCommands

            // Configuration
            ConfigureSettings();
        }

        private void ConfigureSettings()
        {
            // Load Settings
            AppSettings.GetSettingsFromFile();
            Global.AudioPlayer.Volume = AppSettings.Volume;
            Global.MyMusic = new Playlist(0, new ObservableCollection<Song>(), "My Music", "All music from all folders.");

            // Create a playlist of all songs
            for (int i = 0; i < AppSettings.MusicFolders.Count; i++)
            {
                List<Song> songs = FileHandler.GetSongsFromFolder(AppSettings.MusicFolders[i]);

                if (songs != null)
                {
                    //MyMusic.Songs.AddRange(songs);
                    for (int i2 = 0; i2 < songs.Count; i2++)
                    {
                        Song song = songs[i2];
                        song.SetId(Global.MyMusic.Songs.Count);

                        Global.MyMusic.Songs.Add(song);
                    }
                }
            }
        }
    }
}
