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
    public class SettingsSongsFolder
    {
        // Keys
        public int SettingsId { get; set; }
        public int SongsFolderId { get; set; }

        // Objects
        public Settings Settings { get; set; }
        public SongsFolder SongsFolder { get; set; }
    }
}
