using System;
using System.Collections.Generic;
using System.Web.Configuration;
using BreezeShop.Core.FileFactory.UploadMethod;
using Utilities.DataTypes.ExtensionMethods;

namespace BreezeShop.Core.FileFactory
{   
    /// <summary>
    /// 图片尺寸
    /// </summary>
    public enum ImageSize
    {
        _30x30 = 0,
        _40x40 = 1,
        _60x60 = 2,
        _80x80 = 3,
        _120x120 = 4,
        _160x160 = 5,
        _210x210 = 6,
        _310x310 = 7,
        _460x460 = 8,
        _620x620 = 9,
        _800x800 = 10,
        _1200x1200 = 11,
        _1600x1600 = 12
    }

    public class ImageExtension
    {
        private static readonly IDictionary<string, ImageSize> _imageTextFileSizeDictionary = new Dictionary<string, ImageSize>
        {
            {"30", ImageSize._30x30},
            {"40", ImageSize._40x40},
            {"50", ImageSize._60x60},
            {"80", ImageSize._80x80},
            {"120",ImageSize._120x120},
            {"160", ImageSize._160x160},
            {"210", ImageSize._210x210},
            {"310", ImageSize._310x310},
            {"460", ImageSize._460x460},
            {"620", ImageSize._620x620},
            {"800", ImageSize._800x800},
            {"1200", ImageSize._1200x1200},
            {"1600", ImageSize._1600x1600}
        };

        private static readonly IDictionary<ImageSize, string> _imageFileSizeDictionary = new Dictionary<ImageSize, string>
        {
            {ImageSize._30x30, "_30x30"},
            {ImageSize._40x40, "_40x40"},
            {ImageSize._60x60, "_60x60"},
            {ImageSize._80x80, "_80x80"},
            {ImageSize._120x120, "_120x120"},
            {ImageSize._160x160, "_160x160"},
            {ImageSize._210x210, "_210x210"},
            {ImageSize._310x310, "_310x310"},
            {ImageSize._460x460, "_460x460"},
            {ImageSize._620x620, "_620x620"},
            {ImageSize._800x800, "_800x800"},
            {ImageSize._1200x1200, "_1200x1200"},
            {ImageSize._1600x1600, "_1600x1600"}
        };

        /// <summary>
        /// 获取图片的高度
        /// </summary>
        /// <param name="size"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static int GetImageHeight(ImageSize size, string fileName)
        {
            var width =
                _imageFileSizeDictionary[size].Substring(_imageFileSizeDictionary[size].LastIndexOf('x') + 1).TryTo(0);

            if (!fileName.IsNotNullOrEmpty() || fileName.IndexOf('-') <= 0) return width;

            var tStr = fileName.Substring(fileName.IndexOf('-') + 1);
            tStr = tStr.Remove(tStr.IndexOf('.'));
            var originalWidth = tStr.Substring(0, tStr.IndexOf('x')).TryTo(0);
            var originalHeight = tStr.Substring(tStr.IndexOf('x') + 1).TryTo(0);
            return (int) ((width/(double) originalWidth)*originalHeight);
        }

        /// <summary>
        /// 往文件名的末尾加入字符串
        /// </summary>
        /// <param name="originalFileName">文件名</param>
        /// <param name="insertString">待加入的字符串</param>
        /// <returns></returns>
        private static string InsertAtEndOfFileName(string originalFileName, string insertString)
        {
            if (string.IsNullOrEmpty(originalFileName)) return string.Empty;

            if (originalFileName.EndsWith("jpg", StringComparison.CurrentCultureIgnoreCase) || originalFileName.EndsWith("jpeg", StringComparison.CurrentCultureIgnoreCase) ||
                originalFileName.EndsWith("png", StringComparison.CurrentCultureIgnoreCase) || originalFileName.EndsWith("gif", StringComparison.CurrentCultureIgnoreCase))
            {
                var index = originalFileName.LastIndexOf(".", StringComparison.Ordinal); //是否是文件名
                return index >= 0 ? originalFileName.Insert(index, insertString) : string.Empty;
            }

            return originalFileName;
        }

        public static string GetUrl(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = YunClient.WebUrl + "/content/images/none-image.png";
            }

            //输入的文件名是否包http,如果不包含，则需要增加前缀
            if (!fileName.StartsWith("http"))
            {
                fileName = string.Format("{0}{1}{2}", YunClient.WebUrl, FilesUpload.ImageFolder, fileName);
            }

            return fileName;
        }

        public static string GetUrl(ImageSize? size, string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = YunClient.WebUrl + "/content/images/none-image.png";
            }

            //输入的文件名是否包http,如果不包含，则需要增加前缀
            if (!fileName.StartsWith("http"))
            {
                fileName = string.Format("{0}{1}{2}", YunClient.WebUrl, FilesUpload.ImageFolder, fileName);
            }

            return size == null
                ? fileName
                : InsertAtEndOfFileName(fileName, _imageFileSizeDictionary[(ImageSize) size]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetUrl(string size, string fileName)
        {
            return (!string.IsNullOrEmpty(fileName)  && _imageTextFileSizeDictionary.ContainsKey(size))
                ? GetUrl(_imageTextFileSizeDictionary[size], fileName)
                : fileName;
        }
    }
}
