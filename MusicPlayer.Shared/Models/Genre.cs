using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Shared.Models
{
    public class Genre
    {
        [Key]
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        // Junction Table
        public virtual ICollection<SongGenre> Songs { get; set; } // Songs Genre is in
    }
}
