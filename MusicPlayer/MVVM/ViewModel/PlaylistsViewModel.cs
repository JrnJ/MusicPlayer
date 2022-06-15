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

        // Commands???
        public RelayCommand PlaylistViewCommand { get; set; }

        public PlaylistsViewModel()
        {
            // Assign Commands
            PlaylistViewCommand = new(o =>
            {
                Global.SelectedPlaylist = Global.Playlists[int.Parse(o.ToString())];
                Global.CurrentView = Global.PlaylistVM;
            });
        }
    }
}
