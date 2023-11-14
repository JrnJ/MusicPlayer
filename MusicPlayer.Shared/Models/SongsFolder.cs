using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Shared.Models
{
    public class SongsFolder
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Path { get; set; }

        // 1:n with Song
        public virtual ICollection<Song> Songs { get; set; } // Songs Songsfolder has

        // Junction Tables
        public virtual ICollection<SettingsSongsFolder> Settings { get; set; } // Settings SongsFolder has
    }
}
