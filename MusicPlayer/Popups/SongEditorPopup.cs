using MusicPlayer.Core;
using MusicPlayer.Database;
using MusicPlayer.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicPlayer.Popups
{
    internal class SongEditorPopup : Popup
    {
		private SongModel? _current;

		public void StartEdit(SongModel song)
		{
			
		}

		public void StopEdit()
		{
			if (_current == null)
			{
				return;
			}

            
        }
    }
}
