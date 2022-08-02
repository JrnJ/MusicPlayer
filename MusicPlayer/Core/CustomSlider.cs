using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace MusicPlayer.Core
{
    internal class CustomSlider : Slider, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ClickedInSliderProperty = DependencyProperty.Register(
            "ClickedInSlider",
            typeof(bool),
            typeof(CustomSlider),
            new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnClickedOnSliderPropertyChangedCallback))
        );

        public bool ClickedInSlider
        {
            get => (bool)GetValue(ClickedInSliderProperty);
            set => SetValue(ClickedInSliderProperty, value);
        }

        private static void OnClickedOnSliderPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CustomSlider element = (CustomSlider)sender;
            //element.SetValue
            //CustomSlider element = (CustomSlider)sender;
            //if (element != null)
            //{
            //    element.RaiseEvent(new RoutedPropertyChangedEventArgs<bool>((bool)e.OldValue, (bool)e.NewValue, ClickedInSliderChangedEvent));
            //}
            //    //element.OnClickedOnSliderChanged();
        }

        //public static readonly RoutedEvent ClickedInSliderChangedEvent = EventManager.RegisterRoutedEvent(
        //    "ClickedInSliderChanged",
        //    RoutingStrategy.Bubble,
        //    typeof(RoutedEvent),
        //    typeof(CustomSlider));

        //protected virtual void OnClickedOnSliderChanged()
        //{
        //    OnPropertyChanged("ClickedInSlider");
        //}

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && ClickedInSlider)
            {
                Thumb thumb = ((Track)this.Template.FindName("PART_Track", this)).Thumb;
                thumb.RaiseEvent(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left)
                {
                    RoutedEvent = MouseLeftButtonDownEvent,
                    Source = e.Source
                });
            }
        }

        public CustomSlider()
        {
            this.AddHandler(PreviewMouseLeftButtonDownEvent, new RoutedEventHandler((sender, args) =>
            {
                ClickedInSlider = true;
            }), true);

            this.AddHandler(PreviewMouseLeftButtonUpEvent, new RoutedEventHandler((sender, args) =>
            {
                ClickedInSlider = false;
            }), true);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
