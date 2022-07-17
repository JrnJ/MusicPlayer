using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicPlayer.Classes;
using MusicPlayer.Core;
using Newtonsoft.Json;

namespace MusicPlayer.MVVM.Model
{
    internal class PlaylistModel : ObservableObject
    {
        private int _id;

        [JsonProperty("Id")]
        public int Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(); }
        }

        private string _name;

        [JsonProperty("Name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        private string _description;

        [JsonProperty("Description")]
        public string Description
        {
            get { return _description; }
            set { _description = value; OnPropertyChanged(); }
        }

        private ObservableCollection<AlbumSongModel> _songs;

        [JsonProperty("Songs")]
        public ObservableCollection<AlbumSongModel> Songs
        {
            get { return _songs; }
            set { _songs = value; OnPropertyChanged(); }
        }

        public string PlaylistDuration
        {
            get
            {
                double totalMs = 0;
                for (int i = 0; i < Songs.Count; i++)
                {
                    totalMs += Songs[i].Length;
                }

                return HelperMethods.MsToTime(totalMs);
            }
        }

        public PlaylistModel(int id, ObservableCollection<AlbumSongModel> songs, string name = "", string description = "")
        {
            Id = id;
            Songs = songs;
            Name = name;
            Description = description;
        }
    }
}
