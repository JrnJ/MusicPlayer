using MusicPlayer.Core;
using MusicPlayer.Shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.MVVM.Model
{
    internal class SettingsModel : ObservableObject
    {
        private int _id;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }


        private double _volume;

        public double Volume
        {
            get => _volume;
            set { _volume = value; OnPropertyChanged(); }
        }

        private ObservableCollection<SongsFolderModel> _songsFolders = new();

        public ObservableCollection<SongsFolderModel> SongsFolders
        {
            get => _songsFolders;
            set { _songsFolders = value; OnPropertyChanged(); }
        }

        public SettingsModel() { }

        public SettingsModel(Settings settings, ObservableCollection<SongsFolderModel> allSongsFolders)
        {
            _id = settings.Id;
            _volume = settings.Volume;

            // Reference based!
            foreach (SettingsSongsFolder settingsSongsFolder in settings.SongsFolders)
            {
                foreach (SongsFolderModel songsFolder in allSongsFolders)
                {
                    if (settingsSongsFolder.SongsFolderId == songsFolder.Id)
                    {
                        SongsFolders.Add(songsFolder);
                        break;
                    }
                }
            }
        }
    }
}
