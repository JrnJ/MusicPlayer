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

        // Junction Tables
        public virtual ICollection<SettingsSongsFolder> Settings { get; set; } // SongsFolder Songs has
    }
}
