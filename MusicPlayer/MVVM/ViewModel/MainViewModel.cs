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

        private MultiImageButton _playPauseButton;

        public MultiImageButton PlayPauseButton
        {
            get { return _playPauseButton; }
            set { _playPauseButton = value; }
        }

        // Constructor
        public MainViewModel()
        {
            //
            _playPauseButton = new(
            [
                new("play", "/Images/Icons/play_icon.png"),
                new("pause", "/Images/Icons/pause_icon.png")
            ]);

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
                Global.PlaylistsManager.PlaylistViewing = Global.MyMusic;
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
            Global.AudioPlayer.OnStateChanged += (object? sender, AudioPlayerState state) =>
            {
                switch (state)
                {
                    case AudioPlayerState.Playing:
                        PlayPauseButton.Alter("play");
                        break;

                    default:
                        PlayPauseButton.Alter("pause");
                        break;
                }
            };

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
        }
    }
}
