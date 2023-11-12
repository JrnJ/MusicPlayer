using MusicPlayer.Core;

namespace MusicPlayer.MVVM.Model
{
    internal class EditBoxModel : BoxModel
    {
        // Properties
        private PlaylistSongsModel _playlist;

        public PlaylistSongsModel Playlist
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

        public RelayCommand ChangeImageCommand { get; set; }
    }
}
