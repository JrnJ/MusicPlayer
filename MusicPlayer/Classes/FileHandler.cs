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
        /// <summary>
        /// Gets all the playlists from the playlists.json file
        /// </summary>
        /// <returns></returns>
        public static List<Playlist> GetPlaylists()
        {
            try
            {
                string json = File.ReadAllText("C:/Users/jeroe/AppData/Roaming/.jeroenj/MusicPlayer/playlists.json");
                List<Playlist> playlists = JsonConvert.DeserializeObject<List<Playlist>>(json);

                if (playlists == null)
                {
                    return null;
                }

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

        /// <summary>
        /// Gets all settings from the settings.json file
        /// </summary>
        /// <returns></returns>
        public static Settings GetSettings()
        {
            try
            {
                string json = File.ReadAllText("C:/Users/jeroe/AppData/Roaming/.jeroenj/MusicPlayer/settings.json");
                Settings settings = JsonConvert.DeserializeObject<Settings>(json);

                return settings;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<Song> GetSongsFromFolder(string path)
        {
            try
            {
                List<Song> songs = new List<Song>();

                string[] filePaths = Directory.GetFiles(path);

                for (int i = 0; i < filePaths.Length; i++)
                {
                    if (IsMusic(filePaths[i]))
                    {
                        songs.Add(new Song(filePaths[i], i));
                    }
                }

                return songs;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion READ 

        #region WRITE
        /// <summary>
        /// Saves given Playlists to playlists.json
        /// </summary>
        /// <param name="playlists"></param>
        /// <returns>True if save was succesful</returns>
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

        /// <summary>
        /// Saves given settings to settings.json
        /// </summary>
        /// <returns>True if save was succesful</returns>
        public static bool SaveSettings(Settings settings)
        {
            try
            {
                // serialize JSON to a string and then write string to a file
                File.WriteAllText(@"C:/Users/jeroe/AppData/Roaming/.jeroenj/MusicPlayer/settings.json", JsonConvert.SerializeObject(settings));

                // serialize JSON directly to a file
                using (StreamWriter file = File.CreateText(@"C:/Users/jeroe/AppData/Roaming/.jeroenj/MusicPlayer/settings.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, settings);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion WRITE

        public static bool IsMusic(string path)
        {
            if (path.Contains(".mp3"))
            {
                return true;
            }
            else
                return false;
        }
    }
}
