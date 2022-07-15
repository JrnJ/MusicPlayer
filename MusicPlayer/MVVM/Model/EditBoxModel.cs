using MusicPlayer.Classes;
using MusicPlayer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

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

        //private string _name;

        //public string Name
        //{
        //    get { return _name; }
        //    set { _name = value; OnPropertyChanged(); }
        //}

        //private string _description;

        //public string Description
        //{
        //    get { return _description; }
        //    set { _description = value; OnPropertyChanged(); }
        //}

        // Commands
        public RelayCommand ConfirmCommand { get; set; }

        public RelayCommand CancelCommand { get; set; }
    }
}
