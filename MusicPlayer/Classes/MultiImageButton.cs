using MusicPlayer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Classes
{
    internal class MultiImageEntry
    {
        public string Key { get; }
        public string Source { get; }

        public MultiImageEntry(string key, string name)
        {
            Key = key;
            Source = name;
        }
    }

    internal class MultiImageButton : ObservableObject
    {
		private string _imageSource;

		public string ImageSource
		{
			get { return _imageSource; }
			set { _imageSource = value; OnPropertyChanged(); }
		}

        private Dictionary<string, string> _entries = [];

        public MultiImageButton(List<MultiImageEntry> entries)
        {
            foreach (MultiImageEntry entry in entries)
            {
                _entries.Add(entry.Key, entry.Source);
            }
            _imageSource = entries[0].Source;
        }

        public void Alter(string key)
        {
            if (_entries.TryGetValue(key, out string? value))
            {
                ImageSource = value;
            }
        }
    }
}
