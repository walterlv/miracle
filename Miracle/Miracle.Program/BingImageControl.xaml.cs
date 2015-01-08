using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Walterlv.Miracle.Internal;
using Walterlv.Miracle.Properties;

namespace Walterlv.Miracle
{
    /// <summary>
    /// 一个用于在线获取必应图片显示的控件。
    /// </summary>
    public partial class BingImageControl : UserControl
    {
        /// <summary>
        /// 创建 <see cref="BingImageControl"/> 的新实例。
        /// </summary>
        public BingImageControl()
        {
            InitializeComponent();
            Loaded += (sender, args) => LoadBingImage();
        }

        /// <summary>
        /// 加载必应图片。
        /// </summary>
        private void LoadBingImage()
        {
            // 读取最近一次使用的必应图片文件。
            string fileName = Settings.Default.RecentBingImageFileName;
            
            // 如果图片文件存在，则暂时使用，否则暂不使用。
            if (File.Exists(fileName))
            {
                Background = new ImageBrush(LoadImageFile(fileName))
                {
                    Stretch = TempBackgroundImageBrush.Stretch,
                };
            }
            else
            {
                fileName = String.Empty;
            }

            // 准备在异步下载完新的必应图片后，将新的图片替代旧的显示。
            Action<string> loadNewBingImage = newFileName =>
            {
                if (newFileName == null || newFileName == fileName || !File.Exists(newFileName))
                {
                    return;
                }
                TempBackgroundImageBrush.ImageSource = LoadImageFile(newFileName);
                Settings.Default.RecentBingImageFileName = newFileName;
                Settings.Default.Save();
                FadeOutStoryboard.Begin();
            };

            // 异步下载必应图片。
            Task task = new Task(() =>
            {
                string newFileName;
                try
                {
                    newFileName = BingImage.GetBingImageFile();
                }
                catch (WebException)
                {
                    Dispatcher.BeginInvoke(new Action(() => ReportErrorStoryboard.Begin()));
                    return;
                }
                Dispatcher.BeginInvoke(loadNewBingImage, newFileName);
            });
            task.Start();
        }

        /// <summary>
        /// 当新图片显示完成后，将临时图层删除。
        /// </summary>
        private void OnFadeOutStoryboardCompleted(object sender, EventArgs e)
        {
            Background = TempBackgroundBorder.Background;
            Content = null;
        }

        /// <summary>
        /// 获取用于显示新图片的故事板。
        /// </summary>
        private Storyboard FadeOutStoryboard
        {
            get { return (Storyboard) FindResource("FadeOutTempBackgroundStoryboard"); }
        }

        /// <summary>
        /// 获取用于显示网络错误的故事板。
        /// </summary>
        private Storyboard ReportErrorStoryboard
        {
            get { return (Storyboard)FindResource("ReportErrorStoryboard"); }
        }

        /// <summary>
        /// 从本地加载一个文件到内存中形成 <see cref="BitmapImage"/> 对象。
        /// </summary>
        /// <param name="fileName">文件路径。</param>
        /// <returns><see cref="BitmapImage"/> 对象。</returns>
        private static BitmapImage LoadImageFile(string fileName)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                byte[] bytes = new BinaryReader(fileStream).ReadBytes((int) fileStream.Length);
                MemoryStream stream = new MemoryStream(bytes);
                fileStream.CopyTo(stream);
                bitmap.StreamSource = stream;
            }
            bitmap.CacheOption = BitmapCacheOption.None;
            bitmap.EndInit();
            return bitmap;
        }
    }
}
