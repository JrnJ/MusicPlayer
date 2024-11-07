using Microsoft.EntityFrameworkCore;
using MusicPlayer.Database;
using MusicPlayer.MVVM.Model;
using MusicPlayer.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MusicPlayer.Classes
{
    internal static class DbHelper
    {
        #region General
        public static async Task<SongsFolder> CreateSongsFolder(string path, int settingsId)
        {
            using DomainContext context = new();

            Settings? settings = context.Settings
                .Where(s => s.Id == settingsId)
                .Include(s => s.SongsFolders)
                .FirstOrDefault();

            if (settings == null) throw new Exception($"Database doesn't have Settings with id: {settingsId}");

            // Create Models
            SongsFolder songsFolder = new()
            {
                Path = path,
                Settings = []
            };

            SettingsSongsFolder settingsSongsFolder = new()
            {
                SettingsId = settings.Id,
                SongsFolderId = songsFolder.Id
            };

            settings.SongsFolders.Add(settingsSongsFolder);
            songsFolder.Settings.Add(settingsSongsFolder);

            // Update Database
            context.SongsFolders.Add(songsFolder);
            await context.SaveChangesAsync();

            return songsFolder;
        }
        #endregion General

        #region Songs
        public static async Task AlterSong(SongModel song)
        {
            throw new NotImplementedException();
            using DomainContext context = new();


            await context.SaveChangesAsync();
        }

        public static async Task DeleteSong(SongModel song)
        {
            throw new NotImplementedException();
            using DomainContext context = new();



            await context.SaveChangesAsync();
        }
        #endregion Songs

        #region Playlist
        public static async Task<Playlist> CreatePlaylist(string name, string description)
        {
            using DomainContext context = new();

            Playlist playlist = new()
            {
                Name = name,
                Description = description,
                ImagePath = "",
                Songs = [],
            };
            context.Playlists.Add(playlist);

            await context.SaveChangesAsync();

            return playlist;
        }

        public static async Task DeletePlaylist(int playlistId)
        {
            using DomainContext context = new();
            Playlist? playlistToDelete = context.Playlists.FirstOrDefault(p => p.Id == playlistId);

            if (playlistToDelete == null) return;

            context.Playlists.Remove(playlistToDelete);

            await context.SaveChangesAsync();
        }

        public static async Task UpdatePlaylist()
        {
            throw new NotImplementedException();

            using DomainContext context = new();

            await context.SaveChangesAsync();
        }

        public static async Task AddSongToPlaylist(Song song, Playlist playlist)
        {
            using DomainContext context = new();
            await _AddSongToPlaylist(song.Id, playlist.Id, playlist.Songs.Count, context);
            await context.SaveChangesAsync();
        }

        public static async Task AddSongToPlaylist(int songId, int playlistId, int index)
        {
            using DomainContext context = new();
            await _AddSongToPlaylist(songId, playlistId, index, context);
            await context.SaveChangesAsync();
        }

        private static async Task _AddSongToPlaylist(int songId, int playlistId, int index, DomainContext context)
        {
            // Only add if it doesnt exist
            if (await context.PlaylistSongs.AnyAsync(ps => ps.SongId == songId && ps.PlaylistId == playlistId)) return;

            PlaylistSong playlistSong = new()
            {
                SongId = songId,
                PlaylistId = playlistId,
                Index = index
            };
            context.PlaylistSongs.Add(playlistSong);
        }

        public static async Task RemoveSongFromPlaylist(int songId, int playlistId)
        {
            using DomainContext context = new();
            await _RemoveSongFromPlaylist(songId, playlistId, context);
            await context.SaveChangesAsync();
        }

        private static async Task _RemoveSongFromPlaylist(int songId, int playlistId, DomainContext context)
        {
            PlaylistSong? newPlaylistSong = await context.PlaylistSongs.FirstOrDefaultAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId);
            if (newPlaylistSong == null) return;

            context.PlaylistSongs.Remove(newPlaylistSong);

            // Update PlaylistSong Indexes
            IQueryable<PlaylistSong> playlistSongs = context.PlaylistSongs.Where(ps => ps.PlaylistId == playlistId);

            foreach (PlaylistSong playlistSong in playlistSongs)
            {
                if (playlistSong.Index > newPlaylistSong.Index)
                {
                    // If we add a delete many we can decrease more here
                    // If we add a delete random selection this wont work
                    playlistSong.Index -= 1;
                }
            }
        }

        public static async Task SwapSongInPlaylist(int playlistId, int oldIndex, int newIndex)
        {
            using DomainContext context = new();

            Playlist? playlist = await context.Playlists.FirstOrDefaultAsync(p => p.Id == playlistId);
            if (playlist == null) return;

            PlaylistSong? oldSong = playlist.Songs.FirstOrDefault(sm => sm.Index == oldIndex);
            PlaylistSong? newSong = playlist.Songs.FirstOrDefault(sm => sm.Index == oldIndex);
            if (oldSong == null || newSong == null) return;

            oldSong.Index = newIndex;
            newSong.Index = oldIndex;

            await context.SaveChangesAsync();
        }
        #endregion Playlist

        #region Artists
        public static async Task<Artist> CreateArtist(string name)
        {
            using DomainContext context = new();
            return await _CreateArtist(name, context);
        }

        public static async Task<Artist> CreateArtist(string name, DomainContext context)
        {
            return await _CreateArtist(name, context);
        }

        private static async Task<Artist> _CreateArtist(string name, DomainContext context)
        {
            Artist artist = new()
            {
                Name = name,
                Songs = []
            };
            context.Artists.Add(artist);

            // Find Songs by Artist
            IQueryable<Song> songsByArtist = context.Songs.Where(s => s.Artists.Any(a => a.Artist.Name == name));
            foreach (Song song in songsByArtist)
            {
                CreateArtistSong(artist, song);
            }

            await context.SaveChangesAsync();

            return artist;
        }

        private static void CreateArtistSong(Artist artist, Song song)
        {
            // TODO: duplicate can be here
            ArtistSong artistSong = new()
            {
                ArtistId = artist.Id,
                SongId = song.Id
            };
            artist.Songs.Add(artistSong);
            song.Artists.Add(artistSong);
        }
        #endregion Artists

        #region Genres
        public static async Task CreateGenre(string name)
        {
            using DomainContext context = new();
            await _CreateGenre(name, context);
        }

        public static async Task CreateGenre(string name, DomainContext context)
        {
            await CreateGenre(name, context);
        }

        private static async Task _CreateGenre(string name, DomainContext context)
        {
            Genre genre = new()
            {
                Name = name,
                Songs = [],
            };

            // Find all Songs with Genre
            IQueryable<Song> songsWithGenre = context.Songs.Where(s => s.Genres.Any(g => g.GenreName == name));
            foreach (Song song in songsWithGenre)
            {
                CreateSongGenre(song, genre);
            }

            await context.SaveChangesAsync();
        }

        private static void CreateSongGenre(Song song, Genre genre)
        {
            // TODO: duplicate can be here
            SongGenre songGenre = new()
            {
                SongId = song.Id,
                GenreName = genre.Name,
            };
            song.Genres.Add(songGenre);
            genre.Songs.Add(songGenre);
        }
        #endregion Genres

        #region Settings
        public static async Task SetVolume(double volume, int settingsId)
        {
            using DomainContext context = new();
            Settings? settings = context.Settings.FirstOrDefault(s => s.Id == settingsId);
            if (settings == null) return;

            settings.Volume = Math.Clamp(volume, 0.0, 1.0);
            await context.SaveChangesAsync();
        }
        #endregion Settings
    }
}
