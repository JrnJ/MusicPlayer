using MusicPlayer.Core;
using MusicPlayer.Shared.Models;
using System.Collections.ObjectModel;

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

        private string _imagePath;

        public string ImagePath
        {
            get => _imagePath;
            set { _imagePath = value; OnPropertyChanged(); }
        }

        // Not present in Database
        private ObservableCollection<SongModel> _songs;

        public ObservableCollection<SongModel> Songs
        {
            get => _songs;
            set { _songs = value; OnPropertyChanged(); }
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }


        public PlaylistModel() { }

        public PlaylistModel(Playlist playlist, ObservableCollection<SongModel> allSongs)
        {
            _id = playlist.Id;
            _name = playlist.Name;
            _description = playlist.Description;
            _imagePath = playlist.ImagePath;

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
                        break;
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
                Description = _description,
                ImagePath = _imagePath,
            };
        }
    }
}
