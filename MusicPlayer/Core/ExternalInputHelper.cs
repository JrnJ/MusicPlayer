using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Core
{
    // https://docs.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
    public class ExternalInputHelper
    {
        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(Int32 vKey);

        public static bool IsKeyDown(Int32 keycode)
        {
            return (GetAsyncKeyState(keycode) & 0x8000) != 0;
        }
    }
}
