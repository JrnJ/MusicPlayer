using MusicPlayer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.MVVM.ViewModel
{
    internal class DiscordViewModel
    {
        // <GlobalViewModel> //
        public GlobalViewModel Global { get; } = GlobalViewModel.Instance;
        // </GlobalViewModel> //

        public RelayCommand StartServerCommand { get; set; }

        public RelayCommand ConnectCommand { get; set; }

        public RelayCommand DisconnectCommand { get; set; }

        public DiscordViewModel()
        {
            StartServerCommand = new(o =>
            {
                Global.AudioPlayer.StartServer();
            });

            ConnectCommand = new(o =>
            {
                Global.AudioPlayer.AudioServer.Connect();
            });

            DisconnectCommand = new(o =>
            {
                Global.AudioPlayer.AudioServer.Disconnect();
            });
        }
    }
}
