using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MusicPlayer.Classes
{
    public static class FileHandler
    {
        #region READ
        public static ObservableCollection<Playlist> GetPlaylistsLocation()
        {
            try
            {
                string dik = $"{Environment.SpecialFolder.UserProfile}/AppData/Roaming/.jeroenj/MusicPlayer";
                string[] folderContent = Directory.GetFiles($"{Environment.SpecialFolder.ApplicationData}");

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion READ

        #region WRITE
        public static bool SavePlaylistsLocation()
        {
            try
            {
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SaveSettings()
        {
            try
            {
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion WRITE
    }
}
