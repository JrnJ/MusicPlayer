using Microsoft.UI;
using MusicPlayer.Core;
using MusicPlayer.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace MusicPlayer.MVVM.ViewModel
{
    internal class SettingsViewModel : ObservableObject
    {
        // <GlobalViewModel> //
        public GlobalViewModel Global { get; } = GlobalViewModel.Instance;
        // </GlobalViewModel> //

        public RelayCommand AddFolderCommand { get; set; }

        public RelayCommand RemoveFolderCommand { get; set; }

        public SettingsViewModel()
        {
            AddFolderCommand = new(o =>
            {
                AddMusicFolder();
            });

            RemoveFolderCommand = new(o =>
            {
                Global.AppSettinggs.RemoveMusicFolder((int)o);
            });
        }

        public async void AddMusicFolder()
        {
            StorageFolder folder = await AppWindowExtensions.OpenFolderPicker();
            Global.AppSettinggs.AddMusicFolder(folder.Path);
        }
    }
}
