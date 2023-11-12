using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Shared.Models
{
    /// <summary>
    /// Junction Table
    /// </summary>
    public class Settings
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public double Volume { get; set; }

        // Junction Tables
        public virtual ICollection<SettingsSongsFolder> SongsFolders { get; set; } // SongsFolder Settings is in
    }
}
