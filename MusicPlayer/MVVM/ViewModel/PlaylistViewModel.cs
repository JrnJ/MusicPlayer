using System;

namespace MusicPlayer.MVVM.ViewModel
{
    internal class PlaylistViewModel
    {
        // <GlobalViewModel> //
        public GlobalViewModel Global { get; } = GlobalViewModel.Instance;
        // </GlobalViewModel> //

        public PlaylistViewModel()
        {
            
        }
    }
}
