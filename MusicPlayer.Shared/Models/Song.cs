using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Shared.Models
{
    public class Song
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Path { get; set; }

        public string Title { get; set; }

        // Junction Tables
        public virtual ICollection<ArtistSong> Artists { get; set; } // Artists Song has
        public virtual ICollection<PlaylistSong> Playlists { get; set; } // Playlists Songs is in
    }
}
