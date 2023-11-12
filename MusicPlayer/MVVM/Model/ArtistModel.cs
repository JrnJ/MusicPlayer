using MusicPlayer.Core;
using MusicPlayer.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.MVVM.Model
{
    internal class ArtistModel : ObservableObject
    {
        private int _id;

        public int Id
        {
            get  => _id; 
            set { _id = value; OnPropertyChanged(); }
        }

        private string _name;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public ArtistModel() { }

        public ArtistModel(Artist artist)
        {
            _id = artist.Id;
        }

        public Artist ToArtist()
        {
            return new()
            {
                Id = _id,
                Name = _name
            };
        }
    }
}
