using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using MusicPlayer.Core;
using MusicPlayer.MVVM.Model;
using MusicPlayer.MVVM.ViewModel;

namespace MusicPlayer.MVVM.View
{
    /// <summary>
    /// Interaction logic for PlaylistView.xaml
    /// </summary>
    public partial class PlaylistView : UserControl
    {
        [StructLayout(LayoutKind.Sequential)]
        struct POINT
        {
            public Int32 X;
            public Int32 Y;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT point);

        private DispatcherTimer Timer { get; set; }
        private POINT LastPoint { get; set; }
        private bool IsDragging { get; set; } = false;
        private RadioButton ElementSelected { get; set; }
        private int SongId { get; set; }
        private int PreviousMoveIndex { get; set; } = 0;

        public PlaylistView()
        {
            InitializeComponent();
            Timer = new()
            {
                Interval = TimeSpan.FromMilliseconds(10)
            };
            Timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            POINT p = new();
            GetCursorPos(out p);
            if (p.Y != LastPoint.Y)
            {
                // Moved
                Timer.Stop();

                // Get+Set song
                AlbumSongModel song = GlobalViewModel.Instance.PlaylistViewing.Songs.FirstOrDefault(x => x.Id == SongId);
                meow.DataContext = song;
                PreviousMoveIndex = GlobalViewModel.Instance.PlaylistViewing.Songs.IndexOf(song);

                // Element Visuals
                ElementSelected.Visibility = Visibility.Hidden;
                meow.Visibility = Visibility.Visible;

                // Start Dragging
                Mouse.Capture(null);
                IsDragging = true;
            }
        }

        private void RadioButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            POINT p = new();
            GetCursorPos(out p);
            LastPoint = p;
            ElementSelected = (sender as RadioButton);
            SongId = (int)ElementSelected.CommandParameter;
            Timer.Start();

            AppWindowExtensions.GetMainWindow().MouseMove += WindowMouseMove;
            AppWindowExtensions.GetMainWindow().PreviewMouseUp += WindowPreviewMouseUp;
        }

        private void RadioButton_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Timer.Stop();
            ElementSelected = null;
            SongId = 0;
        }

        private void WindowMouseMove(object sender, MouseEventArgs e)
        {
            MouseMoved(e);
        }

        private void WindowPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            DragCanceled();
        }

        void MouseMoved(MouseEventArgs e)
        {
            if (IsDragging)
            {
                RadioButton toMove = meow;
                Point mousePos = e.GetPosition(canvas);

                // For now cap on heigth
                double elementHalfHeight = toMove.ActualHeight / 2.0;
                double marginBetweenElements = 8.0;

                double visualY = Math.Clamp(mousePos.Y - elementHalfHeight, 0.0, canvas.ActualHeight - elementHalfHeight - 24);
                double actualY = Math.Clamp(mousePos.Y + songsScroller.VerticalOffset, 0.0, canvas.ActualHeight - elementHalfHeight - 24);

                // TODO
                // if it reaches the bottom
                // then start scrolling down
                // if it reaches the top
                // then start scrolling up
                if (visualY == 0.0)
                {
                    // scroll scrollbar down based on a percentage
                    // songsScroller.ActualHeight;
                    double newVerticalOffset = songsScroller.VerticalOffset + songsScroller.ScrollableHeight * 0.01;
                    if (newVerticalOffset > songsScroller.ScrollableHeight)
                    {
                        newVerticalOffset = songsScroller.ScrollableHeight;
                    }

                    songsScroller.ScrollToVerticalOffset(newVerticalOffset);
                }

                int elementsPassed = int.Parse(Math.Floor(actualY / (toMove.ActualHeight + marginBetweenElements)).ToString());

                if (PreviousMoveIndex != elementsPassed)
                {
                    // Move Songs
                    GlobalViewModel.Instance.PlaylistViewing.Songs.Move(PreviousMoveIndex, elementsPassed);

                    // Reset
                    PreviousMoveIndex = elementsPassed;

                    // Finally, on mouse let go, save the playlist
                }

                // Move in Canvas
                Canvas.SetTop(toMove, visualY);
            }
        }

        void DragCanceled()
        {
            if (IsDragging)
            {
                AppWindowExtensions.GetMainWindow().MouseMove -= WindowMouseMove;
                AppWindowExtensions.GetMainWindow().PreviewMouseUp -= WindowPreviewMouseUp;

                IsDragging = false;
                ElementSelected.Visibility = Visibility.Visible;
                ElementSelected = null;
                meow.Visibility = Visibility.Hidden;
            }
        }
    }
}
