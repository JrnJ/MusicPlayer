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
        public static ObservableCollection<Playlist> GetPlaylists()
        {
            try
            {
                string json = File.ReadAllText("C:/Users/jeroe/AppData/Roaming/.jeroenj/MusicPlayer/playlists.json");
                ObservableCollection<Playlist> playlists = JsonConvert.DeserializeObject<ObservableCollection<Playlist>>(json);

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
                Console.WriteLine("ERROR: FileHandler.cs ln19" + ex);

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
                Console.WriteLine("ERROR: FileHandler.cs ln53" + ex);

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
                Console.WriteLine("ERROR: FileHandler.cs ln68" + ex);

                return null;
            }
        }
        #endregion READ 

        #region WRITE
        //public static void SaveToJSON(T toSave, string filePath)
        //{
        //    Console.WriteLine("Saving File...");

        //    try
        //    {
        //        string jsonstring = JsonConvert.SerializeObject(toSave);
        //        File.WriteAllText(filePath, jsonstring);

        //        Console.WriteLine("File saved succesfully");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("File could not be saved!\n" + ex);
        //    }
        //}

        /// <summary>
        /// Saves given Playlists to playlists.json
        /// </summary>
        /// <param name="playlists"></param>
        /// <returns>True if save was succesful</returns>
        public static bool SavePlaylists(ObservableCollection<Playlist> playlists)
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
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex);
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
