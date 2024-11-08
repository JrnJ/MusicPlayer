﻿using MusicPlayer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicPlayer.MVVM.Model
{
    internal class MessageBoxModel : BoxModel
    {
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
            _title = "Title";
            _description = "Description";
            _confirmText = "Yes";
            _cancelText = "No";
        }

        // Commands
        public RelayCommand ConfirmCommand { get; set; }

        public RelayCommand CancelCommand { get; set; }
    }
}
