using System;
using MusicPlayer.Core;

namespace MusicPlayer.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        // Commands
        public RelayCommand HomeViewCommand { get; set; }

        public RelayCommand PlaylistViewCommand { get; set; }

        // ViewModels
        public HomeViewModel HomeVM { get; set; }

        public PlaylistViewModel PlaylistVM { get; set; }

        // Current View
        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        // Constructor
        public MainViewModel()
        {
            // Create ViewModels
            HomeVM = new HomeViewModel();
            PlaylistVM = new PlaylistViewModel();
            
            // Set a default
            CurrentView = HomeVM;

            // Assign Commands
            HomeViewCommand = new RelayCommand(o =>
            {
                CurrentView = HomeVM;
            });

            PlaylistViewCommand = new RelayCommand(o =>
            {
                CurrentView = PlaylistVM;
            });
        }
    }
}
