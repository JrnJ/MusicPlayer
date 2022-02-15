using System;
using System.Windows;

namespace MusicPlayer.Classes
{
    public static class HelperMethods
    {
        // Converst milliseconds to a time format
        public static string MsToTime(double ms)
        {
            string time = new DateTime().AddMilliseconds(ms).ToString("H:mm:ss");
            string finalTime = "";

            string[] timeStamps = time.Split(':');

            // Check if strin contains something other than 0
            for (int i = 0; i < timeStamps.Length; i++)
            {
                foreach (char c in timeStamps[i])
                {
                    if (c != '0')
                    {
                        finalTime += timeStamps[i] + ":";
                        break;
                    }
                }
            }

            if (finalTime.Length > 0)
            {
                // Remove last :
                finalTime = finalTime.Remove(finalTime.Length - 1);

                if (finalTime.Length < 4)
                {
                    finalTime = "0:" + finalTime;
                }
                else if (finalTime[0].ToString() == "0")
                {
                    finalTime = finalTime.Remove(0, 1);
                }
            }
            else
            {
                finalTime = "0:00";
            }

            return finalTime;
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
