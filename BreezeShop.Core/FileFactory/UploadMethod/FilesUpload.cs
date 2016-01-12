using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Configuration;
using Utilities.DataTypes.ExtensionMethods;

namespace BreezeShop.Core.FileFactory.UploadMethod
{
    public class FilesUpload
    {
        public FilesUpload(byte[] file, string fileName)
        {
            File = file;
            FileName = fileName.IndexOf(@"\") > 0 ? fileName.Substring(fileName.IndexOf(@"\") + 1) : fileName;

            Size = file.LongLength;
            MaxAttachSize = 2097152;
        }

        public byte[] File;
        public string FileName;
        protected string UploadExtensionName = "jpg,jpeg,gif,png,bmp,rar,zip,7z";
        public long Size;

        internal static readonly string ImageFolder = WebConfigurationManager.AppSettings["ImageFolder"];

        /// <summary>
        /// 默认地址是当前程序下的文件夹
        /// </summary>
        public static readonly string Attachdir =
            string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["FileAbsolutePath"])
                ? HttpContext.Current.Server.MapPath("~/" + ImageFolder)
                : (ConfigurationManager.AppSettings["FileAbsolutePath"]);

        /// <summary>
        /// 默认返回是城购云商的地址
        /// </summary>
        public static readonly string WebPath = ConfigurationManager.AppSettings["FileWebPath"];

        public long MaxAttachSize { get; set; }
        public string Error { get; set; }
        public string SavePath { get; set; }

        public string NewFileName;

        /// <summary>
        /// 检测文件
        /// </summary>
        /// <returns></returns>
        protected bool CheckFile()
        {
            return File != null && File.Length != 0 && File.Length <= MaxAttachSize;
        }

        protected virtual string CreateDirectory()
        {
            var now = DateTime.Now;
            var path = Attachdir + now.Year + @"\" + now.Month.ToString().PadLeft(2, '0') + now.Day.ToString().PadLeft(2, '0');
            Directory.CreateDirectory(path);
            return path + @"\";
        }

        internal static string GetFileExtensionName(string fileName)
        {
            return fileName.Contains(".") ? fileName.Substring(fileName.LastIndexOf("."), fileName.Length - fileName.LastIndexOf(".")) : "";
        }

        internal static string InsertAtEndOfFileName(string originalFileName, string insertString)
        {
            if (string.IsNullOrEmpty(originalFileName)) return string.Empty;

            var index = originalFileName.LastIndexOf(".", StringComparison.Ordinal); //是否是文件名
            return index >= 0 ? originalFileName.Insert(index, insertString) : string.Empty;
        }

        protected virtual string GenerateFileName()
        {
            if (FileName.IndexOf('.') > 0)
            {
                NewFileName = DateTime.Now.ToFileTime() + GetFileExtensionName(FileName);

                return NewFileName;
            }

            throw new Exception("请输入正确的文件名，包括后缀名");
        }

        protected void GenerateFileSavePath()
        {
            if (SavePath.IsNullOrEmpty())
            {
                SavePath = CreateDirectory() + GenerateFileName();
            }
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        protected virtual void SaveFile()
        {
            using (var fs = new FileStream(SavePath, FileMode.Create, FileAccess.Write))
            {
                fs.Write(File, 0, File.Length);
                fs.Flush();
            }
        }

        /// <summary>
        /// 检测文件后缀名
        /// </summary>
        /// <returns></returns>
        private bool CheckFileExtension()
        {
            var extension = GetFileExtension(FileName);
            if (("," + UploadExtensionName + ",").IndexOf("," + extension + ",", StringComparison.Ordinal) < 0)
            {
                Error = "上传文件扩展名必需为：" + UploadExtensionName;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取文件后缀名
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        protected virtual string GetFileExtension(string fullPath)
        {
            return fullPath != "" ? fullPath.Substring(fullPath.LastIndexOf('.') + 1).ToLower() : "";
        }

        /// <summary>
        /// 创建并保存文件
        /// </summary>
        /// <returns></returns>
        public virtual string Create()
        {
            if (CheckFile() && CheckFileExtension())
            {
                GenerateFileSavePath();
                SaveFile();

                if (SavePath.IndexOf(Attachdir) >= 0)
                {
                    return SavePath.Replace(Attachdir, WebPath).Replace(@"\", "/");
                }

                return WebPath + (SavePath.IndexOf(@"\") >= 0 ? SavePath.Replace(@"\", "/") : SavePath);
            }

            return string.Empty;
        }

    }
}
