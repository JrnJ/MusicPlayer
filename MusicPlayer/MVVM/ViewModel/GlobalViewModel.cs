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
        public PlaylistViewModel PlaylistVM { get; set; }

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
        }

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
