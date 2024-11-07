using MusicPlayer.Core;
using MusicPlayer.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Classes
{
    internal class PlaylistsManager : ObservableObject
    {
        #region Properties
        private ObservableCollection<PlaylistModel> _playlists = [];

        public ObservableCollection<PlaylistModel> Playlists
        {
            get => _playlists;
            set { _playlists = value; OnPropertyChanged(); }
        }

        private PlaylistModel? _playlistViewing;

        public PlaylistModel? PlaylistViewing
        {
            get => _playlistViewing;
            set
            {
                _playlistViewing = value;
                OnPropertyChanged();
            }
        }

        private PlaylistModel? _playlistPlaying;

        public PlaylistModel? PlaylistPlaying
        {
            get => _playlistPlaying;
            set
            {
                _playlistPlaying = value;
                OnPropertyChanged();
            }
        }

        private PlaylistModel? _searchingPlaylist;

        public PlaylistModel? SearchingPlaylist
        {
            get { return _searchingPlaylist; }
            set { _searchingPlaylist = value; OnPropertyChanged(); }
        }
        #endregion Properties

        public void SetViewingPlaylist(PlaylistModel playlist)
        {
            PlaylistViewing = playlist;
        }

        public void SetPlayingPlaylist(PlaylistModel playlist)
        {
            PlaylistPlaying = playlist;
        }

        public void SetSearchingPlaylist(PlaylistModel playlist)
        {
            SearchingPlaylist = playlist;
        }

        public void Add(PlaylistModel playlist)
        {
            Playlists.Add(playlist);
        }

        public void Remove(PlaylistModel playlist)
        {
            throw new NotImplementedException();
        }

        public void Remove(int id)
        {
            PlaylistModel? playlistModelToDelete = Playlists.FirstOrDefault(x => x.Id == id);
            if (playlistModelToDelete != null)
            {
                Playlists.Remove(playlistModelToDelete);
            }
        }
    }
}
