using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Utilities.DataTypes.ExtensionMethods;
using Yun.Util;

namespace BreezeShop.Core.FileFactory
{
    public class KindeditorMode
    {
        private const int MaxSize = 2000000;

        public KindeditorMode()
        {
            extTable.Add("image", "gif,jpg,jpeg,png,bmp");
            extTable.Add("flash", "swf,flv");
            extTable.Add("media", "swf,flv,mp3,wav,wma,wmv,mid,avi,mpg,asf,rm,rmvb");
            extTable.Add("file", "doc,docx,xls,xlsx,ppt,htm,html,txt,zip,rar,gz,bz2");

            //最大文件大小
            _content = HttpContext.Current;
            _content.Response.Charset = "UTF-8";
            _disposition = _content.Request.ServerVariables["HTTP_CONTENT_DISPOSITION"];

            if (_disposition != null)
            {
                Html5Upload();
            }
            else
            {
                NormalUpload();
            }
        }

        private readonly HttpContext _content;
        private const string InputName = "imgFile";
        private readonly string _disposition;
        private string _fileName;
        private byte[] _file;
        private string _message;

        /// <summary>
        /// 普通上传模式
        /// </summary>
        private void NormalUpload()
        {
            var postedfile = _content.Request.Files.Get(InputName);
            if (postedfile != null)
            {
                _fileName = postedfile.FileName;
                _file = new Byte[postedfile.ContentLength];

                using (var stream = postedfile.InputStream)
                {
                    stream.Read(_file, 0, postedfile.ContentLength);
                }
            }
        }

        /// <summary>
        /// HTML5上传模式
        /// </summary>
        private void Html5Upload()
        {
            _fileName = Regex.Match(_disposition, "filename=\"(.+?)\"").Groups[1].Value;// 读取原始文件名
            _file = _content.Request.BinaryRead(_content.Request.TotalBytes);
        }

        private readonly Hashtable extTable = new Hashtable();

        public dynamic Upload()
        {
            if (_file == null || _fileName == null || _file.Length > MaxSize)
            {
                _message = "上传文件大小超过限制。限制为2MB";
            }

            var dirName = _content.Request.QueryString["dir"];
            var fileExt = (Path.GetExtension(_fileName) ?? "").ToLower();
            if (string.IsNullOrEmpty(fileExt) ||
                Array.IndexOf(((string) extTable[dirName]).Split(','), fileExt.Substring(1).ToLower()) == -1)
            {
                _message = "上传文件扩展名是不允许的扩展名。\n只允许" + ((string) extTable[dirName]) + "格式。";
            }

            if (_message.IsNullOrEmpty())
            {
                var files = FileManage.Upload(new[] {new FileItem(_fileName, _file)});

                if (files != null && files.Any())
                {
                    return new { error = 0, url = files[0] };
                }
            }

            return new {error = 1, message = _message};
        }

    }
}
