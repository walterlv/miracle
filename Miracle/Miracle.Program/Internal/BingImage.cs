using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Walterlv.Miracle.Properties;

namespace Walterlv.Miracle.Internal
{
    internal static class BingImage
    {
        internal static string GetBingImageFile()
        {
            // 获取必应主页。
            WebClient client = new WebClient();
            string html = client.DownloadString(Settings.Default.BingUrl);

            // 获取主页背景图片下载地址。
            Match imgurlMatch = Regex.Match(html, Settings.Default.ImageUrlPatternFromBingUrl);
            if (!imgurlMatch.Success)
            {
                return null;
            }
            string imgurl = imgurlMatch.Value;

            // 获取图片名称。
            Match imgnameMatch = Regex.Match(imgurl, Settings.Default.ImageFileNamePatternFromImageUrl);
            Match imgextMatch = Regex.Match(imgurl, "\\.\\w*$");
            if (!imgnameMatch.Success || !imgextMatch.Success)
            {
                return null;
            }
            string imgname = imgnameMatch.Value;
            string imgext = imgextMatch.Value;

            // 生成本地路径。
            string imageFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                Settings.Default.BingImageFolderName);
            if (!Directory.Exists(imageFolder))
            {
                Directory.CreateDirectory(imageFolder);
            }
            string imageFilePath = Path.Combine(imageFolder, imgname + imgext);

            // 下载图片。
            if (File.Exists(imageFilePath))
            {
                return imageFilePath;
            }
            client.DownloadFile(imgurl, imageFilePath);
            return imageFilePath;
        }
    }
}
