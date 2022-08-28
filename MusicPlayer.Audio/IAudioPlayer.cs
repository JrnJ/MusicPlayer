using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Audio
{
    internal interface IAudioPlayer : IDisposable
    {
        Task Play();

        Task Pause();

        Task Resume();
    }
}
