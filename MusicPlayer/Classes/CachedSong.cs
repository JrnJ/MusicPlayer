using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MusicPlayer.Classes
{
    internal class CachedSong
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

		private string _title;

		[JsonProperty("Title")]
		public string Title
		{
			get { return _title; }
			set { _title = value; }
		}

		private string _artist;

		[JsonProperty("Artist")]
		public string Artist
		{
			get { return _artist; }
			set { _artist = value; }
		}
	}
}
