using MusicPlayer.Core;
using System;

namespace MusicPlayer.MVVM.ViewModel
{
    internal class PlaylistViewModel
    {
        // <GlobalViewModel> //
        public GlobalViewModel Global { get; } = GlobalViewModel.Instance;
        // </GlobalViewModel> //

        public RelayCommand SelectSongCommand { get; set; }

        public PlaylistViewModel()
        {
            SelectSongCommand = new(o =>
            {
                PlaySong((int)o);
            });
        }

        public void PlaySong(int id)
        {
            
        }
    }
}
