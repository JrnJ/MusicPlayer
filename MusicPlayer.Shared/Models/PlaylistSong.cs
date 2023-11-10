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
    public class PlaylistSong
    {
        // Keys
        public int PlaylistId { get; set; }
        public int SongId { get; set; }

        // Objects
        public Playlist Playlist { get; set; }
        public Song Song { get; set; }
    }
}
