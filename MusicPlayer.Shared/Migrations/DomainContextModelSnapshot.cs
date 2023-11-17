﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MusicPlayer.Shared.Models;

#nullable disable

namespace MusicPlayer.Shared.Migrations
{
    [DbContext(typeof(DomainContext))]
    partial class DomainContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.13");

            modelBuilder.Entity("MusicPlayer.Shared.Models.Artist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Artists");
                });

            modelBuilder.Entity("MusicPlayer.Shared.Models.ArtistSong", b =>
                {
                    b.Property<int>("ArtistId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SongId")
                        .HasColumnType("INTEGER");

                    b.HasKey("ArtistId", "SongId");

                    b.HasIndex("SongId");

                    b.ToTable("ArtistSongs");
                });

            modelBuilder.Entity("MusicPlayer.Shared.Models.Genre", b =>
                {
                    b.Property<string>("Name")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.HasKey("Name");

                    b.ToTable("Genres");
                });

            modelBuilder.Entity("MusicPlayer.Shared.Models.Playlist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("ImagePath")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Playlists");
                });

            modelBuilder.Entity("MusicPlayer.Shared.Models.PlaylistSong", b =>
                {
                    b.Property<int>("PlaylistId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SongId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Index")
                        .HasColumnType("INTEGER");

                    b.HasKey("PlaylistId", "SongId");

                    b.HasIndex("SongId");

                    b.ToTable("PlaylistSongs");
                });

            modelBuilder.Entity("MusicPlayer.Shared.Models.Settings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<double>("Volume")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("MusicPlayer.Shared.Models.SettingsSongsFolder", b =>
                {
                    b.Property<int>("SettingsId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SongsFolderId")
                        .HasColumnType("INTEGER");

                    b.HasKey("SettingsId", "SongsFolderId");

                    b.HasIndex("SongsFolderId");

                    b.ToTable("SettingsSongsFolders");
                });

            modelBuilder.Entity("MusicPlayer.Shared.Models.Song", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("TEXT");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("SongsFolderId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Year")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("SongsFolderId");

                    b.ToTable("Songs");
                });

            modelBuilder.Entity("MusicPlayer.Shared.Models.SongGenre", b =>
                {
                    b.Property<int>("SongId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("GenreName")
                        .HasColumnType("TEXT");

                    b.HasKey("SongId", "GenreName");

                    b.HasIndex("GenreName");

                    b.ToTable("SongGenres");
                });

            modelBuilder.Entity("MusicPlayer.Shared.Models.SongsFolder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("SongsFolders");
                });

            modelBuilder.Entity("MusicPlayer.Shared.Models.ArtistSong", b =>
                {
                    b.HasOne("MusicPlayer.Shared.Models.Artist", "Artist")
                        .WithMany("Songs")
                        .HasForeignKey("ArtistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MusicPlayer.Shared.Models.Song", "Song")
                        .WithMany("Artists")
                        .HasForeignKey("SongId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Artist");

                    b.Navigation("Song");
                });

            modelBuilder.Entity("MusicPlayer.Shared.Models.PlaylistSong", b =>
                {
                    b.HasOne("MusicPlayer.Shared.Models.Playlist", "Playlist")
                        .WithMany("Songs")
                        .HasForeignKey("PlaylistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MusicPlayer.Shared.Models.Song", "Song")
                        .WithMany("Playlists")
                        .HasForeignKey("SongId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Playlist");

                    b.Navigation("Song");
                });

            modelBuilder.Entity("MusicPlayer.Shared.Models.SettingsSongsFolder", b =>
                {
                    b.HasOne("MusicPlayer.Shared.Models.Settings", "Settings")
                        .WithMany("SongsFolders")
                        .HasForeignKey("SettingsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MusicPlayer.Shared.Models.SongsFolder", "SongsFolder")
                        .WithMany("Settings")
                        .HasForeignKey("SongsFolderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Settings");

                    b.Navigation("SongsFolder");
                });

            modelBuilder.Entity("MusicPlayer.Shared.Models.Song", b =>
                {
                    b.HasOne("MusicPlayer.Shared.Models.SongsFolder", "SongsFolder")
                        .WithMany("Songs")
                        .HasForeignKey("SongsFolderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SongsFolder");
                });

            modelBuilder.Entity("MusicPlayer.Shared.Models.SongGenre", b =>
                {
                    b.HasOne("MusicPlayer.Shared.Models.Genre", "Genre")
                        .WithMany("Songs")
                        .HasForeignKey("GenreName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MusicPlayer.Shared.Models.Song", "Song")
                        .WithMany("Genres")
                        .HasForeignKey("SongId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Genre");

                    b.Navigation("Song");
                });

            modelBuilder.Entity("MusicPlayer.Shared.Models.Artist", b =>
                {
                    b.Navigation("Songs");
                });

            modelBuilder.Entity("MusicPlayer.Shared.Models.Genre", b =>
                {
                    b.Navigation("Songs");
                });

            modelBuilder.Entity("MusicPlayer.Shared.Models.Playlist", b =>
                {
                    b.Navigation("Songs");
                });

            modelBuilder.Entity("MusicPlayer.Shared.Models.Settings", b =>
                {
                    b.Navigation("SongsFolders");
                });

            modelBuilder.Entity("MusicPlayer.Shared.Models.Song", b =>
                {
                    b.Navigation("Artists");

                    b.Navigation("Genres");

                    b.Navigation("Playlists");
                });

            modelBuilder.Entity("MusicPlayer.Shared.Models.SongsFolder", b =>
                {
                    b.Navigation("Settings");

                    b.Navigation("Songs");
                });
#pragma warning restore 612, 618
        }
    }
}
