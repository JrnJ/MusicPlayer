using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Core.Searching
{
    internal class SearchBarOption : ObservableObject
    {
        private string _header;

        public string Header
        {
            get { return _header; }
            set { _header = value; OnPropertyChanged(); }
        }

        public List<string> Keywords { get; set; } = new();

        public RelayCommand Command { get; set; }

        public event EventHandler<string>? BeforeSearchOptionShow;

        public void OnBeforeSearchOptionShow(string parameter)
        {
            BeforeSearchOptionShow?.Invoke(this, parameter);
        }

        //
        public void AddKeyword(string keyword) => Keywords.Add(keyword);

    }
}
