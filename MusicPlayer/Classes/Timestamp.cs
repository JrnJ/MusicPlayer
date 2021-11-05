using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Classes
{
    public class Timestamp
    {
        public int At { get; private set; }

        public string Message { get; private set; }

        /// <summary>
        /// Timestamp for a song
        /// </summary>
        /// <param name="at">Seconds</param>
        /// <param name="message">Addon</param>
        public Timestamp(int at, string message)
        {
            At = at;
            Message = message;
        }
    }
}
