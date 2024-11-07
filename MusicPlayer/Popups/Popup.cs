using MusicPlayer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicPlayer.Popups
{
    internal class Popup : ObservableObject
    {
        private Visibility _visible = Visibility.Collapsed;

        public Visibility Visible
        {
            get { return _visible; }
            set { _visible = value; OnPropertyChanged(); }
        }
    }
}
