using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using Microsoft.UI.Xaml;
using System.Windows.Media;

namespace MusicPlayer.Classes
{
    internal class SpotifyServer
    {
        public static int Port => 8888;
        public static string Ip => "127.0.0.1";

        private TcpListener _server;

        public TcpListener Server
        {
            get { return _server; }
            set { _server = value; }
        }

        private Thread _serverThread;

        public Thread ServerThread
        {
            get { return _serverThread; }
            set { _serverThread = value; }
        }

        public SpotifyServer()
        {
            
        }

        public void CreateServer()
        {
            Server = new(IPAddress.Parse(Ip), Port);
            Server.Start();

            // Create a thread
            ServerThread = new Thread(new ThreadStart(ServerThreadFunction));
            ServerThread.Start();
            // https://developer.mozilla.org/en-US/docs/Web/API/WebSockets_API/Writing_WebSocket_server
        }

        private void ServerThreadFunction()
        {
            // Buffer for reading data
            Byte[] bytes = new Byte[256];
            String data = null;

            try
            {
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    using TcpClient client = Server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        // MessageBox.Show("Received: {0}", data);

                        // Process the data sent by the client.
                        data = data.ToUpper();

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes("hello");

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);

                        // MessageBox.Show("Sent: {0}", data);

                        if (data.Contains("login"))
                        {
                            MessageBox.Show("UwU");
                        }
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error");
            }
        }
    }
}
