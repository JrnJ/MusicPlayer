using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
