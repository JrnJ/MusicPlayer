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
using Windows.Storage.FileProperties;
using Windows.Storage;

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

        private int _id;

        [JsonProperty("Id")]
        public int Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(); }
        }

        private Microsoft.UI.Xaml.Media.Imaging.BitmapImage _bitmapImage;

        [JsonIgnore]
        public Microsoft.UI.Xaml.Media.Imaging.BitmapImage BitmapImage
        {
            get { return _bitmapImage; }
            set { _bitmapImage = value; OnPropertyChanged(); }
        }

        private Windows.Storage.FileProperties.MusicProperties _musicProperties;

        [JsonIgnore]
        public Windows.Storage.FileProperties.MusicProperties MusicProperties
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

        // Getters
        public string StringTime => HelperMethods.MsToTime(MusicProperties.Duration.TotalMilliseconds);
        // TODO: Change how playlists work

        // Constructor
        public AlbumSongModel()
        {
            
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
