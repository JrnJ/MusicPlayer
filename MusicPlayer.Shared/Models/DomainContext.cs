using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace MusicPlayer.Shared.Models
{
    public class DomainContext : DbContext
    {
        // Tables
        public DbSet<Song> Songs { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<SongsFolder> SongsFolders { get; set; }

        // Junction Tables
        public DbSet<ArtistSong> ArtistSongs { get; set; }
        public DbSet<PlaylistSong> PlaylistSongs { get; set; }
        public DbSet<SongGenre> SongGenres { get; set; }
        public DbSet<SettingsSongsFolder> SettingsSongsFolders { get; set; }

        // Db Path
        public string DbPath { get; private set; }

        public DomainContext()
        {
            DbPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\JeroenJ\\MusicPlayer\\MusicPlayer.db";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            SqliteConnectionStringBuilder connectionStringBuilder = new()
            {
                DataSource = DbPath
            };

            string connectionString = connectionStringBuilder.ToString();
            SqliteConnection connection = new(connectionString);

            options.UseSqlite(connection);

            // options.UseSqlite($"Data Source={DbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relations
            // n:m ArtistSong
            modelBuilder.Entity<ArtistSong>()
                .HasKey(x => new { x.ArtistId, x.SongId });

            modelBuilder.Entity<ArtistSong>()
                .HasOne(pt => pt.Artist)
                .WithMany(p => p.Songs)
                .HasForeignKey(pt => pt.ArtistId);

            modelBuilder.Entity<ArtistSong>()
                .HasOne(pt => pt.Song)
                .WithMany(p => p.Artists)
                .HasForeignKey(pt => pt.SongId);

            // n:m PlaylistSong
            modelBuilder.Entity<PlaylistSong>()
                .HasKey(x => new { x.PlaylistId, x.SongId });

            modelBuilder.Entity<PlaylistSong>()
                .HasOne(pt => pt.Playlist)
                .WithMany(p => p.Songs)
                .HasForeignKey(pt => pt.PlaylistId);

            modelBuilder.Entity<PlaylistSong>()
                .HasOne(pt => pt.Song)
                .WithMany(p => p.Playlists)
                .HasForeignKey(pt => pt.SongId);

            // n:m SongGenre
            modelBuilder.Entity<SongGenre>()
                .HasKey(x => new { x.SongId, x.GenreName });

            modelBuilder.Entity<SongGenre>()
                .HasOne(pt => pt.Song)
                .WithMany(p => p.Genres)
                .HasForeignKey(pt => pt.SongId);

            modelBuilder.Entity<SongGenre>()
                .HasOne(pt => pt.Genre)
                .WithMany(p => p.Songs)
                .HasForeignKey(pt => pt.GenreName);

            // n:m SettingsSongsFolder
            modelBuilder.Entity<SettingsSongsFolder>()
                .HasKey(x => new { x.SettingsId, x.SongsFolderId });

            modelBuilder.Entity<SettingsSongsFolder>()
                .HasOne(pt => pt.Settings)
                .WithMany(p => p.SongsFolders)
                .HasForeignKey(pt => pt.SettingsId);

            modelBuilder.Entity<SettingsSongsFolder>()
                .HasOne(pt => pt.SongsFolder)
                .WithMany(p => p.Settings)
                .HasForeignKey(pt => pt.SongsFolderId);

            // 1:n SongsFolder has Songs
            modelBuilder.Entity<Song>()
                .HasOne(s => s.SongsFolder)
                .WithMany(f => f.Songs)
                .HasForeignKey(s => s.SongsFolderId);
        }
    }
}
