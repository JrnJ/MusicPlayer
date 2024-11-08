using System.IO;
using System.Windows.Media.Imaging;

namespace MusicPlayer.Classes
{
    internal class AudioProperties
    {
        public AudioProperties(string path)
        {
            
        }

        public static BitmapImage? GetAlbumArt(string path)
        {
            TagLib.File file = TagLib.File.Create(path);
            TagLib.IPicture? picture = file.Tag.Pictures.FirstOrDefault();

            if (picture == null) return null;

            try
            {
                using MemoryStream mStream = new(picture.Data.Data);

                BitmapImage bitmap = new();
                bitmap.BeginInit();
                bitmap.StreamSource = mStream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                return bitmap;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
