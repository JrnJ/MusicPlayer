using MusicPlayer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.MVVM.ViewModel
{
    internal class PlaylistsViewModel : ObservableObject
    {
        // <GlobalViewModel> //
        public GlobalViewModel Global { get; } = GlobalViewModel.Instance;
        // </GlobalViewModel> //   

        // Commands
        public RelayCommand NewPlaylistCommand { get; set; }

        public RelayCommand ShowPlaylistCommand { get; set; }

        public PlaylistsViewModel()
        {
            NewPlaylistCommand = new(o =>
            {
                Global.CreatePlaylist();
            });

            ShowPlaylistCommand = new(o =>
            {
                Global.ShowPlaylist((int)o);
            });
        }
    }
}
