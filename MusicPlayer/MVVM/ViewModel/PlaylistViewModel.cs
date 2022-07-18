using MusicPlayer.Classes;
using MusicPlayer.Core;
using MusicPlayer.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MusicPlayer.MVVM.ViewModel
{
    internal class PlaylistViewModel
    {
        // <GlobalViewModel> //
        public GlobalViewModel Global { get; } = GlobalViewModel.Instance;
        // </GlobalViewModel> //

        // Song Commands
        public RelayCommand SelectSongCommand { get; set; }

        public RelayCommand RemoveSongCommand { get; set; }

        public RelayCommand AddSongToPlaylistCommand { get; set; }

        public RelayCommand MoveSongUpCommand { get; set; }

        public RelayCommand MoveSongDownCommand { get; set; }

        // View Commands
        public RelayCommand EditPlaylistCommand { get; set; }

        public RelayCommand DeletePlaylistCommand { get; set; }


        public PlaylistViewModel()
        {
            SelectSongCommand = new(o =>
            {
                AlbumSongModel song = Global.SelectedPlaylist.Songs.FirstOrDefault(x => x.Id == (int)o);
                Global.OpenMedia(song);
            });


            RemoveSongCommand = new(o =>
            {
                AlbumSongModel song = Global.SelectedPlaylist.Songs.FirstOrDefault(x => x.Id == (int)o);

                Global.RemoveSongFromPlaylist(song, Global.SelectedPlaylist);
            });

            AddSongToPlaylistCommand = new(o =>
            {
                string[] ids = o.ToString().Split(","); // 0 = playlistId, 1 = songId
                AlbumSongModel song = Global.SelectedPlaylist.Songs.FirstOrDefault(x => x.Id == int.Parse(ids[1]));
                PlaylistModel playlist = Global.Playlists.FirstOrDefault(x => x.Id == int.Parse(ids[0]));

                Global.AddSongToPlaylist(song, playlist);
            });

            MoveSongUpCommand = new(o =>
            {
                int songId = (int)o;
                int songIndex = Global.SelectedPlaylist.Songs.IndexOf(Global.SelectedPlaylist.Songs.Where(x => x.Id == songId).FirstOrDefault());

                if (songIndex != 0)
                {
                    Global.SelectedPlaylist.Songs.Move(songIndex, songIndex - 1);
                    FileHandler.SavePlaylists(Global.Playlists);
                }
            });

            MoveSongDownCommand = new(o =>
            {
                int songId = (int)o;
                int songIndex = Global.SelectedPlaylist.Songs.IndexOf(Global.SelectedPlaylist.Songs.Where(x => x.Id == songId).FirstOrDefault());

                if (songIndex != Global.SelectedPlaylist.Songs.Count - 1)
                {
                    Global.SelectedPlaylist.Songs.Move(songIndex, songIndex + 1);
                    FileHandler.SavePlaylists(Global.Playlists);
                }
            });

            EditPlaylistCommand = new(o =>
            {
                // Create EditPlaylistBox
                Global.EditPlaylistBox = new()
                {
                    Playlist = Global.SelectedPlaylist,
                    Visibility = System.Windows.Visibility.Visible,

                    ConfirmCommand = new(o =>
                    {
                        // Save Playlist
                        // This wank af tbh, fix this crap
                        Global.UpdatePlaylist(Global.SelectedPlaylist.Id, Global.EditPlaylistBox.Playlist);

                        Global.PopupVisibility = System.Windows.Visibility.Collapsed;
                        Global.EditPlaylistBox.Visibility = System.Windows.Visibility.Collapsed;
                    }),
                    CancelCommand = new(o =>
                    {
                        Global.PopupVisibility = System.Windows.Visibility.Collapsed;
                        Global.EditPlaylistBox.Visibility = System.Windows.Visibility.Collapsed;
                    })
                };

                // Show ConfirmBox
                Global.PopupVisibility = System.Windows.Visibility.Visible;
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
