using MusicPlayer.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Models
{
    /// <summary>
    /// Class to combine Song and all Playlists
    /// </summary>
    public class SongPlaylists
    {
        public Song Song { get; set; }
        public ObservableCollection<Playlist> Playlists { get; set; }
    }
}
