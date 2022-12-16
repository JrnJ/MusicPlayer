using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MusicPlayer.Classes
{
    internal class CachedSong
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

		private string _title;

		[JsonPropertyName("Title")]
		public string Title
		{
			get { return _title; }
			set { _title = value; }
		}

		private string _artist;

		[JsonPropertyName("Artist")]
		public string Artist
		{
			get { return _artist; }
			set { _artist = value; }
		}
	}
}
