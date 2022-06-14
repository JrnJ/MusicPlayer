using MusicPlayer.Classes;
using MusicPlayer.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public PlaylistsViewModel PlaylistVM { get; set; }


        private ObservableCollection<Playlist> _playlists;

        public ObservableCollection<Playlist> Playlists
        {
            get { return _playlists; }
            set { _playlists = value; OnPropertyChanged(); }
        }

        private Playlist _selectedPlaylist;

        public Playlist SelectedPlaylist
        {
            get { return _selectedPlaylist; }
            set { _selectedPlaylist = value; OnPropertyChanged(); }
        }

        // Commands???
        public RelayCommand PlaylistViewCommand { get; set; }

        public GlobalViewModel()
        {
            // Create ViewModels
            PlaylistVM = new();

            // Load Playlists
            Playlists = FileHandler.GetPlaylists();

            // Assign Commands
            PlaylistViewCommand = new(o =>
            {
                SelectedPlaylist = Playlists[1];
                CurrentView = PlaylistVM;
            });
        }
    }
}
