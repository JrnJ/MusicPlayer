using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Classes
{
    public class Playlist
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<Song> Songs { get; set; }

        public Playlist(int id, string name, List<Song> songs)
        {
            Id = id;
            Name = name;
            Songs = songs;
        }
    }
}
