using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace BreezeShop.Core.FileFactory.UploadMethod
{
    public class PictureCore : FilesUpload
    {
        public PictureCore(byte[] file, string fileName)
            : base(file, fileName)
        {
            UploadExtensionName = "jpg,jpeg,gif,png,bmp";
            MaxAttachSize = 4194304;
            GetImageSize();
        }

        /// <summary>
        /// 图片高度
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// 图片宽度
        /// </summary>
        public int Width { get; set; }

        private Image _image;

        /// <summary>
        /// 图片尺寸
        /// </summary>
        protected IDictionary<int, int> ImageSize = new Dictionary<int, int>
        {
            {30, 30},
            {40, 40},
            {60, 60},
            {80, 80},
            {120, 120},
            {160, 160},
            {210, 210},
            {310, 310},
            {460, 460},
            {620, 620},
            {800, 800},
            {1200, 1200},
            {1600, 1600}
        };

        /// <summary>
        /// 图片缩放方式选择
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns></returns>
        private Dimensions ImageSaveType(int width, int height)
        {
            //return Dimensions.Width;
            return (double) _image.Width/width >= (double) _image.Height/height ? Dimensions.Width : Dimensions.Height;
        }

        protected override string GenerateFileName()
        {
            if (FileName.IndexOf('.') > 0)
            {
                NewFileName = DateTime.Now.ToFileTime() + "-" + Width + "x" + Height + GetFileExtensionName(FileName);
                return NewFileName;
            }

            throw new Exception("请输入正确的文件名，包括后缀名");
        }

        /// <summary>
        /// 根据图片后缀名自动对应图片类型
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <returns></returns>
        private static ImageFormat ImageFormatSelect(string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case "jpg": return ImageFormat.Jpeg;
                case "bmp": return ImageFormat.Bmp;
                case "gif": return ImageFormat.Gif;
                case "png": return ImageFormat.Png;
                default: return ImageFormat.Jpeg;
            }
        }

        /// <summary>
        /// 创建缩略图并保存
        /// </summary>
        protected virtual void GenerateThumb()
        {
            foreach (var s in ImageSize)
            {
                if (_image.Width < s.Key && _image.Height < s.Value)
                {
                    _image.Save(InsertAtEndOfFileName(SavePath, "_" + s.Key + "x" + s.Value),
                                ImageFormatSelect(GetFileExtension(SavePath)));
                }
                else
                {
                    ImageHandler.ConstrainProportions(_image, s.Key, ImageSaveType(s.Key, s.Value)).Save(
                        InsertAtEndOfFileName(SavePath, "_" + s.Key + "x" + s.Value),
                        ImageFormatSelect(GetFileExtension(SavePath)));
                }
            }
        }

        private MemoryStream _ms;

        private void GetImageSize()
        {
            _ms = new MemoryStream(File);
            _image = Image.FromStream(_ms);
            Height = _image.Height;
            Width = _image.Width;
        }

        protected override void SaveFile()
        {
            try
            {
                base.SaveFile();
                GenerateThumb();
            }
            finally
            {
                _image.Dispose();
                _ms.Dispose();
            }
        }
    }
}
