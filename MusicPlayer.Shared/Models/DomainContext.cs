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

        // Junction Tables
        public DbSet<ArtistSong> ArtistSongs { get; set; }
        public DbSet<PlaylistSong> PlaylistSongs { get; set; }
        public DbSet<SongGenre> SongGenres { get; set; }

        // Db Path
        public string DbPath { get; private set; }

        // https://stackoverflow.com/questions/59444014/entity-framework-tools-not-working-with-uwp-apps-c-sharp
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
            // ArtistSong
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

            // PlaylistSong
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

            // SongGenre
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
        }
    }
}
