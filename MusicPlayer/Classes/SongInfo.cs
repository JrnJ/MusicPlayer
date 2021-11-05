using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MusicPlayer.Classes
{
    public class SongInfo
    {
        public Uri Path { get; private set; }

        public string Name { get; private set; }

        public string Artist { get; private set; }

        public int Drop { get; private set; }

        public List<Timestamp> Timestamps { get; private set; }

        public SongInfo(string filePath)
        {
            // Look for file with same name as a .txt instead of .mp3
            Path = new Uri(filePath);

            string[] file = filePath.Split("/");
            string fileName = file[file.Length - 1].Split(".")[0];
            string fileFormat = ".mp3";
            string path = filePath.Replace(fileName + fileFormat, "");

            string[] lines = File.ReadAllLines(path + fileName + ".txt");

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("song:"))
                {
                    Name = lines[i].Replace("song:", "");
                }
                else if (lines[i].StartsWith("artist:"))
                {
                    Artist = lines[i].Replace("artist:", "");
                }
                else if (lines[i].StartsWith("drop:"))
                {
                    Drop = int.Parse(lines[i].Replace("drop:", ""));
                }
            }
        }
    }
}
