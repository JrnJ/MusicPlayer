using MusicPlayer.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MusicPlayer.Classes
{
    internal class GetIndexConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<AlbumSongModel> list = (ObservableCollection<AlbumSongModel>)values[1];
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Id == (int)values[0])
                    return (list.IndexOf(list[i]) + 1).ToString();
            }

            return "0";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
