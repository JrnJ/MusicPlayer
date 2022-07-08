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

        public RelayCommand AddSongToPlaylistCommand { get; set; }

        public RelayCommand RemoveSongCommand { get; set; }

        public RelayCommand EditPlaylistCommand { get; set; }

        public RelayCommand DeletePlaylistCommand { get; set; }


        public PlaylistViewModel()
        {
            SelectSongCommand = new(o =>
            {
                Global.OpenMedia(Global.SelectedPlaylist.Songs[(int)o]);
            });

            AddSongToPlaylistCommand = new(o =>
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

            EditPlaylistCommand = new(o =>
            {
                
            });

            DeletePlaylistCommand = new(o =>
            {
                // Create ConfirmBox
                Global.ConfirmBox = new()
                {
                    Title = "Delete Playlist?",
                    Description = $"Are you sure you want to delete {Global.SelectedPlaylist.Name}?",
                    ConfirmText = "Cancel",
                    CancelText = "Delete",
                    Visibility = System.Windows.Visibility.Visible,

                    ConfirmCommand = new(o =>
                    {
                        Global.PopupVisibility = System.Windows.Visibility.Collapsed;
                        Global.ConfirmBox.Visibility = System.Windows.Visibility.Collapsed;
                    }),
                    CancelCommand = new(o =>
                    {
                        Global.PopupVisibility = System.Windows.Visibility.Collapsed;
                        Global.ConfirmBox.Visibility = System.Windows.Visibility.Collapsed;

                        // Delete Playlist
                        int playlistId = Global.SelectedPlaylist.Id;
                        Global.DeletePlaylist(playlistId);

                        // Change View to Playlists View
                        Global.CurrentView = Global.PlaylistsVM;
                    })
                };

                // Show ConfirmBox
                Global.PopupVisibility = System.Windows.Visibility.Visible;
            });
        }
    }
}
