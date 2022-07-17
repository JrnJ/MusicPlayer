using MusicPlayer.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Media.Imaging;
using MusicPlayer.Classes;
using System.IO;

namespace MusicPlayer.MVVM.Model
{
    internal class AlbumSongModel : ObservableObject
    {
        // Other
        private string _path;

        [JsonProperty("Path")]
        public string Path
        {
            get => _path;
            set { _path = value; OnPropertyChanged(); }
        }

        // Description
        private string _title;

        [JsonIgnore]
        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged(); }
        }

        private string _subtitle;

        [JsonIgnore]
        public string SubTitle
        {
            get { return _subtitle; }
            set { _subtitle = value; OnPropertyChanged(); }
        }

        private string _rating;

        [JsonIgnore]
        public string Rating
        {
            get { return _rating; }
            set { _rating = value; OnPropertyChanged(); }
        }

        private string _comments;

        [JsonIgnore]
        public string Comments
        {
            get { return _comments; }
            set { _comments = value; OnPropertyChanged(); }
        }

        // Media
        private string _contributingArtists;

        [JsonIgnore]
        public string ContributingArtists
        {
            get { return _contributingArtists; }
            set { _contributingArtists = value; OnPropertyChanged(); }
        }

        private string _albumArtist;

        [JsonIgnore]
        public string AlbumArtist
        {
            get { return _albumArtist; }
            set { _albumArtist = value; OnPropertyChanged(); }
        }

        private string _album;

        [JsonIgnore]
        public string Album
        {
            get { return _album; }
            set { _album = value; OnPropertyChanged(); }
        }

        private uint _year;

        [JsonIgnore]
        public uint Year
        {
            get { return _year; }
            set { _year = value; OnPropertyChanged(); }
        }

        private int _id;

        [JsonProperty("Id")]
        public int Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(); }
        }

        private string _genre;

        [JsonIgnore]
        public string Genre
        {
            get { return _genre; }
            set { _genre = value; OnPropertyChanged(); }
        }

        private BitmapImage _image;

        [JsonIgnore]
        public BitmapImage Image
        {
            get { return _image; }
            set { _image = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Length of Media in Milliseconds
        /// </summary>
        private double _length;

        [JsonIgnore]
        public double Length
        {
            get { return _length; }
            set { _length = value; OnPropertyChanged(); }
        }

        // Audio
        private int _bitrate;

        [JsonIgnore]
        public int Bitrate
        {
            get { return _bitrate; }
            set { _bitrate = value; OnPropertyChanged(); }
        }

        // Getters
        public string StringTime => HelperMethods.MsToTime(Length);

        // Constructor
        public AlbumSongModel(string path, int id)
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
