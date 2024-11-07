using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Shared
{
    public class Constants
    {
        private const string BASE_DATABASE_NAME = "MusicPlayer";
#if DEBUG
        public const string DatabaseName = BASE_DATABASE_NAME + "_Debug";
#elif RELEASE
        public const string DatabaseName = BASE_DATABASE_NAME;
#else
        public const string DatabaseName = BASE_DATABASE_NAME + "_Default";
#endif
    }
}
