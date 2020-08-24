using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
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
using Vlc.DotNet.Wpf;

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DirectoryInfo vlcLibDirectory;
        private VlcControl control;
        private string filePath;
        public MainWindow()
        {
            InitializeComponent(); 
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            // Default installation path of VideoLAN.LibVLC.Windows
            vlcLibDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            this.control?.Dispose();
            base.OnClosing(e);
        }
        private string GetFileLocation()
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            else
            {
                return "";
            }
        }
        private void OnOpenFileClick(object sender, RoutedEventArgs e)
        {
            this.filePath = this.GetFileLocation();
        }

        private void OnPlayButtonClick(object sender, RoutedEventArgs e)
        {
            this.control?.Dispose();
            this.control = new VlcControl();
            this.ControlContainer.Content = this.control;
            this.control.SourceProvider.CreatePlayer(this.vlcLibDirectory);

            // This can also be called before EndInit
            this.control.SourceProvider.MediaPlayer.Log += (_, args) =>
            {
                string message = $"libVlc : {args.Level} {args.Message} @ {args.Module}";
                System.Diagnostics.Debug.WriteLine(message);
            };
            if (String.IsNullOrEmpty(this.filePath))
            {
                this.filePath = this.GetFileLocation();
            }
            control.SourceProvider.MediaPlayer.Play(new Uri(@filePath));
        }

        private void OnStopButtonClick(object sender, RoutedEventArgs e)
        {
            this.control?.Dispose();
            this.control = null;
        }

        private void OnForwardButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.control == null)
            {
                return;
            }

            this.control.SourceProvider.MediaPlayer.Rate = 2;
        }

        private void GetLength_Click(object sender, RoutedEventArgs e)
        {
            if (this.control == null)
            {
                return;
            }

            GetLength.Content = this.control.SourceProvider.MediaPlayer.Length + " ms";
        }

        private void GetCurrentTime_Click(object sender, RoutedEventArgs e)
        {
            if (this.control == null)
            {
                return;
            }

            GetCurrentTime.Content = this.control.SourceProvider.MediaPlayer.Time + " ms";
        }

        private void SetCurrentTime_Click(object sender, RoutedEventArgs e)
        {
            if (this.control == null)
            {
                return;
            }

            this.control.SourceProvider.MediaPlayer.Time = 5000;
            SetCurrentTime.Content = this.control.SourceProvider.MediaPlayer.Time + " ms";
        }
    }
}
