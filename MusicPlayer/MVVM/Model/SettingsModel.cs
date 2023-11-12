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
        private double _volume;

        public double Volume
        {
            get => _volume;
            set { _volume = value; OnPropertyChanged(); }
        }

        private ObservableCollection<SongsFolderModel> _songsFolders;

        public ObservableCollection<SongsFolderModel> SongsFolders
        {
            get => _songsFolders;
            set { _songsFolders = value; OnPropertyChanged(); }
        }

        public SettingsModel() { }

        public SettingsModel(Settings settings, ObservableCollection<SongsFolderModel> allSongsFolders)
        {
            _volume = settings.Volume;

            _songsFolders = new();

            // Reference based!
            foreach (SettingsSongsFolder settingsSongsFolder in settings.SongsFolders)
            {
                foreach (SongsFolderModel songsFolder in allSongsFolders)
                {
                    if (settingsSongsFolder.SettingsId == songsFolder.Id)
                    {
                        SongsFolders.Add(songsFolder);
                        break;
                    }
                }
            }
        }
    }
}
