using System;
using System.Windows;
using System.Collections.Generic;

namespace MusicPlayer.Classes
{
    public static class HelperMethods
    {
        /// <summary>
        /// Converst milliseconds to a h:m:ss string
        /// </summary>
        /// <param name="ms">Amount of time in milliseconds</param>
        /// <returns>a h:m:ss string</returns>
        public static string MsToTime(double ms)
        {
            double hours = ms / 1000 / 60 / 60;
            double minutes = hours % 1 * 60;
            double seconds = Math.Floor(minutes % 1 * 60);
            hours = Math.Floor(hours);
            minutes = Math.Floor(minutes);

            return (hours > 0 ? hours + ":" : "") + minutes + ":" + (seconds < 10 ? "0" + seconds : seconds);
        }

        public static bool IsMusicFile(string path)
        {
            if (path.Contains(".mp3"))
            {
                return true;
            }
            else
                return false;
        }

        public static Rect GetAbsolutePlacement(this FrameworkElement element, bool relativeToScreen = false)
        {
            var absolutePos = element.PointToScreen(new System.Windows.Point(0, 0));
            if (relativeToScreen)
            {
                return new Rect(absolutePos.X, absolutePos.Y, element.ActualWidth, element.ActualHeight);
            }
            var posMW = Application.Current.MainWindow.PointToScreen(new System.Windows.Point(0, 0));
            absolutePos = new System.Windows.Point(absolutePos.X - posMW.X, absolutePos.Y - posMW.Y);
            return new Rect(absolutePos.X, absolutePos.Y, element.ActualWidth, element.ActualHeight);
        }
    }
}
