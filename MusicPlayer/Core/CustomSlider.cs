﻿using System;
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
    internal class CustomSlider : Slider
    {
        public static readonly DependencyProperty ClickedInSliderProperty = DependencyProperty.Register(
            "ClickedInSlider",
            typeof(bool),
            typeof(CustomSlider),
            new FrameworkPropertyMetadata(
                false, 
                new PropertyChangedCallback(OnClickedInSliderChanged)
                )
        );

        public bool ClickedInSlider
        {
            get => (bool)GetValue(ClickedInSliderProperty);
            set => SetValue(ClickedInSliderProperty, value);
        }

        private static void OnClickedInSliderChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CustomSlider element = (CustomSlider)sender;
            element.ClickedInSlider = (bool)e.NewValue;
        }

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
                //SetValue(ClickedInSliderProperty, true);
                ClickedInSlider = true;
            }), true);

            this.AddHandler(PreviewMouseLeftButtonUpEvent, new RoutedEventHandler((sender, args) =>
            {
                //SetValue(ClickedInSliderProperty, false);
                ClickedInSlider = false;
            }), true);
        }
    }
}
