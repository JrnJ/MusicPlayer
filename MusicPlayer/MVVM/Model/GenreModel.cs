using MusicPlayer.Core;
using MusicPlayer.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.MVVM.Model
{
    internal class GenreModel : ObservableObject
    {
		private string _name;

		public string Name
		{
			get => _name;
			set { _name = value; OnPropertyChanged(); }
		}

		public GenreModel() { } 
		public GenreModel(Genre genre)
		{
			_name = genre.Name;
		}

		public Genre ToGenre()
		{
			return new()
			{
				Name = _name
			};
		}
	}
}
