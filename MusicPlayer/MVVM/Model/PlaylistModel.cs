﻿using MusicPlayer.Classes;
using MusicPlayer.Core;
using MusicPlayer.Shared.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Documents;

namespace MusicPlayer.MVVM.Model
{
    internal class PlaylistModel : ObservableObject
    {
        public int Id { get; init; }

        private string _name;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private string _description;

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        private string _imagePath;

        public string ImagePath
        {
            get => _imagePath;
            set { _imagePath = value; OnPropertyChanged(); }
        }

        // Not present in Database
        private ObservableCollection<SongModel> _songs = [];

        public ObservableCollection<SongModel> Songs
        {
            get => _songs;
            set 
            { 
                _songs = value;

                int totalMs = 0;
                foreach (SongModel song in _songs)
                {
                    totalMs += song.Duration.Milliseconds;
                }
                PlaylistDuration = HelperMethods.MsToTime(totalMs);

                OnPropertyChanged(); 
            }
        }

        private string _playlistDuration;

        public string PlaylistDuration
        {
            get => _playlistDuration;
            set {  _playlistDuration = value; OnPropertyChanged(); }
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }

        public PlaylistModel() { }

        public PlaylistModel(Playlist playlist)
        {
            Id = playlist.Id;
            _name = playlist.Name;
            _description = playlist.Description;
            _imagePath = playlist.ImagePath;
        }

        public PlaylistModel(Playlist playlist, ObservableCollection<SongModel> allSongs)
        {
            Id = playlist.Id;
            _name = playlist.Name;
            _description = playlist.Description;
            _imagePath = playlist.ImagePath;

            // Reference based!
            foreach (PlaylistSong playlistSong in playlist.Songs)
            {
                foreach (SongModel song in allSongs)
                {
                    if (playlistSong.SongId == song.Id)
                    {
                        Songs.Add(song);
                        break;
                    }
                }
            }
        }
    }
}
