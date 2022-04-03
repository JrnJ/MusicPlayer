using System;
using MusicPlayer.Core;

namespace MusicPlayer.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public PlaylistViewModel PlaylistVM { get; set; }

        private object _currentView;

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }


        public MainViewModel()
        {
            PlaylistVM = new PlaylistViewModel();
        }
    }
}
