using MusicPlayer.Core;
using MusicPlayer.Shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.MVVM.Model
{
    internal class PlaylistModel : ObservableObject
    {
        private int _id;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        private string _name;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private string _description;

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        private ObservableCollection<SongModel> _songs;

        public ObservableCollection<SongModel> Songs
        {
            get => _songs;
            set { _songs = value; OnPropertyChanged(); }
        }

        public PlaylistModel() { }

        public PlaylistModel(Playlist playlist, ObservableCollection<SongModel> allSongs)
        {
            _id = playlist.Id;
            _name = playlist.Name;
            _description = playlist.Description;

            //
            _songs = new();

            // Reference based!
            foreach (PlaylistSong playlistSong in playlist.Songs)
            {
                foreach (SongModel song in allSongs)
                {
                    if (playlistSong.SongId == song.Id)
                    {
                        Songs.Add(song);
                    }
                }
            }
        }

        public Playlist ToPlaylist()
        {
            return new()
            {
                Id = _id,
                Name = _name,
                Description = _description
            };
        }
    }
}
