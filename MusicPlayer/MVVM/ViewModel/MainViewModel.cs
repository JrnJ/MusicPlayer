using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MusicPlayer.Classes;
using MusicPlayer.Core;

namespace MusicPlayer.MVVM.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        // Commands
        #region Navigation
        public RelayCommand HomeViewCommand { get; set; }

        public RelayCommand PlaylistsViewCommand { get; set; }
        #endregion Navigation
        public RelayCommand CreatePlaylistCommand { get; set; }

        public RelayCommand PausePlayCommand { get; set; }

        // ViewModels
        public HomeViewModel HomeVM { get; set; }

        public PlaylistsViewModel PlaylistsVM { get; set; }

        // rather have this here somehow
        //// Current View
        //private object _currentView;

        //public object CurrentView
        //{
        //    get { return _currentView; }
        //    set { _currentView = value; OnPropertyChanged(); }
        //}

        // <GlobalViewModel> //
        public GlobalViewModel Global { get; } = GlobalViewModel.Instance;
        // </GlobalViewModel> //

        // Constructor
        public MainViewModel()
        {
            // Create ViewModels
            HomeVM = new();
            PlaylistsVM = new();
            
            // Set a default
            Global.CurrentView = HomeVM;

            // Assign Commands
            HomeViewCommand = new(o =>
            {
                Global.CurrentView = HomeVM;
            });

            PlaylistsViewCommand = new(o =>
            {
                Global.CurrentView = PlaylistsVM;
            });

            CreatePlaylistCommand = new(o =>
            {
                // Create sample template to add a new playlist
                Playlist playlist = new Playlist(Global.Playlists.Count, new ObservableCollection<Song>(), "New Playlist", "New Playlist");

                Global.Playlists.Add(playlist);
                Global.ShowPlaylist(playlist.Id);

                // Save Playlists
                FileHandler.SavePlaylists(Global.Playlists);
            });

            PausePlayCommand = new(o =>
            {
                if (Global.AudioPlayer.IsPlaying)
                    Global.AudioPlayer.Pause();
                else
                    Global.AudioPlayer.Play();
            });

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
                        Global.MyMusic.Songs.Add(songs[i2]);
                    }
                }
            }
        }
    }
}
