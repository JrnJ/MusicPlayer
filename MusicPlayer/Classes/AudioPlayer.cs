using MusicPlayer.Core;
using MusicPlayer.Core.Searching;
using MusicPlayer.MVVM.Model;
using MusicPlayer.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
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

    internal class AudioPlayer : ObservableObject, ISearchCommandAddon
    {
        // Unsure about accesibility here
        public SystemMediaTransportControls MediaControls { get; private set; }

        public Windows.Media.Playback.MediaPlayer MediaPlayer { get; private set; }

        // Stoopid props
        private SongModel _currentSong;

        public SongModel CurrentSong
        {
            get =>_currentSong;
            set { _currentSong = value; OnPropertyChanged(); }
        }

        public double Volume
        {
            get => MediaPlayer.Volume;
            set { MediaPlayer.Volume = value; OnPropertyChanged(); }
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

            // TODO
            //ConfigureSearchOptions(GlobalViewModel.Instance.GlobalSearch);

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

            SMTC();

            //MediaPlayer.SystemMediaTransportControls.IsPlayEnabled = false;
            //MediaPlayer.SystemMediaTransportControls.IsPauseEnabled = false;
            //MediaPlayer.SystemMediaTransportControls.IsStopEnabled = false;

            //// Properties
            //MediaPlayer.CommandManager.IsEnabled = true;

            //// Events
            //MediaPlayer.MediaOpened += MediaPlayerMediaOpened;
            //MediaPlayer.MediaEnded += MediaPlayerMediaEnded;

            // SMTC
            //MediaPlayer.SystemMediaTransportControls.IsNextEnabled = true;
            //MediaPlayer.SystemMediaTransportControls.IsPreviousEnabled = true; 
            // https://github.com/microsoft/Windows-universal-samples/blob/dev/Samples/SystemMediaTransportControls/cs/Scenario1.xaml.cs
        }

        #region SMTC
        private void SMTC()
        {
            // MediaControls Event enabling
            MediaPlayer.CommandManager.IsEnabled = true;
            MediaPlayer.CommandManager.PreviousBehavior.EnablingRule = MediaCommandEnablingRule.Always;
            MediaPlayer.CommandManager.NextBehavior.EnablingRule = MediaCommandEnablingRule.Always;
            MediaPlayer.CommandManager.PlayBehavior.EnablingRule = MediaCommandEnablingRule.Always;
            MediaPlayer.CommandManager.PauseBehavior.EnablingRule = MediaCommandEnablingRule.Always;

            MediaPlayer.SystemMediaTransportControls.IsPlayEnabled = true;
            MediaPlayer.SystemMediaTransportControls.IsPauseEnabled = true;
            MediaPlayer.SystemMediaTransportControls.IsStopEnabled = true;

            // SystemMediaTransportControls Visual
            MediaPlayer.SystemMediaTransportControls.IsEnabled = true;
            MediaPlayer.MediaOpened += MediaPlayerMediaOpened;

            // Catch MediaControl clicks
            MediaPlayer.SystemMediaTransportControls.ButtonPressed += SystemMediaTransportControls_ButtonPressed;
        }

        private void MediaPlayerMediaOpened(Windows.Media.Playback.MediaPlayer sender, object args)
        {
            UpdateSMTCDisplay();
        }

        private void SystemMediaTransportControls_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                case SystemMediaTransportControlsButton.Pause:
                    // PausePlay();
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    
                    break;
                case SystemMediaTransportControlsButton.Next:
                    break;
            }
        }

        private void UpdateSMTCDisplay()
        {
            // Text
            MediaPlayer.SystemMediaTransportControls.DisplayUpdater.Type = MediaPlaybackType.Video;
            MediaPlayer.SystemMediaTransportControls.DisplayUpdater.VideoProperties.Title = CurrentSong.Title;
            MediaPlayer.SystemMediaTransportControls.DisplayUpdater.VideoProperties.Subtitle = CurrentSong.ArtistsText;

            // Image
            if (CurrentSong.Image != null)
            {
                // TODO: async | CopyToAsync
                MemoryStream memStream = new();
                CurrentSong.Image.StreamSource.CopyTo(memStream);

                MediaPlayer.SystemMediaTransportControls.DisplayUpdater.Thumbnail =
                    Windows.Storage.Streams.RandomAccessStreamReference.CreateFromStream(memStream.AsRandomAccessStream());
            }
            else
            {
                MediaPlayer.SystemMediaTransportControls.DisplayUpdater.Thumbnail = 
                    Windows.Storage.Streams.RandomAccessStreamReference.CreateFromUri(new Uri("C:/Users/jeroe/Downloads/SongImagePlaceholder.png"));
            }

            MediaPlayer.SystemMediaTransportControls.DisplayUpdater.Update();
        }
        #endregion SMTC

        //private void CommandManager_NextReceived(MediaPlaybackCommandManager sender, MediaPlaybackCommandManagerNextReceivedEventArgs args)
        //{
        //    throw new NotImplementedException();
        //}

        private void ConfigureAudioServer()
        {
            AudioServer = new AudioServer();
        }

        //#region MediaPlayerEvents
        //private void MediaPlayerMediaOpened(Windows.Media.Playback.MediaPlayer sender, object args)
        //{
        //    UpdateSMTCDisplay();
        //}

        //private void MediaPlayerMediaEnded(Windows.Media.Playback.MediaPlayer sender, object args)
        //{
        //    // Call to MainWindow thread
        //    //Dispatcher.Invoke(() => { MediaEnded(); });
        //}
        //#endregion MediaPlayerEvents

        //// This is a joke and doesnt even work
        //private void UpdateSMTCDisplay()
        //{
        //    //if (CurrentSong != null && 1 == 2)
        //    //{
        //    //    SystemMediaTransportControls smtc = MediaPlayer.SystemMediaTransportControls;
        //    //    //smtc.DisplayUpdater.ClearAll();
        //    //    //smtc.IsNextEnabled = true;
        //    //    //smtc.IsPreviousEnabled = true;
        //    //    smtc.DisplayUpdater.Type = MediaPlaybackType.Video;
        //    //    smtc.DisplayUpdater.VideoProperties.Title = CurrentSong.MusicProperties.Title;
        //    //    smtc.DisplayUpdater.VideoProperties.Subtitle = CurrentSong.MusicProperties.Artist;
        //    //    smtc.DisplayUpdater.Thumbnail = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromUri(new Uri("C:/Users/jeroe/Downloads/SongImagePlaceholder.png"));
        //    //    smtc.IsNextEnabled = true;
        //    //    smtc.IsPreviousEnabled = true;

        //    //    // Update the system media transport controls
        //    //    smtc.DisplayUpdater.Update();

        //    //    // LateUpdate??
        //    //    //MediaPlayer.SystemMediaTransportControls.IsNextEnabled = true;
        //    //    //MediaPlayer.SystemMediaTransportControls.IsPreviousEnabled = true;
        //    //}
        //    //else
        //    //{
        //    //    Console.WriteLine("Could not update SMTC display because song is null");
        //    //}

        //    if (CurrentSong != null)
        //    {
        //        // Set Other
        //        //MediaPlayer.SystemMediaTransportControls.IsNextEnabled = true;
        //        //MediaPlayer.SystemMediaTransportControls.IsPreviousEnabled = true;

        //        // Set DisplayUpdater properties
        //        MediaPlayer.SystemMediaTransportControls.DisplayUpdater.Type = MediaPlaybackType.Video;
        //        MediaPlayer.SystemMediaTransportControls.DisplayUpdater.VideoProperties.Title = CurrentSong.MusicProperties.Title;
        //        MediaPlayer.SystemMediaTransportControls.DisplayUpdater.VideoProperties.Subtitle = CurrentSong.MusicProperties.Artist;
        //        MediaPlayer.SystemMediaTransportControls.DisplayUpdater.Thumbnail = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromUri(new Uri("C:/Users/jeroe/Downloads/SongImagePlaceholder.png"));

        //        // Update Display
        //        MediaPlayer.SystemMediaTransportControls.DisplayUpdater.Update();
        //    }
        //}

        #endregion Private

        #region SearchOptions
        private SearchBarOption PlaySongSearchBarOption { get; set; }

        public void ConfigureSearchOptions(GlobalSearch globalSearch)
        {
            PlaySongSearchBarOption = new()
            {
                Header = "Play Song: ",
                Keywords = { "play", "start" },
                Command = new(o =>
                {
                    string text = o != null ? o.ToString() : "";
                    MessageBox.Show("Parameter: " + text);
                }),
            };
            PlaySongSearchBarOption.BeforeSearchOptionShow += PlaySongSearchBarOption_BeforeSearchOptionShow;

            globalSearch.AddSearchBarOption(PlaySongSearchBarOption);
        }

        private void PlaySongSearchBarOption_BeforeSearchOptionShow(object sender, string e)
        {
            foreach (SongModel song in GlobalViewModel.Instance.MyMusic.Songs)
            {
                if ((song.Title.Contains(e)))
                {
                    PlaySongSearchBarOption.Header = "Play Song: " + song.Title + " (" + song.ArtistsText + ")";
                    break;
                }
            }

            // Code to remove element
        }
        #endregion SearchOptions

        public void OpenMedia(SongModel song)
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
        /// Adds an amount of time in milliseconds to the song
        /// </summary>
        /// <param name="amount">Time in milliseconds</param>
        public void AddTime(double amount)
        {
            Position = Position.Add(TimeSpan.FromMilliseconds(amount));
        }

        /// <summary>
        /// Subtracts an amount of time in milliseconds from the song
        /// </summary>
        /// <param name="amount">Time in milliseconds</param>
        public void SubtractTime(double amount)
        {
            Position = Position.Subtract(TimeSpan.FromMilliseconds(amount));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="volume">Min 0.00, max 1.00</param>
        public void SetVolume(double volume)
        {
            Volume = Math.Clamp(volume, 0.00, 1.00);
        }
    }
}
