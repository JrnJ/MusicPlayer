using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using MusicPlayer.Classes;
using MusicPlayer.Core;
using MusicPlayer.Database;
using MusicPlayer.MVVM.Model;
using MusicPlayer.Shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
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

        private ScrollViewer _scrollViewer;

        public ScrollViewer ScrollViewer
        {
            get { return _scrollViewer; }
            set { _scrollViewer = value; OnPropertyChanged(); }
        }

        public SettingsViewModel()
        {
            AddFolderCommand = new(o =>
            {
                AddSongsFolder();
            });

            RemoveFolderCommand = new(o =>
            {
                // Create ConfirmBox
                Global.ConfirmBox = new()
                {
                    Title = "Remove Folder?",
                    Description = $"Are you sure you want to remove {Global.Settings.SongsFolders.FirstOrDefault(x => x.Id == (int)o).Path}?",
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
                        Global.RemoveSongsFolder((int)o);
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

        // TODO2
        public async void AddSongsFolder()
        {
            StorageFolder folder = await AppWindowExtensions.OpenFolderPicker();
            if (folder == null) return;

            Global.AddSongsFolder(folder);
        }
    }
}
