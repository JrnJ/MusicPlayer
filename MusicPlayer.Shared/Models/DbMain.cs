using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Shared.Models
{
    public class DbMain
    {
        public static void Main(string[] args)
        {
            using (DomainContext context = new())
            {
                // TODO2:
                // remove test data
                Artist artist = new()
                {
                    Name = "Jeroen",
                    Songs = new List<ArtistSong>()
                };
                context.Artists.Add(artist);

                Genre genre = new()
                {
                    Name = "Jeroenium",
                    Songs = new List<SongGenre>()
                };
                context.Genres.Add(genre);

                Playlist playlist = new()
                {
                    Name = "Jeroen's Playlist",
                    Description = "Playlist made by Jeroen",
                    ImagePath = "D:\\Music\\Images\\K-ON.png",
                    Songs = new List<PlaylistSong>()
                };

                Song song = new()
                {
                    Path = "D:\\Music\\Feelgood\\Earth Wind  Fire - September.mp3",
                    Title = "Jeroen's Song!",
                    Year = 2002,
                    Duration = new TimeSpan(0, 3, 21),
                    Artists = new List<ArtistSong>(),
                    Playlists = new List<PlaylistSong>(),
                    Genres = new List<SongGenre>(),
                    SongsFolder = new()
                };

                SongGenre songGenre = new()
                {
                    // song.Id is set but never created, yet in the database this went correctly
                    SongId = song.Id,
                    GenreName = genre.Name
                };
                song.Genres.Add(songGenre);
                genre.Songs.Add(songGenre);

                ArtistSong artistSong = new()
                {
                    // song.Id is set but never created, yet in the database this went correctly
                    ArtistId = artist.Id,
                    SongId = song.Id,
                };
                artist.Songs.Add(artistSong);
                song.Artists.Add(artistSong);

                PlaylistSong playlistSong = new()
                {
                    PlaylistId = playlist.Id,
                    SongId = song.Id,
                    Index = 0
                };
                playlist.Songs.Add(playlistSong);
                song.Playlists.Add(playlistSong);

                // Settings
                Settings settings = new()
                {
                    Volume = 0.33,
                    SongsFolders = new List<SettingsSongsFolder>()
                };

                SongsFolder songsFolder = new()
                {
                    Path = "D:\\Music\\Feelgood",
                    Settings = new List<SettingsSongsFolder>(),
                    Songs = new List<Song>()
                };
                song.SongsFolder = songsFolder;
                songsFolder.Songs.Add(song);

                SettingsSongsFolder settingsSongsFolder = new()
                {
                    SettingsId = settings.Id,
                    SongsFolderId = songsFolder.Id,
                };
                settings.SongsFolders.Add(settingsSongsFolder);
                songsFolder.Settings.Add(settingsSongsFolder);

                // Remove if already exists
                if (context.Songs.ToList().Count > 0) return;
                Console.WriteLine("Created Test Data because DataBase was empty! MusicPlayer.Shared.Models.DbMain::Main");

                context.Songs.Add(song);

                context.Playlists.Add(playlist);

                context.Settings.Add(settings);
                context.SongsFolders.Add(songsFolder);

                context.SaveChanges();
            }
        }
    }
}
