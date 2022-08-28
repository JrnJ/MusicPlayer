using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Media;
using Windows.Media.Playback;

namespace MusicPlayer.Audio
{
    internal abstract class AudioPlayer : IAudioPlayer
    {
        // Properties
        private MediaPlayer _mediaPlayer;

        public MediaPlayer MediaPlayer
        {
            get { return _mediaPlayer; }
            set { _mediaPlayer = value; }
        }

        // Constructor
        public AudioPlayer()
        {
            MediaPlayer = new();
        }

        // Methods
        public abstract void Dispose();

        public Task Pause()
        {
            throw new NotImplementedException();
        }

        public Task Play()
        {
            throw new NotImplementedException();
        }

        public Task Resume()
        {
            throw new NotImplementedException();
        }
    }
}
