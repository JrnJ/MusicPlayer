using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        public int Year { get; set; }

        public TimeSpan Duration { get; set; }

        // Foreign Keys
        [ForeignKey(nameof(SongsFolder))]
        [Required]
        public int SongsFolderId { get; set; }
        public virtual SongsFolder SongsFolder { get; set; }

        [ForeignKey(nameof(Album))]
        public int? AlbumId { get; set; }
        public virtual Album? Album { get; set; }

        // Junction Tables
        public virtual ICollection<ArtistSong> Artists { get; set; } // Artists Song has
        public virtual ICollection<PlaylistSong> Playlists { get; set; } // Playlists Songs is in
        public virtual ICollection<SongGenre> Genres { get; set; } // Genres Songs is in
    }
}
