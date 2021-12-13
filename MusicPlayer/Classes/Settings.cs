using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Classes
{
    public class Settings
    {
        [JsonProperty("Volume")]
        public double Volume { get; set; }

        [JsonProperty("MusicFolders")]
        public List<string> MusicFolders { get; set; }
    }
}
