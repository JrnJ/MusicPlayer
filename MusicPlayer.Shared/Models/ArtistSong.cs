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
    public class ArtistSong
    {
        // Keys
        public int ArtistId { get; set; }
        public int SongId { get; set; }

        // Objects
        public Artist Artist { get; set; }
        public Song Song { get; set; }
    }
}
