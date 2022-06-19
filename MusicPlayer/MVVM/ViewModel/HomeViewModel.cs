using MusicPlayer.Classes;
using MusicPlayer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.MVVM.ViewModel
{
    internal class HomeViewModel
    {
        // <GlobalViewModel> //
        public GlobalViewModel Global { get; } = GlobalViewModel.Instance;
        // </GlobalViewModel> //

        public RelayCommand SelectSongCommand { get; set; }

        public RelayCommand AddSongToPlaylistCommand { get; set; }

        public RelayCommand RemoveSongCommand { get; set; }

        public HomeViewModel()
        {
            SelectSongCommand = new(o =>
            {
                Global.OpenMedia(Global.MyMusic.Songs[(int)o]);
            });

            AddSongToPlaylistCommand = new(o =>
            {
                string[] ids = o.ToString().Split(","); // 0 = playlistId, 1 = songId
                Song song = Global.MyMusic.Songs.FirstOrDefault(x => x.Id == int.Parse(ids[1]));
                Playlist playlist = Global.Playlists.FirstOrDefault(x => x.Id == int.Parse(ids[0]));

                Global.AddSongToPlaylist(song, playlist);
            });
        }
    }
}
