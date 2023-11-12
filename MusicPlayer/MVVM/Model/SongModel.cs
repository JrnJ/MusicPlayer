using MusicPlayer.Core;
using MusicPlayer.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public SongModel() { }

        public SongModel(Song song)
        {
            _id = song.Id;
            _path = song.Path;
            _title = song.Title;
        }

        public Song ToSong()
        {
            return new Song
            {
                Id = _id,
                Path = _path,
                Title = _title,
            };
        }
    }
}
