using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace MusicPlayer.Classes
{
    public class Song
    {
        // Other
        [JsonProperty("Path")]
        public string Path { get; set; }

        // Description
        [JsonIgnore]
        public string Title { get; private set; } = "Unknown Title";

        [JsonIgnore]
        public string Subtitle { get; private set; } = "Unknown Subtitle";

        [JsonIgnore]
        public string Rating { get; private set; } = "";

        [JsonIgnore]
        public string Comments { get; private set; } = "";

        // Media
        [JsonIgnore]
        public string ContributingArtists { get; private set; } = "Unknown Artist";

        [JsonIgnore]
        public string AlbumArtist { get; private set; } = "Unknown Album Artist";

        [JsonIgnore]
        public string Album { get; private set; } = "Unknown Album";

        [JsonIgnore]
        public uint Year { get; private set; } = 1970;

        [JsonProperty("Id")]
        public int Id { get; private set; }

        [JsonIgnore]
        public string Genre { get; private set; } = "Unknown Genre";

        [JsonIgnore]
        public BitmapImage Image { get; private set; }

        /// <summary>
        /// Length of Media in Milliseconds
        /// </summary>
        [JsonIgnore]
        public double Length { get; private set; }

        // Audio
        [JsonIgnore]
        public int Bitrate { get; private set; }

        // Getters
        public string StringTime => HelperMethods.MsToTime(Length);

        // Constructor
        public Song(string path, int id)
        {
            Path = path;
            Id = id;

            AddSongInfo();
        }

        public void SetId(int id)
        {
            Id = id;
        }

        public bool AddSongInfo()
        {
            if (Path != null)
            {
                // Get file
                TagLib.File tFile = TagLib.File.Create(Path);

                if (tFile.Tag.Pictures.Length > 0)
                {
                    TagLib.IPicture picture = tFile.Tag.Pictures[0];
                    MemoryStream ms = new(picture.Data.Data);
                    ms.Seek(0, SeekOrigin.Begin);

                    BitmapImage bitmap = new();
                    bitmap.BeginInit();
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();

                    Image = bitmap;
                }

                //
                if (string.IsNullOrEmpty(tFile.Tag.Title) || string.IsNullOrWhiteSpace(tFile.Tag.Title))
                {
                    Title = Path.Split("\\")[^1];
                }
                else
                {
                    Title = tFile.Tag.Title;
                }

                // 
                Comments = tFile.Tag.Comment;
                ContributingArtists = tFile.Tag.FirstPerformer;
                AlbumArtist = tFile.Tag.FirstAlbumArtist;
                Album = tFile.Tag.Album;
                Year = tFile.Tag.Year;
                Genre = tFile.Tag.FirstGenre;

                Length = tFile.Properties.Duration.TotalMilliseconds;

                //
                Bitrate = tFile.Properties.AudioBitrate;

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
