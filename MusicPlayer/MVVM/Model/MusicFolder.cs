using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace MusicPlayer.MVVM.Model
{
    internal class MusicFolder
    {
        private int _id;

        [JsonProperty("Id")]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _path;

        [JsonProperty("Path")]
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
    }
}
