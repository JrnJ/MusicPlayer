using MusicPlayer.Core;
using MusicPlayer.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.MVVM.Model
{
    internal class SongsFolderModel : ObservableObject
    {
        private int _id;

        public int Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(); }
        }

        private string _path;

        public string Path
        {
            get { return _path; }
            set { _path = value; OnPropertyChanged(); }
        }

        public SongsFolderModel() { }

        public SongsFolderModel(SongsFolder songsFolder)
        {
            _id = songsFolder.Id;
            _path = songsFolder.Path;
        }

        public SongsFolder ToSongsFolder()
        {
            return new()
            {
                Id = _id,
                Path = _path
            };
        }
    }
}
