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

        public RelayCommand AddSongToPlaylist { get; set; }

        public PlaylistViewModel()
        {
            SelectSongCommand = new(o =>
            {
                PlaySong((int)o);
            });

            AddSongToPlaylist = new(o =>
            {
                string[] ids = o.ToString().Split(","); // 0 = playlistId, 1 = songId
                Global.Playlists[int.Parse(ids[0])].Songs.Add(Global.SelectedPlaylist.Songs[int.Parse(ids[1])]);
            });
        }

        public void PlaySong(int id)
        {
            Global.OpenMedia(Global.SelectedPlaylist.Songs[id]);
        }
    }
}
