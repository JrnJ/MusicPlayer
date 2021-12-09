using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace MusicPlayer.Classes
{
    public static class FileHandler
    {
        #region READ
        public static List<Playlist> GetPlaylists()
        {
            try
            {
                string json = File.ReadAllText("C:/Users/jeroe/AppData/Roaming/.jeroenj/MusicPlayer/playlists.json");
                List<Playlist> playlists = JsonConvert.DeserializeObject<List<Playlist>>(json);

                for (int i = 0; i < playlists.Count; i++)
                {
                    for (int i2 = 0; i2 < playlists[i].Songs.Count; i2++)
                    {
                        playlists[i].Songs[i2].AddSongInfo();
                    }
                }

                return playlists;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static Song GetSong(string path)
        {
            try
            {
                //// Check if song is music
                //if (IsMusic(path))
                //{
                //    // Get file
                //    TagLib.File tFile = TagLib.File.Create(path);

                //    TagLib.Picture pic = new TagLib.Picture("../../../Images/SongImagePlaceholder.png");
                //    //TagLib.Picture pic = null;

                //    // Add song to songs
                //    return new Song(new Uri(path), tFile.Tag.Title, null, null, tFile.Tag.Comment,
                //        tFile.Tag.FirstPerformer, tFile.Tag.FirstAlbumArtist, tFile.Tag.Album, tFile.Tag.Year, id, tFile.Tag.FirstGenre, pic, tFile.Properties.Duration.TotalMilliseconds,
                //        tFile.Properties.AudioBitrate
                //        );
                //}


                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion READ 

        #region WRITE
        public static bool SavePlaylistsLocation(List<Playlist> playlists)
        {
            try
            {
                // serialize JSON to a string and then write string to a file
                File.WriteAllText(@"C:/Users/jeroe/AppData/Roaming/.jeroenj/MusicPlayer/playlists.json", JsonConvert.SerializeObject(playlists));

                // serialize JSON directly to a file
                using (StreamWriter file = File.CreateText(@"C:/Users/jeroe/AppData/Roaming/.jeroenj/MusicPlayer/playlists.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, playlists);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SaveSettings()
        {
            try
            {
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion WRITE

        public static bool IsMusic(string fileName)
        {
            return fileName.Contains(".mp3");
        }
    }
}
