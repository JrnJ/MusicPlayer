using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Classes
{
    public class Song
    {
        // Other
        public Uri Path { get; set; }

        // Description
        public string Title { get; private set; } = "Unknown Title";

        public string Subtitle { get; private set; } = "Unknown Subtitle";

        public string Rating { get; private set; } = "";

        public string Comments { get; private set; } = ""; 

        // Media
        public string ContributingArtists { get; private set; } = "Unknown Artist";

        public string AlbumArtist { get; private set; } = "Unknown Album Artist";

        public string Album { get; private set; } = "Unknown Album";

        public uint Year { get; private set; } = 0;

        public int Id { get; private set; }

        public string Genre { get; private set; } = "Unknown Genre";

        public TagLib.Picture Picture { get; private set; }

        /// <summary>
        /// Length of Media in Milliseconds
        /// </summary>
        public double Length { get; private set; }

        // Audio
        public int Bitrate { get; private set; }


        // Constructor
        public Song(Uri path, string title, string subtitle, string rating, string comments, 
            string contributingArtists, string albumArtists, string album, uint year, int id, string genre, TagLib.Picture picture, double length,
            int bitrate)
        {
            Path = path;

            if (title != null)
                Title = title;
            else
                Title = path.ToString().Split("/")[path.ToString().Split("/").Length - 1];

            if (subtitle != null)
                Subtitle = subtitle;

            Rating = rating;
            Comments = comments;

            if (contributingArtists != null)
                ContributingArtists = contributingArtists;

            if (albumArtists != null)
                AlbumArtist = albumArtists;

            if (album != null)
                Album = album;

            Year = year;
            Id = id;

            if (genre != null)
                Genre = genre;

            if (picture != null)
                Picture = picture;
            else
                Picture = new TagLib.Picture("../../../Images/SongImagePlaceholder.png");

            Length = length;

            Bitrate = bitrate;
        }
    }
}
