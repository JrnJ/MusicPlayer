﻿using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MusicPlayer.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.MVVM.Model
{
    public class DomainContext : DbContext
    {
        // Tables
        public DbSet<Song> Songs { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Playlist> Playlists { get; set; }

        // Junction Tables
        public DbSet<ArtistSong> ArtistSongs { get; set; }
        public DbSet<PlaylistSong> PlaylistSongs { get; set; }

        // Db Path
        public string DbPath { get; private set; }

        public DomainContext(DbContextOptions<DomainContext> options) 
            : base(options)
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
        }
    }
}
