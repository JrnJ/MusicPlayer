using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Core
{
    public static class ThemesController
    {
        public enum Themes
        {
            Light, 
            Dark, 
            Custom
        }

        public static Themes CurrentTheme { get; private set; }

        private static ResourceDictionary ThemeDictionary
        {
            get { return Application.Current.Resources.MergedDictionaries[0]; }
            set { Application.Current.Resources.MergedDictionaries[0] = value; }
        }

        private static void ChangeTheme(Uri uri)
        {
            ThemeDictionary = new ResourceDictionary() { Source = uri };
        }

        public static void SetTheme(Themes theme, string themeName = null)
        {
            switch (theme)
            {
                case Themes.Light:
                    ChangeTheme(new Uri($"Themes/Light.xaml", UriKind.Relative));
                    break;
                case Themes.Dark:
                    ChangeTheme(new Uri($"Themes/Dark.xaml", UriKind.Relative));
                    break;
                case Themes.Custom:
                    // Needs some work
                    break;
            }
        }
    }
}
