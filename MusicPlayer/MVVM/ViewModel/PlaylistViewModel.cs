using MusicPlayer.Classes;
using MusicPlayer.Core;
using System;
using System.Linq;

namespace MusicPlayer.MVVM.ViewModel
{
    internal class PlaylistViewModel
    {
        // <GlobalViewModel> //
        public GlobalViewModel Global { get; } = GlobalViewModel.Instance;
        // </GlobalViewModel> //

        public RelayCommand SelectSongCommand { get; set; }

        public RelayCommand AddSongToPlaylist { get; set; }

        public RelayCommand RemoveSongCommand { get; set; }

        public PlaylistViewModel()
        {
            SelectSongCommand = new(o =>
            {
                Global.OpenMedia(Global.SelectedPlaylist.Songs[(int)o]);
            });

            AddSongToPlaylist = new(o =>
            {
                string[] ids = o.ToString().Split(","); // 0 = playlistId, 1 = songId
                Song song = Global.SelectedPlaylist.Songs.FirstOrDefault(x => x.Id == int.Parse(ids[1]));
                Playlist playlist = Global.Playlists.FirstOrDefault(x => x.Id == int.Parse(ids[0]));

                Global.AddSongToPlaylist(song, playlist);
            });

            RemoveSongCommand = new(o =>
            {
                Song song = Global.SelectedPlaylist.Songs.FirstOrDefault(x => x.Id == (int)o);

                Global.RemoveSongFromPlaylist(song, Global.SelectedPlaylist);
            });
        }
    }
}
