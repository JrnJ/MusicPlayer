using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Media;
using Windows.Media.Playback;

namespace MusicPlayer.Audio
{
    public abstract class NewAudioPlayer : IAudioPlayer
    {
        // Properties
        private MediaPlayer _mediaPlayer;

        public MediaPlayer MediaPlayer
        {
            get { return _mediaPlayer; }
            set { _mediaPlayer = value; }
        }

        // Constructor
        public NewAudioPlayer()
        {
            _mediaPlayer = new();
        }

        // Methods
        public abstract void Dispose();

        public virtual Task PlaySong(SongModel song)
        {
            throw new NotImplementedException();
        }

        public virtual Task Pause()
        {
            throw new NotImplementedException();
        }

        public virtual Task Resume()
        {
            throw new NotImplementedException();
        }
    }
}
