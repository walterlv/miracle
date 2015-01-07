using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Walterlv.Miracle.Internal;

namespace Walterlv.Miracle
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Task task = new Task(() =>
            {
                string fileName = BingImage.GetBingImageFile();
                if (fileName == null || !File.Exists(fileName))
                {
                    return;
                }
                Dispatcher.BeginInvoke(new Action(() =>
                    BackgroundImageBrush.ImageSource = new BitmapImage(new Uri(fileName, UriKind.Absolute))));
            });
            task.Start();
        }
    }
}
