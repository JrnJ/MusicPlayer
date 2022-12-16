using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Audio
{
    public interface IAudioPlayer : IDisposable
    {
        Task PlaySong(SongModel song);

        Task Pause();

        Task Resume();
    }
}
