using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Shared.Models
{
    /// <summary>
    /// Junction Table
    /// </summary>
    public class SongGenre
    {
        // Keys
        public int SongId { get; set; }
        public string GenreName { get; set; }

        // Objects
        public Song Song { get; set; }
        public Genre Genre { get; set; }
    }
}
