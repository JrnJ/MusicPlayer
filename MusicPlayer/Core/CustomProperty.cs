using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MusicPlayer.Core
{
    public abstract class CustomProperty : DependencyObject
    {
        #region ImageSource
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.RegisterAttached(
            "ImageSource",
            typeof(ImageSource),
            typeof(CustomProperty)
            );

        public static void SetImageSource(UIElement element, ImageSource value)
        {
            element.SetValue(ImageSourceProperty, value);
        }

        public static ImageSource GetImageSource(UIElement element)
        {
            return (ImageSource)element.GetValue(ImageSourceProperty);
        }
        #endregion ImageSource

        #region HexToBrush
        public static readonly DependencyProperty HexToBrushProperty = DependencyProperty.RegisterAttached(
            "HexToBrush",
            typeof(SolidColorBrush),
            typeof(CustomProperty)
            );

        public static void SetHexToBrush(UIElement element, SolidColorBrush value)
        {
            element.SetValue(ImageSourceProperty, (SolidColorBrush)new BrushConverter().ConvertFrom(value.ToString()));
        }

        public static SolidColorBrush GetHexToBrush(UIElement element)
        {
            return (SolidColorBrush)element.GetValue(ImageSourceProperty);
        }
        #endregion HexToBrush
    }
}
