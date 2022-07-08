using MusicPlayer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicPlayer.MVVM.Model
{
    internal class MessageBoxModel : ObservableObject
    {
        // Visiblity
        private Visibility _visibility;

        public Visibility Visibility
        {
            get => _visibility;
            set { _visibility = value; OnPropertyChanged(); }
        }

        // Properties
        private string _title;

        public string Title
        {
            get => _title; 
            set { _title = value; OnPropertyChanged(); }
        }

        private string _description;

        public string Description
        {
            get => _description; 
            set { _description = value; OnPropertyChanged(); }
        }

        private string _confirmText;

        public string ConfirmText
        {
            get => _confirmText; 
            set { _confirmText = value; OnPropertyChanged(); }
        }

        private string _cancelText;

        public string CancelText
        {
            get => _cancelText; 
            set { _cancelText = value; OnPropertyChanged(); }
        }

        public MessageBoxModel()
        {
            Visibility = Visibility.Collapsed;

            Title = "Title";
            Description = "Description";
            ConfirmText = "Yes";
            CancelText = "No";
        }

        // Commands
        public RelayCommand ConfirmCommand { get; set; }

        public RelayCommand CancelCommand { get; set; }
    }
}
