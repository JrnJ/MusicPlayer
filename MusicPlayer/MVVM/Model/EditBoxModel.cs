using MusicPlayer.Classes;
using MusicPlayer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MusicPlayer.MVVM.Model
{
    internal class EditBoxModel : BoxModel
    {
        // Properties
        private Playlist _playlist;

        public Playlist Playlist
        {
            get { return _playlist; }
            set { _playlist = value; OnPropertyChanged(); }
        }

        // Commands
        public RelayCommand ConfirmCommand { get; set; }

        public RelayCommand CancelCommand { get; set; }
    }
}
