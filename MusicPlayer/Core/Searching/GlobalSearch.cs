using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicPlayer.Core.Searching
{
    internal class GlobalSearch : ObservableObject
    {
        private string _searchBarInput;

        public string SearchBarInput
        {
            get => _searchBarInput;
            set
            {
                _searchBarInput = value;
                OnPropertyChanged();

                //
                SearchBarInputChanged();
            }
        }

        private Visibility _searchPopupVisibility;

        public Visibility SearchPopupVisibility
        {
            get { return _searchPopupVisibility; }
            set { _searchPopupVisibility = value; OnPropertyChanged(); }
        }

        private ObservableCollection<SearchBarItemModel> _searchBarItems;

        public ObservableCollection<SearchBarItemModel> SearchBarItems
        {
            get { return _searchBarItems; }
            set { _searchBarItems = value; OnPropertyChanged(); }
        }

        private ObservableCollection<SearchBarOption> _searchBarOptions;

        public ObservableCollection<SearchBarOption> SearchBarOptions
        {
            get { return _searchBarOptions; }
            set { _searchBarOptions = value; }
        }

        public GlobalSearch()
        {
            _searchPopupVisibility = Visibility.Collapsed;
            _searchBarOptions = new();

            _searchBarItems = new();
        }

        public void AddSearchBarOption(SearchBarOption searchBarOption)
        {
            SearchBarOptions.Add(searchBarOption);
        }

        private void SearchBarInputChanged()
        {
            string input = SearchBarInput.ToLower();

            if (input.Length == 0)
            {
                SearchBarItems.Clear();
                SearchPopupVisibility = Visibility.Collapsed;
                return;
            }
            else
            {
                SearchPopupVisibility = Visibility.Visible;
            }

            // Change this later
            SearchBarItems.Clear();

            foreach (SearchBarOption option in _searchBarOptions)
            {
                string? parameter = IsValid(input, option);

                if (parameter != null)
                {
                    // Call Event
                    option.OnBeforeSearchOptionShow(parameter);

                    SearchBarItems.Add(
                        new SearchBarItemModel()
                        {
                            Header = option.Header,
                            Description = "",
                            Command = option.Command,
                            CommandParameter = parameter
                        });
                }
            }
        }

        private static string? IsValid(string input, SearchBarOption option)
        {
            string[] inputs = input.Split(' ');
            if (inputs.Length < 2) return null;

            foreach (string keyword in option.Keywords)
            {
                for (int i = 0; i < inputs.Length; i++)
                {
                    if (keyword.Contains(inputs[i]))
                    {
                        return inputs[1];
                    }
                }
            }

            return null;
        }
    }
}
