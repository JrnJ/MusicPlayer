using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace MusicPlayer.MVVM.Model
{
    internal class MusicFolder
    {
        private int _id;

        [JsonPropertyName("Id")]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _path;

        [JsonPropertyName("Path")]
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
    }
}
