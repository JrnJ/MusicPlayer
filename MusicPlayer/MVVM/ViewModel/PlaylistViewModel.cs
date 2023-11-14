using Microsoft.UI.Xaml.Controls;
using MusicPlayer.Classes;
using MusicPlayer.Core;
using MusicPlayer.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace MusicPlayer.MVVM.ViewModel
{
    internal class PlaylistViewModel : ObservableObject
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

        public RelayCommand LoopPlaylistCommand { get; set; }

        public RelayCommand ShufflePlaylistCommand { get; set; }

        // Properties
        private SolidColorBrush _loopButtonColor;

        public SolidColorBrush LoopButtonColor
        {
            get { return _loopButtonColor; }
            set { _loopButtonColor = value; OnPropertyChanged(); }
        }

        private SolidColorBrush _shuffleButtonColor;

        public SolidColorBrush ShuffleButtonColor
        {
            get { return _shuffleButtonColor; }
            set { _shuffleButtonColor = value; OnPropertyChanged(); }
        }

        // Properties
        private string _searchText;

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                Global.SearchSongInPlaylist(value);
                OnPropertyChanged();
            }
        }

        public PlaylistViewModel()
        {
            LoopButtonColor = GetSelectedColor(false);
            ShuffleButtonColor = GetSelectedColor(false);

            SelectSongCommand = new(o =>
            {
                SongModel song = Global.PlaylistViewing.Songs.FirstOrDefault(x => x.Id == (int)o);
                //AlbumSongModel song = Global.MyMusic.Songs.FirstOrDefault(x => x.Id == (int)o);
                Global.OpenMedia(song);
            });

            RemoveSongCommand = new(o =>
            {
                SongModel song = Global.PlaylistViewing.Songs.FirstOrDefault(x => x.Id == (int)o);

                Global.RemoveSongFromPlaylist(song, Global.PlaylistViewing);
            });

            AddSongToPlaylistCommand = new(o =>
            {
                string[] ids = o.ToString().Split(","); // 0 = playlistId, 1 = songId
                SongModel song = Global.PlaylistViewing.Songs.FirstOrDefault(x => x.Id == int.Parse(ids[1]));
                PlaylistModel playlist = Global.Playlists.FirstOrDefault(x => x.Id == int.Parse(ids[0]));

                Global.AddSongToPlaylist(song, playlist);
            });

            MoveSongUpCommand = new(o =>
            {
                int songId = (int)o;
                int songIndex = Global.PlaylistViewing.Songs.IndexOf(Global.PlaylistViewing.Songs.Where(x => x.Id == songId).FirstOrDefault());

                if (songIndex != 0)
                {
                    // TODO2: order doesnt exist
                    Global.PlaylistViewing.Songs.Move(songIndex, songIndex - 1);
                    //Global.SavePlaylists();
                }
            });

            MoveSongDownCommand = new(o =>
            {
                int songId = (int)o;
                int songIndex = Global.PlaylistViewing.Songs.IndexOf(Global.PlaylistViewing.Songs.Where(x => x.Id == songId).FirstOrDefault());

                if (songIndex != Global.PlaylistViewing.Songs.Count - 1)
                {
                    // TODO2: order doesnt exist
                    Global.PlaylistViewing.Songs.Move(songIndex, songIndex + 1);
                    //Global.SavePlaylists();
                }
            });

            EditPlaylistCommand = new(o =>
            {
                // 
                PlaylistModel playlistToCopy = Global.PlaylistViewing;

                // Create EditPlaylistBox
                Global.EditPlaylistBox = new()
                {
                    Playlist = new()
                    {
                        Name = playlistToCopy.Name,
                        Description = playlistToCopy.Description,
                        ImagePath = playlistToCopy.ImagePath,
                        Songs = playlistToCopy.Songs,
                    },
                    Visibility = System.Windows.Visibility.Visible,

                    ConfirmCommand = new(o =>
                    {
                        // Save Playlist
                        // This wank af tbh, fix this crap
                        Global.UpdatePlaylist(Global.PlaylistViewing.Id, Global.EditPlaylistBox.Playlist);

                        Global.PopupVisibility = System.Windows.Visibility.Collapsed;
                        Global.EditPlaylistBox.Visibility = System.Windows.Visibility.Collapsed;
                    }),
                    CancelCommand = new(o =>
                    {
                        Global.PopupVisibility = System.Windows.Visibility.Collapsed;
                        Global.EditPlaylistBox.Visibility = System.Windows.Visibility.Collapsed;
                    }),
                    ChangeImageCommand = new(o =>
                    {
                        // Open File Explorer for image
                        OpenFilePicker();
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
                    Description = $"Are you sure you want to delete {Global.PlaylistViewing.Name}?",
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
                        int playlistId = Global.PlaylistViewing.Id;
                        Global.DeletePlaylist(playlistId);

                        // Change View to Playlists View
                        Global.CurrentView = Global.PlaylistsVM;
                    })
                };

                // Show ConfirmBox
                Global.PopupVisibility = System.Windows.Visibility.Visible;
            });

            LoopPlaylistCommand = new(o =>
            {
                Global.LoopPlaylistEnabled = !Global.LoopPlaylistEnabled;
                LoopButtonColor = GetSelectedColor(Global.LoopPlaylistEnabled);
            });

            ShufflePlaylistCommand = new(o =>
            {
                Global.ShufflePlaylistEnabled = !Global.ShufflePlaylistEnabled;
                ShuffleButtonColor = GetSelectedColor(Global.ShufflePlaylistEnabled);
            });
        }

        private async void OpenFilePicker()
        {
            FileOpenPicker filePicker = new();
            filePicker.FileTypeFilter.Add(".png");
            filePicker.FileTypeFilter.Add(".jpg");
            filePicker.FileTypeFilter.Add(".jpeg");

            StorageFile file = await AppWindowExtensions.OpenFilePicker(filePicker);

            if (file == null)
            {
                return;
            }

            if (file.ContentType.Contains("image"))
            {
                Global.EditPlaylistBox.Playlist.ImagePath = file.Path;
            }
        }

        public SolidColorBrush GetSelectedColor(bool isSelected) => isSelected ? new SolidColorBrush(Color.FromRgb(49, 49, 49)) : new SolidColorBrush(Color.FromRgb(41, 41, 41));
    }
}
