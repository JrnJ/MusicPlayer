﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MusicPlayer.Shared.Models;

#nullable disable

namespace MusicPlayer.Shared.Migrations
{
    [DbContext(typeof(DomainContext))]
    [Migration("20231112000419_initDb")]
    partial class initDb
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

            modelBuilder.Entity("MusicPlayer.Shared.Models.Playlist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
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

                    b.HasKey("PlaylistId", "SongId");

                    b.HasIndex("SongId");

                    b.ToTable("PlaylistSongs");
                });

            modelBuilder.Entity("MusicPlayer.Shared.Models.Song", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Songs");
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

            modelBuilder.Entity("MusicPlayer.Shared.Models.Artist", b =>
                {
                    b.Navigation("Songs");
                });

            modelBuilder.Entity("MusicPlayer.Shared.Models.Playlist", b =>
                {
                    b.Navigation("Songs");
                });

            modelBuilder.Entity("MusicPlayer.Shared.Models.Song", b =>
                {
                    b.Navigation("Artists");

                    b.Navigation("Playlists");
                });
#pragma warning restore 612, 618
        }
    }
}
