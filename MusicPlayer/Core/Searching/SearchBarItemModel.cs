using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Core.Searching
{
    internal class SearchBarItemModel : ObservableObject
    {
        private string _header;

        public string Header
        {
            get { return _header; }
            set { _header = value; OnPropertyChanged(); }
        }

        private string _description;

        public string Description
        {
            get { return _description; }
            set { _description = value; OnPropertyChanged(); }
        }

        // Icon
        private RelayCommand _command;

        public RelayCommand Command
        {
            get { return _command; }
            set { _command = value; }
        }

        private object _commandParameter;

        public object CommandParameter
        {
            get { return _commandParameter; }
            set { _commandParameter = value; }
        }
    }
}
