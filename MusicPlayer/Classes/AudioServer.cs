using MusicPlayer.Core;
using MusicPlayer.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Classes
{
    internal class AudioServer : ObservableObject
    {
        // Put this in a config file and in the .gitignore whenever this goes live
        public static int Port => 18600;
        public static string Ip => "127.0.0.1";
        public static string MusicFolderPath => "D:/Music";

        public enum TCPMessage
        {
            Play = 0,
            Resume = 1,
            Pause = 2
        }

        private TcpClient _client;

        public TcpClient Client
        {
            get => _client; 
            set { _client = value; OnPropertyChanged(); }
        }

        public AudioServer()
        {
            Client = new TcpClient();
        }

        public void Connect()
        {
            if (!Client.Connected)
            {
                try
                {
                    Client.Connect(Ip, Port);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex);
                }
            } 
            else
            {
                // Client already connected
            }
        }

        public void Disconnect()
        {
            if (Client.Connected)
            {
                // Close Connection
                Client.Close();
                Client = new TcpClient();
            }
            else
            {
                // No Connection
            }
        }

        public void Play(AlbumSongModel song)
        {
            try
            {
                if (Client.Connected)
                {
                    string path = song.Path;
                    path = path.Replace(MusicFolderPath, "");


                    SendMessage($"{(int)TCPMessage.Play}>" + path);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex);
            }
        }

        public void Resume()
        {
            try
            {
                if (Client.Connected)
                {
                    SendMessage($"{(int)TCPMessage.Resume}>");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex);
            }
        }

        public void Pause()
        {
            try
            {
                if (Client.Connected)
                {
                    SendMessage($"{(int)TCPMessage.Pause}>");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex);
            }
        }
        
        private void SendMessage(string message)
        {
            try
            {
                // Create a stream
                NetworkStream stream = Client.GetStream();

                // String to a byteArray
                byte[] bytesToSend = Encoding.ASCII.GetBytes(message);

                // Send through NetworkStream
                stream.Write(bytesToSend, 0, bytesToSend.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex);
            }
        }
    }
}
