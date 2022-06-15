﻿using MusicPlayer.Classes;
using MusicPlayer.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MusicPlayer.MVVM.ViewModel
{
    internal class GlobalViewModel : ObservableObject
    {
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

        // MusicPlayer
        private AudioPlayer _audioPlayer;

        public AudioPlayer AudioPlayer
        {
            get { return _audioPlayer; }
            set { _audioPlayer = value; }
        }

        private ObservableCollection<Playlist> _playlists;

        public ObservableCollection<Playlist> Playlists
        {
            get => _playlists;
            set { _playlists = value; OnPropertyChanged(); }
        }

        private Playlist _selectedPlaylist;

        public Playlist SelectedPlaylist
        {
            get => _selectedPlaylist;
            set { _selectedPlaylist = value; OnPropertyChanged(); }
        }

        private Playlist _myMusic;

        public Playlist MyMusic
        {
            get { return _myMusic; }
            set { _myMusic = value; OnPropertyChanged(); }
        }

        private Song _currentSong;

        public Song CurrentSong
        {
            get { return _currentSong; }
            set { _currentSong = value; OnPropertyChanged(); }
        }

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

        public int CurrentSongIndex { get; set; }

        // Commands
        public RelayCommand SelectPlaylistCommand { get; set; }

        public GlobalViewModel()
        {
            // Create ViewModels
            PlaylistVM = new();

            // Load Playlists
            Playlists = FileHandler.GetPlaylists();

            // Assign Commands
            SelectPlaylistCommand = new(o =>
            {
                ShowPlaylist((int)o);
            });

            // Configuration
            ConfigureAudioPlayer();
        }

        #region Configuration
        private void ConfigureAudioPlayer()
        {
            // AudioPlayer
            AudioPlayer = new AudioPlayer();
            AudioPlayer.MediaPlayer.MediaEnded += MediaPlayerMediaEnded;

            // Timer Tick Event
            AudioPlayer.Timer.Tick += Timer_Tick;
        }

        private void MediaPlayerMediaEnded(Windows.Media.Playback.MediaPlayer sender, object args)
        {
            // Call to MainWindow thread
            Dispatcher.Invoke(() =>
            {
                AudioPlayer.Pause();

                if (SelectedPlaylist.Songs.Count - 1 > CurrentSongIndex)
                {
                    CurrentSongIndex += 1;
                    CurrentSong = SelectedPlaylist.Songs[CurrentSongIndex];
                    OpenMedia(CurrentSong);
                    AudioPlayer.Play();
                }
            });
        }

        private void OpenMedia(Song song)
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
            sliderSongTimePlayed.Value = 0;


            // Update UI
            CurrentTime = "0:00";
            FinalTime = HelperMethods.MsToTime(song.Length);

            // Remove whenever this mess is fixed
            AudioPlayer.Play();
        }
        #endregion Configuration

        public void ShowPlaylist(int id)
        {
            SelectedPlaylist = Playlists[id];

            // Maybe do this whenever SelectedPlaylist is changed, might f up if a different view is made though
            CurrentView = PlaylistVM;
        }

        public void DeletePlaylist(int id)
        {
            Playlists.Remove(Playlists.Where(x => x.Id == id).First());
            FileHandler.SavePlaylists(Playlists);

            // load home view if playloist view with id x is active
        }
    }
}
