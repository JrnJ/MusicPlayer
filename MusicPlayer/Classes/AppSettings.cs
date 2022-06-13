using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Classes
{
    public static class AppSettings
    {
        private static Settings _settings { get; set; } = new Settings();

        public static double Volume
        {
            get => _settings.Volume;
            set 
            {
                _settings.Volume = value; 
            }
        }

        public static List<string> MusicFolders
        {
            get => _settings.MusicFolders;
            set 
            {
                _settings.MusicFolders = value;
            }
        }

        // Can also transform settings property but this might be less confusing for a user
        public static Settings GetSettings() => _settings;

        public static void GetSettingsFromFile()
        {
            try
            {
                string json = File.ReadAllText("C:/Users/jeroe/AppData/Roaming/.jeroenj/MusicPlayer/settings.json");
                _settings = JsonConvert.DeserializeObject<Settings>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: Could not get settings file: " + ex);
            }
        }

        public static void SaveSettingsToFile()
        {
            try
            {
                // serialize JSON to a string and then write string to a file
                File.WriteAllText(@"C:/Users/jeroe/AppData/Roaming/.jeroenj/MusicPlayer/settings.json", JsonConvert.SerializeObject(_settings));

                // serialize JSON directly to a file
                using (StreamWriter file = File.CreateText(@"C:/Users/jeroe/AppData/Roaming/.jeroenj/MusicPlayer/settings.json"))
                {
                    JsonSerializer serializer = new();
                    serializer.Serialize(file, _settings);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: Could not save settings to file: " + ex);
            }
        }
    }
}
