using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using BreezeShop.Core.FileFactory.UploadMethod;
using Yun.User.Request;
using Yun.Util;

namespace BreezeShop.Core.FileFactory
{
    public class FileManage
    {
        private static readonly string _fileUploadType = WebConfigurationManager.AppSettings["FileUploadType"];

        /// <summary>
        /// 获取待上传文件的首个文件
        /// </summary>
        /// <returns></returns>
        public static FileItem GetFirstFile()
        {
            var t = GetUploadFile();
            if (t != null && t.Any())
            {
                return t.First();
            }

            return null;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public static IList<string> Upload(IList<FileItem> files, bool absolutePath = false)
        {
            if (files == null || !files.Any())
            {
                return null;
            }

            //远程
            if (_fileUploadType.Equals("REMOTE", StringComparison.CurrentCultureIgnoreCase))
            {
                var req = YunClient.Instance.Execute(new FileUploadRequest { Images = files });
                if (!req.IsError && req.Files != null && req.Files.Any())
                {
                    return req.Files;
                }

                return null;
            }

            var resultData = files.Select(file => new PictureCore(file.GetContent(), file.GetFileName()))
                    .Select(instance => instance.Create())
                    .Where(result => !string.IsNullOrWhiteSpace(result))
                    .ToList();

            if (absolutePath)
            {
                for (var i = 0; i < resultData.Count; i++)
                {
                    resultData[i] = ImageExtension.GetUrl(resultData[i]);
                }
            }

            //本地
            return resultData;

        }

        public static string UploadOneFile()
        {
            var result = Upload(GetUploadFile());
            if (result != null && result.Any())
            {
                return result[0];
            }

            return "";
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        public static IList<string> Upload(bool absolutePath = false)
        {
            return Upload(GetUploadFile(), absolutePath);
        }

        /// <summary>
        /// 从FORM表单中获取需要上传的文件
        /// </summary>
        /// <returns></returns>
        private static IList<FileItem> GetUploadFile()
        {
            var r = new List<FileItem>();
            for (var i = 0; i < HttpContext.Current.Request.Files.Count; i++)
            {
                if (HttpContext.Current.Request.Files[i].ContentLength <= 0) continue;
                var f = HttpContext.Current.Request.Files[i];
                using (var b = new BinaryReader(f.InputStream))
                {
                    r.Add(new FileItem(f.FileName, b.ReadBytes(f.ContentLength)));
                }
            }

            return r;
        }
    }
}
