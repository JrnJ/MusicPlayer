using MusicPlayer.Core;
using System;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.IO;
using Windows.Storage.FileProperties;
using System.Text.Json.Serialization;

namespace MusicPlayer.MVVM.Model
{
    internal class AlbumSongModel : ObservableObject
    {
        // Other
        private string _path;

        [JsonPropertyName("Path")]
        public string Path
        {
            get => _path;
            set 
            { 
                _path = value;
                FileType = value.Split('.')[^1];
                OnPropertyChanged(); 
            }
        }

        private int _id;

        [JsonPropertyName("Id")]
        public int Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(); }
        }

        private bool _isPlaying;

        [JsonIgnore]
        public bool IsPlaying
        {
            get { return _isPlaying; }
            set { _isPlaying = value; OnPropertyChanged(); }
        }

        private Microsoft.UI.Xaml.Media.Imaging.BitmapImage _bitmapImage;

        [JsonIgnore]
        public Microsoft.UI.Xaml.Media.Imaging.BitmapImage BitmapImage
        {
            get { return _bitmapImage; }
            set { _bitmapImage = value; OnPropertyChanged(); }
        }

        private MusicProperties _musicProperties;

        [JsonIgnore]
        public MusicProperties MusicProperties
        {
            get { return _musicProperties; }
            set { _musicProperties = value; OnPropertyChanged(); }
        }

        private BitmapImage _image;

        [JsonIgnore]
        public BitmapImage Image
        {
            get { return _image; }
            set { _image = value; OnPropertyChanged(); }
        }

        private string _fileType;

        [JsonIgnore]
        public string FileType
        {
            get { return _fileType.ToUpper(); }
            set { _fileType = value; OnPropertyChanged(); }
        }

        public AlbumSongModel()
        {
            IsPlaying = false;
        }

        public async Task Init(Windows.Storage.StorageFile storageFile)
        {
            MusicProperties = await storageFile.Properties.GetMusicPropertiesAsync();

            if (string.IsNullOrEmpty(MusicProperties.Title))
                MusicProperties.Title = storageFile.Name;

            StorageItemThumbnail thumbnail = await storageFile.GetThumbnailAsync(ThumbnailMode.MusicView);

            if (thumbnail != null)
            {
                BitmapImage bitmap = new();

                bitmap.BeginInit();
                bitmap.StreamSource = thumbnail.AsStreamForRead();
                bitmap.EndInit();

                Image = bitmap;
            }
        }

        public void SetId(int id)
        {
            Id = id;
        }

        // https://stackoverflow.com/questions/18843315/how-to-store-save-thumbnail-image-in-device-in-windows-8-metro-apps-c-sharp/18844387#18844387
    }
}
