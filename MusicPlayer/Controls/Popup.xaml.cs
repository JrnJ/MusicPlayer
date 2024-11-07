using MusicPlayer.Popups;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace MusicPlayer.Controls
{
    /// <summary>
    /// Interaction logic for Popup.xaml
    /// </summary>
    public partial class Popup : UserControl
    {
        public Popup()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty MyContentProperty =
            DependencyProperty.Register("MyContent", typeof(object), typeof(Popup), new PropertyMetadata(null));

        public object MyContent
        {
            get => GetValue(MyContentProperty);
            set => SetValue(MyContentProperty, value);
        }

        public static readonly DependencyProperty ActionsProperty = 
            DependencyProperty.Register("Actions", typeof(object), typeof(Popup), new PropertyMetadata(null));

        public object Actions
        {
            get => GetValue(ActionsProperty);
            set => SetValue(ActionsProperty, value);
        }
    }
}
