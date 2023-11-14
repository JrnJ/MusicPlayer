//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Text;
//using System.Text.Json.Serialization;
//using System.Threading.Tasks;
//using MusicPlayer.Classes;
//using MusicPlayer.Core;
//using Windows.Media.Playlists;
//using Windows.Storage;

//namespace MusicPlayer.MVVM.Model
//{
//    internal class PlaylistSongsModel : ObservableObject
//    {
//        private int _id;

//        [JsonPropertyName("Id")]
//        public int Id
//        {
//            get { return _id; }
//            set { _id = value; OnPropertyChanged(); }
//        }

//        private string _name;

//        [JsonPropertyName("Name")]
//        public string Name
//        {
//            get { return _name; }
//            set { _name = value; OnPropertyChanged(); }
//        }

//        private string _description;

//        [JsonPropertyName("Description")]
//        public string Description
//        {
//            get { return _description; }
//            set { _description = value; OnPropertyChanged(); }
//        }

//        private string _imagePath;

//        [JsonPropertyName("ImagePath")]
//        public string ImagePath
//        {
//            get { return _imagePath; }
//            set { _imagePath = value; OnPropertyChanged(); }
//        }

//        private ObservableCollection<AlbumSongModel> _songs;

//        [JsonPropertyName("Songs")]
//        public ObservableCollection<AlbumSongModel> Songs
//        {
//            get { return _songs; }
//            set 
//            { 
//                _songs = value; 
//                OnPropertyChanged();
//                OnPropertyChanged(PlaylistDuration);
//            }
//        }

//        private bool _isSelected;

//        [JsonIgnore]
//        public bool IsSelected
//        {
//            get { return _isSelected; }
//            set { _isSelected = value; OnPropertyChanged(); }
//        }

//        public string PlaylistDuration
//        {
//            get
//            {
//                if (Songs == null)
//                    return HelperMethods.MsToTime(0);

//                double ms = 0.0;

//                for (int i = 0; i < Songs.Count; i++)
//                {
//                    if (Songs[i].MusicProperties != null)
//                    {
//                        ms += Songs[i].MusicProperties.Duration.TotalMilliseconds;
//                    }
//                }

//                return HelperMethods.MsToTime(ms);
//            }
//        }

//        public PlaylistSongsModel()
//        {
//            Songs = new();
//            Name = "New Playlist";
//            Description = "Description";
//        }

//        // TODO: RN, this is a mess fix it
//        public AlbumSongModel AddSong(StorageFile storageFile, int id = 0)
//        {
//            // Check if song already exists (and create an id meanwhile)
//            if (id == 0)
//            {
//                for (int i = 0; i < Songs.Count; i++)
//                {
//                    if (Songs[i].Path == storageFile.Path)
//                        return null;

//                    if (Songs[i].Id + 1 > id)
//                        id = Songs[i].Id + 1;
//                }
//            }
//            else
//            {
//                for (int i = 0; i < Songs.Count; i++)
//                {
//                    if (Songs[i].Path == storageFile.Path)
//                        return null;
//                }
//            }

//            AlbumSongModel newSong = new()
//            {
//                Id = id,
//                Path = storageFile.Path
//            };
//            Songs.Add(newSong);
//            OnPropertyChanged(PlaylistDuration);

//            //if (saveSettings)
//            //    SaveSettings();

//            return newSong;
//        }

//        public bool RemoveSong(int id)
//        {
//            try
//            {
//                Songs.Remove(Songs.Where(x => x.Id == id).FirstOrDefault());
//                OnPropertyChanged(PlaylistDuration);
//                //if (saveSettings)
//                //    SaveSettings();

//                return true;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("ERROR: " + ex);
//                return false;
//            }
//        }
//    }
//}
