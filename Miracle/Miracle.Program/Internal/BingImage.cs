using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Walterlv.Miracle.Internal
{
    internal static class BingImage
    {
        internal static string GetBingImageFile()
        {
            // 获取必应主页。
            WebClient client = new WebClient();
            string html = client.DownloadString("http://cn.bing.com/");

            // 获取主页背景图片下载地址。
            Match imgurlMatch = Regex.Match(html,
                "(?<=(g_img={.*?url:'))http://.*?\\.jpg(?=('.*}))|(?<=(<div\\sid=\"bgDiv\".*?style='.*?background-image:\\s*url\\(\"))(http://.*?\\.jpg)(?=(\"\\);.*?'.*?>))");
            if (!imgurlMatch.Success)
            {
                return null;
            }
            string imgurl = imgurlMatch.Value;

            // 获取图片名称。
            Match imgnameMatch = Regex.Match(imgurl, "(?<=/)\\w*(?=_.*?\\.jpg)");
            if (!imgnameMatch.Success)
            {
                return null;
            }
            string imgname = imgnameMatch.Value;

            // 生成本地路径。
            string imageFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "必应");
            if (!Directory.Exists(imageFolder))
            {
                Directory.CreateDirectory(imageFolder);
            }
            string imageFilePath = Path.Combine(imageFolder, imgname + ".jpg");

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
