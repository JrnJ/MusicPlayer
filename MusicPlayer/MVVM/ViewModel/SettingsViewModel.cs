using Microsoft.UI;
using MusicPlayer.Classes;
using MusicPlayer.Core;
using MusicPlayer.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        public RelayCommand OpenInFileExplorerCommand { get; set; }

        public SettingsViewModel()
        {
            AddFolderCommand = new(o =>
            {
                AddMusicFolder();
            });

            RemoveFolderCommand = new(o =>
            {
                // Create ConfirmBox
                Global.ConfirmBox = new()
                {
                    Title = "Remove Folder?",
                    Description = $"Are you sure you want to remove {Global.AppSettinggs.MusicFolders.FirstOrDefault(x => x.Id == (int)o).Path}?",
                    ConfirmText = "Cancel",
                    CancelText = "Delete",
                    Visibility = Visibility.Visible,

                    ConfirmCommand = new(a =>
                    {
                        Global.PopupVisibility = Visibility.Collapsed;
                        Global.ConfirmBox.Visibility = Visibility.Collapsed;
                    }),
                    CancelCommand = new(a =>
                    {
                        Global.PopupVisibility = Visibility.Collapsed;
                        Global.ConfirmBox.Visibility = Visibility.Collapsed;

                        // Remove Folder
                        RemoveMusicFolder((int)o);
                    })
                };

                // Show ConfirmBox
                Global.PopupVisibility = Visibility.Visible;
            });

            OpenInFileExplorerCommand = new(o =>
            {
                //Process.Start($@"{o}");
                Process.Start(new ProcessStartInfo()
                { 
                    FileName = $@"{o}",
                    UseShellExecute = true
                });
            });
        }

        public async void AddMusicFolder()
        {
            StorageFolder folder = await AppWindowExtensions.OpenFolderPicker();

            if (folder != null)
            {
                Global.AppSettinggs.AddMusicFolder(folder.Path);

                // Add to Cache
                IReadOnlyList<StorageFile> files = await folder.GetFilesAsync();

                foreach (StorageFile file in files)
                {
                    if (HelperMethods.IsMusicFile(file.Path))
                    {
                        // Add to MyMusic
                        Global.MyMusic.AddSong(file);
                        await Global.MyMusic.Songs[^1].Init(file);

                        // Add to Cache
                        AlbumSongModel song = Global.MyMusic.Songs[^1];
                        Global.AppSettinggs.CachedSongs.Add(new()
                        {
                            Id = song.Id,
                            Title = song.MusicProperties.Title,
                            Artist = song.MusicProperties.Artist,
                            Path = song.Path
                        });
                    }
                }

                // Save Cache
                await FileHandler<ObservableCollection<CachedSong>>.SaveJSON(Global.CachedSongsFilePath, Global.AppSettinggs.CachedSongs);
            }
        }

        public async void RemoveMusicFolder(int folderId)
        {
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(Global.AppSettinggs.MusicFolders.FirstOrDefault(x => x.Id == folderId).Path);
            IReadOnlyList<StorageFile> files = await folder.GetFilesAsync();

            foreach (StorageFile file in files)
            {
                if (HelperMethods.IsMusicFile(file.Path))
                {
                    // TODO: this will return 0 for anything not found, change it bc it will delete different songs
                    int songId = Global.MyMusic.Songs.FirstOrDefault(x => x.Path == file.Path).Id;

                    Global.MyMusic.RemoveSong(songId);
                    Global.AppSettinggs.CachedSongs.Remove(Global.AppSettinggs.CachedSongs.FirstOrDefault(x => x.Id == songId));
                }
            }

            Global.AppSettinggs.RemoveMusicFolder(folderId);

            // Save Cache
            await FileHandler<ObservableCollection<CachedSong>>.SaveJSON(Global.CachedSongsFilePath, Global.AppSettinggs.CachedSongs);
        }
    }
}
