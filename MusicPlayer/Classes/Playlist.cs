using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Classes
{
    public class Playlist
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Songs")]
        public ObservableCollection<Song> Songs { get; set; }

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

        public Playlist(int id, ObservableCollection<Song> songs, string name = "", string description = "")
        {
            Id = id;
            Songs = songs;
            Name = name;
            Description = description;
        }
    }
}
