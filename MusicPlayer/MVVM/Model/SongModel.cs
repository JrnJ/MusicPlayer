using MusicPlayer.Core;
using MusicPlayer.Shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.Media.Playlists;

namespace MusicPlayer.MVVM.Model
{
    internal class SongModel : ObservableObject
    {
        private int _id;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        private string _path;

        public string Path
        {
            get => _path;
            set { _path = value; OnPropertyChanged(); }
        }

        private string _title;

        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        private int _year;

        public int Year
        {
            get { return _year; }
            set { _year = value; OnPropertyChanged(); }
        }

        private TimeSpan _duration;

        public TimeSpan Duration
        {
            get { return _duration; }
            set { _duration = value; OnPropertyChanged(); }
        }

        private int _songsFolderId;

        public int SongsFolderId
        {
            get { return _songsFolderId; }
            set { _songsFolderId = value; OnPropertyChanged(); }
        }

        // Not present in Database
        private ObservableCollection<ArtistModel> _artists = new();

        public ObservableCollection<ArtistModel> Artists
        {
            get => _artists;
            set 
            { 
                _artists = value;
                TextualizeArtists();
                OnPropertyChanged(); 
            }
        }

        private ObservableCollection<GenreModel> _genres = new();

        public ObservableCollection<GenreModel> Genres
        {
            get => _genres;
            set  {  _genres = value; OnPropertyChanged(); }
        }

        private string _artistsText = "Unknown Artist";

        public string ArtistsText
        {
            get => _artistsText;
            set { _artistsText = value; OnPropertyChanged(); }
        }

        private bool _isPlaying;

        public bool IsPlaying
        {
            get => _isPlaying;
            set { _isPlaying = value; OnPropertyChanged(); }
        }

        private BitmapImage _image;

        public BitmapImage Image
        {
            get => _image;
            set { _image = value; OnPropertyChanged(); }
        }

        public SongModel() { }

        public SongModel(Song song, ObservableCollection<ArtistModel> allArtists, ObservableCollection<GenreModel> allGenres)
        {
            _id = song.Id;
            _path = song.Path;
            _title = song.Title;
            _year = song.Year;
            _duration = song.Duration;
            _songsFolderId = song.SongsFolderId;

            // Reference based!
            foreach (ArtistSong artistSong in song.Artists)
            {
                foreach (ArtistModel artist in allArtists)
                {
                    if (artistSong.ArtistId == artist.Id)
                    {
                        Artists.Add(artist);
                        break;
                    }
                }
            }

            foreach (SongGenre songGenre in song.Genres)
            {
                foreach (GenreModel genre in allGenres)
                {
                    if (songGenre.GenreName == genre.Name)
                    {
                        Genres.Add(genre);
                        break;
                    }
                }
            }

            TextualizeArtists();
        }

        public Song ToSong()
        {
            return new Song
            {
                Id = _id,
                Path = _path,
                Title = _title,
                Year = _year,
                Duration = _duration,
                SongsFolderId = _songsFolderId
            };
        }

        private void TextualizeArtists()
        {
            // Alter Artists Text
            string newArtistsText = "";
            foreach (ArtistModel artist in _artists)
            {
                newArtistsText += artist.Name;
            }
            ArtistsText = newArtistsText;
        }
    }
}
