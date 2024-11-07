using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
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
using Microsoft.UI.Xaml.Input;
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

        // Indexes
        private int FirstMoveIndex { get; set; }
        private int PreviousMoveIndex { get; set; } = 0;

        // AutoScrolling
        private DispatcherTimer AutoScrollTimer { get; set; }
        private bool AutoScrollUp { get; set; }
        private double UnClamppedVisualY { get; set; }
        private double MaxCanvasY { get; set; }

        // 
        private int ElementsPassed { get; set; }

        public PlaylistView()
        {
            InitializeComponent();
            Timer = new()
            {
                Interval = TimeSpan.FromMilliseconds(10)
            };
            Timer.Tick += Timer_Tick;

            AutoScrollTimer = new()
            {
                Interval = TimeSpan.FromMilliseconds(10)
            };
            AutoScrollTimer.Tick += AutoScrollTimerTick;
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
                SongModel song = GlobalViewModel.Instance.PlaylistsManager.PlaylistViewing.Songs.FirstOrDefault(x => x.Id == SongId);
                meow.DataContext = song;
                PreviousMoveIndex = GlobalViewModel.Instance.PlaylistsManager.PlaylistViewing.Songs.IndexOf(song);

                // Element Visuals
                ElementSelected.Visibility = Visibility.Hidden;
                meow.Visibility = Visibility.Visible;
                // TODO
                // element shows at 0, 0 before the drag really happens so it looks like it will snap, canvas thing

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
            FirstMoveIndex = GlobalViewModel.Instance.PlaylistsManager.PlaylistViewing.Songs
                .IndexOf(GlobalViewModel.Instance.PlaylistsManager.PlaylistViewing.Songs.Where(s => s.Id == SongId).FirstOrDefault());
            Timer.Start();

            AppWindowExtensions.GetMainWindow().MouseMove += WindowMouseMove;
            AppWindowExtensions.GetMainWindow().PreviewMouseUp += WindowPreviewMouseUp;
        }

        private void AutoScrollTimerTick(object sender, EventArgs e)
        {
            // double newVerticalOffset = Math.Clamp(songsScroller.VerticalOffset + (AutoScrollUp ? 5.0 : -5.0), 0.0, songsScroller.ScrollableHeight);

            if (AutoScrollUp)
            {
                double newVerticalOffset = songsScroller.VerticalOffset - (5.0 * (7.0 / 100.0 * Math.Abs(UnClamppedVisualY)));
                if (newVerticalOffset < 0.0)
                {
                    newVerticalOffset = 0.0;
                    AutoScrollTimer.Stop();
                }

                songsScroller.ScrollToVerticalOffset(newVerticalOffset);
            }
            else
            {
                double newVerticalOffset = songsScroller.VerticalOffset + (5.0 * (7.0 / 100.0 * (UnClamppedVisualY - MaxCanvasY)));
                if (newVerticalOffset > songsScroller.ScrollableHeight)
                {
                    newVerticalOffset = songsScroller.ScrollableHeight;
                    AutoScrollTimer.Stop();
                }
                songsScroller.ScrollToVerticalOffset(newVerticalOffset);
            }

            MouseMoved(null);
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
                Point mousePos;
                if (e != null)
                {
                    mousePos = e.GetPosition(canvas);
                }
                else
                {
                    mousePos = Mouse.GetPosition(canvas);
                }
                
                // For now cap on heigth
                double elementHalfHeight = toMove.ActualHeight / 2.0;
                double marginBetweenElements = 8.0;

                MaxCanvasY = canvas.ActualHeight - elementHalfHeight - 24.0;
                UnClamppedVisualY = mousePos.Y - elementHalfHeight;
                double visualY = Math.Clamp(UnClamppedVisualY, 0.0, MaxCanvasY);
                double actualY = Math.Clamp(mousePos.Y + songsScroller.VerticalOffset, 0.0, songsScroller.ScrollableHeight + MaxCanvasY);

                // Scroll Up
                if (UnClamppedVisualY < 0.0)
                {
                    if (!AutoScrollTimer.IsEnabled)
                    {
                        AutoScrollUp = true;
                        AutoScrollTimer.Start();
                    }
                }
                else if (AutoScrollTimer.IsEnabled && AutoScrollUp)
                {
                    AutoScrollTimer.Stop();
                }

                // Scroll Down
                if (UnClamppedVisualY > MaxCanvasY)
                {
                    if (!AutoScrollTimer.IsEnabled)
                    {
                        AutoScrollUp = false;
                        AutoScrollTimer.Start();
                    }
                }
                else if (AutoScrollTimer.IsEnabled && !AutoScrollUp)
                {
                    AutoScrollTimer.Stop();
                }

                ElementsPassed = int.Parse(Math.Floor(actualY / (toMove.ActualHeight + marginBetweenElements)).ToString());

                if (PreviousMoveIndex != ElementsPassed)
                {
                    // Move Songs
                    GlobalViewModel.Instance.SwapSongInPlaylist(GlobalViewModel.Instance.PlaylistsManager.PlaylistViewing, PreviousMoveIndex, ElementsPassed);

                    // Reset
                    PreviousMoveIndex = ElementsPassed;
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

                // TODO2
                GlobalViewModel.Instance.SwapSongInPlaylist(GlobalViewModel.Instance.PlaylistsManager.PlaylistViewing, FirstMoveIndex, ElementsPassed);
            }
        }
    }
}
