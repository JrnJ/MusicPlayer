using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MusicPlayer.Classes
{
    public static class Elements
    {
        // Element Creation Methods
        public static Border CreateSongUI(Song song, int index)
        {
            // Create UI Elements
            Border borderContainer = new Border()
            {
                Style = Application.Current.FindResource("AlbumSong") as Style
            };
            //borderContainer.MouseDown += AlbumSongMouseDown;
            //borderContainer.Name = RegisterAndOrSetName($"Song{index}", borderContainer);

            DockPanel dpContentDocker = new DockPanel()
            {
                Height = 50
            };

            TextBlock tblSongId = new TextBlock()
            {
                Text = song.Id.ToString(),
                TextAlignment = TextAlignment.Center,
                Width = 26
            };

            StackPanel spSongInfoDevider = new StackPanel()
            {
                VerticalAlignment = VerticalAlignment.Center
            };

            TextBlock tblSongTitle = new TextBlock()
            {
                Text = song.Title
            };

            TextBlock tblSongAuthor = new TextBlock()
            {
                Text = song.ContributingArtists,
                Foreground = new SolidColorBrush(Color.FromRgb(153, 153, 153)),
                FontSize = 14
            };

            DockPanel dpRightSide = new DockPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 16, 0)
            };

            TextBlock tblSongLength = new TextBlock()
            {
                Text = HelperMethods.MsToTime(song.Length),
                // ONLY IF ALL NUMBERS AFTER : ARE 0 REMOVE THEM 4:10 != 4:1
                Foreground = new SolidColorBrush(Color.FromRgb(153, 153, 153))
            };

            Button btnAction = new Button()
            {
                Content = "- - -",
                Width = 30,
                Height = 20,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(20, 0, 0, 0)
            };

            // Put UI elements together
            spSongInfoDevider.Children.Add(tblSongTitle);
            spSongInfoDevider.Children.Add(tblSongAuthor);

            dpContentDocker.Children.Add(tblSongId);
            dpContentDocker.Children.Add(spSongInfoDevider);

            dpRightSide.Children.Add(tblSongLength);
            dpRightSide.Children.Add(btnAction);

            dpContentDocker.Children.Add(dpRightSide);

            borderContainer.Child = dpContentDocker;

            // Return new UI element
            return borderContainer;
        }

        // Helper Methods
    }
}
