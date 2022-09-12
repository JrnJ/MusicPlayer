using MusicPlayer.Core;
using MusicPlayer.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Windows.Media;
using Windows.Media.Playback;

namespace MusicPlayer.Classes
{
    internal enum AudioPlayerState
    {
        Closed = 0,
        Opening = 1,
        Buffering = 2,
        Playing = 3,
        Paused = 4,
        Stopped = 5
    }

    internal class AudioPlayer : ObservableObject
    {
        // Unsure about accesibility here
        public SystemMediaTransportControls MediaControls { get; set; }

        public MediaPlayer MediaPlayer { get; set; }

        // Stoopid props
        private AlbumSongModel _currentSong;

        public AlbumSongModel CurrentSong
        {
            get { return _currentSong; }
            set { _currentSong = value; OnPropertyChanged(); }
        }

        public double Volume
        {
            get => MediaPlayer.Volume;
            set => MediaPlayer.Volume = value;
        }

        public TimeSpan Position
        {
            get => MediaPlayer.Position;
            set => MediaPlayer.Position = value;
        }

        public bool IsLoopingEnabled
        {
            get => MediaPlayer.IsLoopingEnabled;
            set => MediaPlayer.IsLoopingEnabled = value;
        }

        public AudioPlayerState CurrentState => (AudioPlayerState)MediaPlayer.CurrentState;

        // Discord Integration
        private AudioServer _audioServer;

        public AudioServer AudioServer
        {
            get { return _audioServer; }
            set { _audioServer = value; OnPropertyChanged(); }
        }

        public DispatcherTimer Timer { get; private set; }

        // Constructor
        public AudioPlayer()
        {
            // Configuration
            ConfigureMediaPlayer();

            // Timer
            Timer = new()
            {
                Interval = new TimeSpan(0, 0, 0, 0, 250)
            };
        }

        #region Private
        private void ConfigureMediaPlayer()
        {
            // Mediaplayer
            MediaPlayer = new();

            MediaPlayer.SystemMediaTransportControls.IsPlayEnabled = false;
            MediaPlayer.SystemMediaTransportControls.IsPauseEnabled = false;
            MediaPlayer.SystemMediaTransportControls.IsStopEnabled = false;

            // Properties
            MediaPlayer.CommandManager.IsEnabled = true;

            // Events
            MediaPlayer.MediaOpened += MediaPlayerMediaOpened;
            MediaPlayer.MediaEnded += MediaPlayerMediaEnded;

            // SMTC
            //MediaPlayer.SystemMediaTransportControls.IsNextEnabled = true;
            //MediaPlayer.SystemMediaTransportControls.IsPreviousEnabled = true; 
            // https://github.com/microsoft/Windows-universal-samples/blob/dev/Samples/SystemMediaTransportControls/cs/Scenario1.xaml.cs
        }

        private void CommandManager_NextReceived(MediaPlaybackCommandManager sender, MediaPlaybackCommandManagerNextReceivedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void ConfigureAudioServer()
        {
            AudioServer = new AudioServer();
        }

        #region MediaPlayerEvents
        private void MediaPlayerMediaOpened(Windows.Media.Playback.MediaPlayer sender, object args)
        {
            UpdateSMTCDisplay();
        }

        private void MediaPlayerMediaEnded(Windows.Media.Playback.MediaPlayer sender, object args)
        {
            // Call to MainWindow thread
            //Dispatcher.Invoke(() => { MediaEnded(); });
        }
        #endregion MediaPlayerEvents

        // This is a joke and doesnt even work
        private void UpdateSMTCDisplay()
        {
            //if (CurrentSong != null && 1 == 2)
            //{
            //    SystemMediaTransportControls smtc = MediaPlayer.SystemMediaTransportControls;
            //    //smtc.DisplayUpdater.ClearAll();
            //    //smtc.IsNextEnabled = true;
            //    //smtc.IsPreviousEnabled = true;
            //    smtc.DisplayUpdater.Type = MediaPlaybackType.Video;
            //    smtc.DisplayUpdater.VideoProperties.Title = CurrentSong.MusicProperties.Title;
            //    smtc.DisplayUpdater.VideoProperties.Subtitle = CurrentSong.MusicProperties.Artist;
            //    smtc.DisplayUpdater.Thumbnail = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromUri(new Uri("C:/Users/jeroe/Downloads/SongImagePlaceholder.png"));
            //    smtc.IsNextEnabled = true;
            //    smtc.IsPreviousEnabled = true;

            //    // Update the system media transport controls
            //    smtc.DisplayUpdater.Update();

            //    // LateUpdate??
            //    //MediaPlayer.SystemMediaTransportControls.IsNextEnabled = true;
            //    //MediaPlayer.SystemMediaTransportControls.IsPreviousEnabled = true;
            //}
            //else
            //{
            //    Console.WriteLine("Could not update SMTC display because song is null");
            //}

            if (CurrentSong != null)
            {
                // Set Other
                //MediaPlayer.SystemMediaTransportControls.IsNextEnabled = true;
                //MediaPlayer.SystemMediaTransportControls.IsPreviousEnabled = true;

                // Set DisplayUpdater properties
                MediaPlayer.SystemMediaTransportControls.DisplayUpdater.Type = MediaPlaybackType.Video;
                MediaPlayer.SystemMediaTransportControls.DisplayUpdater.VideoProperties.Title = CurrentSong.MusicProperties.Title;
                MediaPlayer.SystemMediaTransportControls.DisplayUpdater.VideoProperties.Subtitle = CurrentSong.MusicProperties.Artist;
                MediaPlayer.SystemMediaTransportControls.DisplayUpdater.Thumbnail = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromUri(new Uri("C:/Users/jeroe/Downloads/SongImagePlaceholder.png"));

                // Update Display
                MediaPlayer.SystemMediaTransportControls.DisplayUpdater.Update();
            }
        }

        #endregion Private

        public void OpenMedia(AlbumSongModel song)
        {
            if (CurrentSong != null)
            {
                CurrentSong.IsPlaying = false;
            }

            CurrentSong = song;
            CurrentSong.IsPlaying = true;
            Pause();

            MediaPlayer.SetUriSource(new Uri(song.Path));

            // Play Song
            //Play();

            // Notify Server
            if (AudioServer != null)
            {
                AudioServer.Play(song);
            }
        }

        /// <summary>
        /// Starts the AudioServer for the Discord
        /// </summary>
        public void StartServer()
        {
            ConfigureAudioServer();
        }

        // Maybe resume instead
        /// <summary>
        /// Plays the Audio
        /// </summary>
        public void Play()
        {
            MediaPlayer.Play();

            Timer.Start();

            // Notify Server
            if (AudioServer != null)
            {
                AudioServer.Resume();
            }
        }

        /// <summary>
        /// Pauses the Audio
        /// </summary>
        public void Pause()
        {
            MediaPlayer.Pause();

            Timer.Stop();

            // Notify Server
            if (AudioServer != null)
            {
                AudioServer.Pause();
            }
        }

        /// <summary>
        /// Pause or Play
        /// </summary>
        public void PausePlay()
        {
            switch (CurrentState)
            {
                case AudioPlayerState.Closed:
                    break;
                case AudioPlayerState.Opening:
                    break;
                case AudioPlayerState.Buffering:
                    break;
                case AudioPlayerState.Playing:
                    Pause();
                    break;
                case AudioPlayerState.Paused:
                    Play();
                    break;
                case AudioPlayerState.Stopped:
                    break;
            }
        }

        /// <summary>
        /// Adds an amount of time in seconds to the song
        /// </summary>
        /// <param name="amount">Time in seconds</param>
        public void AddTime(int amount)
        {
            Position = Position.Add(new TimeSpan(0, 0, amount));
        }

        /// <summary>
        /// Subtracts an amount of time in seconds from the song
        /// </summary>
        /// <param name="amount">Time in seconds</param>
        public void SubtractTime(int amount)
        {
            Position = Position.Subtract(new TimeSpan(0, 0, amount));
        }
    }
}
