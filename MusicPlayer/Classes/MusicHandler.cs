using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MusicPlayer.Classes
{
    public static class MusicHandler
    {
        public static Playlist GetMusicFromFolder(string path)
        {
            try
            {
                // Get files in folder
                string[] folderContent = Directory.GetFiles(path);
                List<Song> songs = new();
                int songId = 1;

                // Loop through files
                for (int i = 0; i < folderContent.Length; i++)
                {
                    if (IsMusic(folderContent[i]))
                    {
                        // Get file
                        TagLib.File tFile = TagLib.File.Create(folderContent[i]);

                        TagLib.Picture pic = new TagLib.Picture("../../../Images/SongImagePlaceholder.png");
                        //TagLib.Picture pic = null;

                        // Add song to songs
                        songs.Add(new Song(new Uri(folderContent[i]), tFile.Tag.Title, null, null, tFile.Tag.Comment,
                            tFile.Tag.FirstPerformer, tFile.Tag.FirstAlbumArtist, tFile.Tag.Album, tFile.Tag.Year, songId, tFile.Tag.FirstGenre, pic, tFile.Properties.Duration.TotalMilliseconds,
                            tFile.Properties.AudioBitrate
                            ));

                        songId++;
                    }
                }

                return new Playlist(0, path, songs);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool IsMusic(string fileName)
        {
            return fileName.Contains(".mp3");
        }
    }
}
