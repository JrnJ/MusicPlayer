using MusicPlayer.Classes;
using MusicPlayer.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.MVVM.Model
{
    internal class AppSettingsModel : ObservableObject
    {
        [JsonIgnore]
        public static readonly string SettingsFilePath = "C:/Users/jeroe/AppData/Roaming/.jeroenj/MusicPlayer/settings.json";

        private double _volume;

        [JsonProperty("Volume")]
        public double Volume
        {
            get { return _volume; }
            set { _volume = value; }
        }

        private ObservableCollection<MusicFolder> _musicFolders;

        [JsonProperty("MusicFolders")]
        public ObservableCollection<MusicFolder> MusicFolders
        {
            get { return _musicFolders; }
            set { _musicFolders = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Adds a new MusicFolder to the MusicFolders
        /// </summary>
        /// <param name="path">Path of folder</param>
        /// <param name="saveSettings">If settings should be saved</param>
        /// <returns>False if folder is already added</returns>
        public bool AddMusicFolder(string path, bool saveSettings = true)
        {
            // Check if folder already exists (and create an id meanwhile)
            int id = 0;
            for (int i = 0; i < MusicFolders.Count; i++)
            {
                if (MusicFolders[i].Path == path)
                    return false;

                if (MusicFolders[i].Id > id)
                    id = MusicFolders[i].Id;
            }

            MusicFolders.Add(new()
            {
                Id = id + 1,
                Path = path
            });

            if (saveSettings)
                SaveSettingsToFile();

            return true;
        }

        public bool RemoveMusicFolder(int id, bool saveSettings = true)
        {
            try
            {
                MusicFolders.Remove(MusicFolders.Where(x => x.Id == id).FirstOrDefault());
                if (saveSettings)
                    SaveSettingsToFile();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex);
                return false;
            }
        }

        public async Task GetSettingsFromFile()
        {
            AppSettingsModel appSettings = await FileHandler<AppSettingsModel>.GetJSON(SettingsFilePath);
            this.Volume = appSettings.Volume;
            this.MusicFolders = appSettings.MusicFolders;
        }

        public void SaveSettingsToFile()
        {
            // trycatch?
            // return bool?
            // something plz
            FileHandler<AppSettingsModel>.SaveJSON(SettingsFilePath, this);
        }
    }
}
