﻿using MusicPlayer.Core;
using MusicPlayer.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Windows.Media;

namespace MusicPlayer.Classes
{
    internal class AudioPlayer : ObservableObject
    {
        // Unsure about accesibility here
        public SystemMediaTransportControls MediaControls { get; set; }

        public Windows.Media.Playback.MediaPlayer MediaPlayer { get; set; }

        // Stoopid props
        private AlbumSongModel _currentSong;

        public AlbumSongModel CurrentSong
        {
            get { return _currentSong; }
            set { _currentSong = value; OnPropertyChanged(); }
        }

        public bool IsPlaying { get; private set; }

        public double Volume
        {
            get => MediaPlayer.Volume;
            set => MediaPlayer.Volume = value;
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
            MediaPlayer = new Windows.Media.Playback.MediaPlayer();

            // Events
            MediaPlayer.MediaOpened += MediaPlayerMediaOpened;
            MediaPlayer.MediaEnded += MediaPlayerMediaEnded;

            MediaPlayer.VolumeChanged += MediaPlayerVolumeChanged;

            // Properties
            MediaPlayer.CommandManager.IsEnabled = true;

            // SMTC
            MediaPlayer.SystemMediaTransportControls.DisplayUpdater.Type = MediaPlaybackType.Video;
            MediaPlayer.SystemMediaTransportControls.IsNextEnabled = true;
            MediaPlayer.SystemMediaTransportControls.IsPreviousEnabled = true; 
            // https://github.com/microsoft/Windows-universal-samples/blob/dev/Samples/SystemMediaTransportControls/cs/Scenario1.xaml.cs
        }

        private void MediaPlayerVolumeChanged(Windows.Media.Playback.MediaPlayer sender, object args)
        {
            // Apply volume change to settings
            AppSettings.Volume = sender.Volume;
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

        private void UpdateSMTCDisplay()
        {
            if (CurrentSong != null)
            {
                SystemMediaTransportControls smtc = MediaPlayer.SystemMediaTransportControls;
                //smtc.DisplayUpdater.ClearAll();
                //smtc.IsNextEnabled = true;
                //smtc.IsPreviousEnabled = true;
                smtc.DisplayUpdater.Type = MediaPlaybackType.Video;
                smtc.DisplayUpdater.VideoProperties.Title = CurrentSong.Title;
                smtc.DisplayUpdater.VideoProperties.Subtitle = CurrentSong.ContributingArtists;
                smtc.DisplayUpdater.Thumbnail = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromUri(new Uri("C:/Users/jeroe/Downloads/SongImagePlaceholder.png"));
                smtc.IsNextEnabled = true;
                smtc.IsPreviousEnabled = true;

                // Update the system media transport controls
                smtc.DisplayUpdater.Update();

                // LateUpdate??
                //MediaPlayer.SystemMediaTransportControls.IsNextEnabled = true;
                //MediaPlayer.SystemMediaTransportControls.IsPreviousEnabled = true;
            }
            else
            {
                Console.WriteLine("Could not update SMTC display because song is null");
            }
        }

        #endregion Private

        public void OpenMedia(AlbumSongModel song)
        {
            CurrentSong = song;
            Pause();

            MediaPlayer.SetUriSource(new Uri(song.Path));

            // Play Song
            //Play();
        }

        // Maybe resume instead
        /// <summary>
        /// Plays the Audio
        /// </summary>
        public void Play()
        {
            MediaPlayer.Play();
            IsPlaying = true;

            Timer.Start();
        }

        /// <summary>
        /// Pauses the Audio
        /// </summary>
        public void Pause()
        {
            MediaPlayer.Pause();
            IsPlaying = false;

            Timer.Stop();
        }

        /// <summary>
        /// Adds an amount of time in seconds to the song
        /// </summary>
        /// <param name="amount">Time in seconds</param>
        public void AddTime(int amount)
        {
            MediaPlayer.Position = MediaPlayer.Position.Add(new TimeSpan(0, 0, amount));
            //UpdateTime(); this only updates the ui so it wont take a full second to move, nothing major breaking atm
        }

        /// <summary>
        /// Subtracts an amount of time in seconds from the song
        /// </summary>
        /// <param name="amount">Time in seconds</param>
        public void SubtractTime(int amount)
        {
            MediaPlayer.Position = MediaPlayer.Position.Subtract(new TimeSpan(0, 0, amount));
            //UpdateTime();
        }
    }
}
