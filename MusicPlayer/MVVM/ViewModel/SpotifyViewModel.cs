using ABI.System;
using MusicPlayer.Classes;
using MusicPlayer.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicPlayer.MVVM.ViewModel
{
    internal class SpotifyViewModel
    {
        // <GlobalViewModel> //
        public GlobalViewModel Global { get; } = GlobalViewModel.Instance;
        // </GlobalViewModel> //

        public RelayCommand StartServerCommand { get; set; }

        public RelayCommand ConnectCommand { get; set; }

        public RelayCommand DisconnectCommand { get; set; }

        // <Spotify Stuff> //

        public string ClientId = "6b4917d2b18d4a8c8384a9658655af77";
        public string ClientSecret = "997e57ed686b4da8b476d8eee5f8b7d6";
        public string RedirectUrl = "http://localhost:8888/callback";

        public string Scope = "user-read-private user-read-email";

        public string StateKey = "spotify_auth_state";

        private SpotifyServer _spotifyServer;

        public SpotifyServer SpotifyServer
        {
            get { return _spotifyServer; }
            set { _spotifyServer = value; }
        }


        private string GenerateRandomString(int length)
        {
            string text = "";
            var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new();

            for (var i = 0; i < length; i++)
            {
                text += possible[random.Next(0, length)];
            }

            return text;
        }

        // </Spotify Stuff> //

        public SpotifyViewModel()
        {
            StartServerCommand = new(o =>
            {
                SpotifyServer = new();
                SpotifyServer.CreateServer();
            });

            ConnectCommand = new(o =>
            {
                // // //


                string state = GenerateRandomString(16);

                // &state=4GqJgzNCQohzuUKv
                string url = "https://accounts.spotify.com/en/authorize?response_type=code" +
                    "&client_id=" + ClientId +
                    "&scope=" + Scope +
                    "&redirect_uri=" + RedirectUrl +
                    "&state=" + state;

                url = url.Replace("&", "^&");
                // Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            });

            DisconnectCommand = new(o =>
            {

            });
        }
    }
}
