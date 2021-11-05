using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MusicPlayer.Classes
{
    public class Debugger
    {
        // Classes
        private Window _window;

        // Privates
        private string _debuggerName;
        private string _debuggerTitleName;
        private string _debuggerTextName;

        // Properties
        private string _title;

        public string Title
        {
            get { return _title; }
            set 
            { 
                _title = value;

                (_window.FindName(_debuggerTitleName) as TextBlock).Text = Title;
            }
        }

        // Constructor
        public Debugger(Window window)
        {
            if (window != null)
            {
                _window = window;
                ChangeCurrentWindow();
            }
        }

        #region Setup
        /// <summary>
        /// Changes the Window code to add the DebuggingConsole
        /// </summary>
        private void ChangeCurrentWindow()
        {
            // Get old Window content
            Grid oldPage = new Grid(); 
            oldPage = _window.Content as Grid;
            _window.Content = null;

            // Create grid is container
            Grid windowContainer = new Grid();
            windowContainer.Children.Add(oldPage);

            // Add console
            windowContainer.Children.Add(CreateDebuggingConsole());

            // Apply changes to page
            Page newPage = new Page { Content = windowContainer };
            _window.Content = newPage;
        }

        /// <summary>
        /// Creates the UI for the DebuggingConsole
        /// </summary>
        /// <returns>An DebuggingConsole element</returns>
        private Canvas CreateDebuggingConsole()
        {
            Canvas debuggingConsole = new Canvas();
            debuggingConsole.Name = RegisterName("canvasDebugger", debuggingConsole);
            _debuggerName = debuggingConsole.Name;

            Border consoleBorder = new Border()
            {
                BorderBrush = new SolidColorBrush(Color.FromRgb(48, 48, 48)),
                BorderThickness = new Thickness(1)
            };

            Grid console = new Grid()
            {
                Width = 980,
                Height = 512,
            };

            #region RowDefenitions
            RowDefinition row0 = new RowDefinition()
            {
                Height = new GridLength(30, GridUnitType.Pixel)
            };

            RowDefinition row1 = new RowDefinition();

            // Set rows
            console.RowDefinitions.Add(row0);
            console.RowDefinitions.Add(row1);
            #endregion RowDefenitions

            Grid consoleFrame = new Grid()
            {
                Background = new SolidColorBrush(Color.FromRgb(0, 0, 0))
            };
            Grid.SetRow(consoleFrame, 0); // Grid.Row = "0";

            Grid consoleFrameInner = new Grid()
            {
                VerticalAlignment = VerticalAlignment.Center
            };
            

            TextBlock consoleFrameTitle = new TextBlock()
            {
                Text = _window.Title
            };
            consoleFrameTitle.Name = RegisterName("tblDebuggerTitle", consoleFrameTitle);
            _debuggerTitleName = consoleFrameTitle.Name;

            TextBlock consoleText = new TextBlock()
            {
                Background = new SolidColorBrush(Color.FromRgb(12, 12, 12)),
                Foreground = new SolidColorBrush(Color.FromRgb(204, 204, 204)),
                FontSize = 14,
                FontFamily = new FontFamily("Consolas"),
                Text = "Debugger started!\n\n"
            };
            consoleText.Name = RegisterName("tblDebuggerText", consoleText);
            _debuggerTextName = consoleText.Name;
            Grid.SetRow(consoleText, 1); // Grid.Row = "1";

            // Set children
            consoleFrameInner.Children.Add(consoleFrameTitle);
            consoleFrame.Children.Add(consoleFrameInner);

            console.Children.Add(consoleFrame);
            console.Children.Add(consoleText);

            consoleBorder.Child = console;

            debuggingConsole.Children.Add(consoleBorder);

            return debuggingConsole;
        }
        #endregion Setup

        #region Events
        /// <summary>
        /// Writes text to the Debugger
        /// </summary>
        /// <param name="text">Text to write</param>
        public void Write(string? text)
        {
            (_window.FindName(_debuggerTextName) as TextBlock).Text += text;
        }

        /// <summary>
        /// Writes a line of text to the Debugger
        /// </summary>
        /// <param name="text">Text to write</param>
        public void WriteLine(string text)
        {
            (_window.FindName(_debuggerTextName) as TextBlock).Text += text + "\n";
        }

        /// <summary>
        /// Enable or disable the Debugger UI
        /// </summary>
        /// <param name="value">true or false</param>
        public void SetActive(bool value)
        {
            (_window.FindName(_debuggerName) as Canvas).Visibility = value ? Visibility.Visible : Visibility.Hidden;
        }
        #endregion Events

        #region HelperMethods
        private string RegisterName(string name, object obj)
        {
            Random rnd = new Random();

            while (true)
            {
                if (_window.FindName(name) == null)
                    break;

                name += rnd.Next(0, 9);
            }

            _window.RegisterName(name, obj);

            return name;
        }


        #endregion HelperMethods
    }
}
