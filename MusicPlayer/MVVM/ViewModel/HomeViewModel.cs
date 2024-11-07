using MusicPlayer.Core;
using MusicPlayer.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.MVVM.ViewModel
{
    internal class HomeViewModel : ObservableObject
    {
        // <GlobalViewModel> //
        public GlobalViewModel Global { get; } = GlobalViewModel.Instance;
        // </GlobalViewModel> //

        public RelayCommand SelectSongCommand { get; set; }

        public RelayCommand AddSongToPlaylistCommand { get; set; }

        public RelayCommand RemoveSongCommand { get; set; }

        // Properties
        private string _searchText;

        public string SearchText
        {
            get =>_searchText; 
            set
            { 
                _searchText = value;
                Global.SearchSongInMyMusic(value);
                OnPropertyChanged(); 
            }
        }

        public HomeViewModel()
        {
            SelectSongCommand = new(o =>
            {
                SongModel song = Global.MyMusic.Songs.FirstOrDefault(x => x.Id == (int)o);
                Global.OpenMedia(song);
            });

            AddSongToPlaylistCommand = new(o =>
            {
                string[] ids = o.ToString().Split(","); // 0 = playlistId, 1 = songId
                SongModel song = Global.MyMusic.Songs.FirstOrDefault(x => x.Id == int.Parse(ids[1]));
                PlaylistModel playlist = Global.PlaylistsManager.Playlists.FirstOrDefault(x => x.Id == int.Parse(ids[0]));

                Global.AddSongToPlaylist(song, playlist);
            });
        }
    }
}
