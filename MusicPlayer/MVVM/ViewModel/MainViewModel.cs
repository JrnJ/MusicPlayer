using System;
using MusicPlayer.Core;

namespace MusicPlayer.MVVM.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        // Commands
        public RelayCommand HomeViewCommand { get; set; }

        public RelayCommand PlaylistsViewCommand { get; set; }

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
        }
    }
}
