﻿using System.ComponentModel.DataAnnotations;

namespace MusicPlayer.Shared.Models
{
    public class Artist
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        // Junction Tables
        public virtual ICollection<ArtistSong> Songs { get; set; } // Songs Artist has
    }
}
