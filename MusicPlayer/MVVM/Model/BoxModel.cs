using MusicPlayer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicPlayer.MVVM.Model
{
    internal class BoxModel : ObservableObject
    {
        // Visiblity
        private Visibility _visibility;

        public Visibility Visibility
        {
            get => _visibility;
            set { _visibility = value; OnPropertyChanged(); }
        }

        public BoxModel()
        {
            Visibility = Visibility.Collapsed;
        }
    }
}
