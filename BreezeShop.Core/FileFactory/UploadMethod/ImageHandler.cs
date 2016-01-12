using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace BreezeShop.Core.FileFactory.UploadMethod
{

    public enum Dimensions
    {
        Width,
        Height
    }

    public enum AnchorPosition
    {
        Top,
        Center,
        Bottom,
        Left,
        Right
    }

    public class ImageHandler
    {
        public static Image ScaleByPercent(Image imgPhoto, int Percent)
        {
            var nPercent = ((float)Percent / 100);

            var sourceWidth = imgPhoto.Width;
            var sourceHeight = imgPhoto.Height;
            var sourceX = 0;
            var sourceY = 0;

            var destX = 0;
            var destY = 0;
            var destWidth = (int)(sourceWidth * nPercent + 0.5);
            var destHeight = (int)(sourceHeight * nPercent + 0.5);

            var bmPhoto = new Bitmap(destWidth, destHeight, PixelFormat.Format32bppArgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);
            bmPhoto.MakeTransparent();
            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Transparent);
            grPhoto.SmoothingMode = SmoothingMode.HighQuality;
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }

        public static Image ConstrainProportions(Image imgPhoto, int Size, Dimensions Dimension)
        {
            var sourceWidth = imgPhoto.Width;
            var sourceHeight = imgPhoto.Height;
            var sourceX = 0;
            var sourceY = 0;
            var destX = 0;
            var destY = 0;
            double nPercent = 0;

            switch (Dimension)
            {
                case Dimensions.Width:
                    nPercent = (Size / (double)sourceWidth);
                    break;
                default:
                    nPercent = (Size / (double)sourceHeight);
                    break;
            }

            var destWidth = (int)(sourceWidth * nPercent + 0.5);
            var destHeight = (int)(sourceHeight * nPercent + 0.5);

            var bmPhoto = new Bitmap(destWidth, destHeight, PixelFormat.Format32bppArgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);
            bmPhoto.MakeTransparent();
            var grPhoto = System.Drawing.Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Transparent);
            grPhoto.SmoothingMode = SmoothingMode.HighQuality;
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto, new Rectangle(destX, destY, destWidth, destHeight), new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight), GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }

        public static Image FixedSize(Image imgPhoto, int Width, int Height)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);

            //if we have to pad the height pad both the top and the bottom
            //with the difference between the scaled height and the desired height
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = (int)((Width - (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = (int)((Height - (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent + 0.5);
            int destHeight = (int)(sourceHeight * nPercent + 0.5);

            Bitmap bmPhoto = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);
            bmPhoto.MakeTransparent();
            System.Drawing.Graphics grPhoto = System.Drawing.Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Transparent);
            grPhoto.SmoothingMode = SmoothingMode.HighQuality;
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }

        public static Image FixedSize(string imagepath, int Width, int Height)
        {
            var imgPhoto = Image.FromFile(imagepath);
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);

            //if we have to pad the height pad both the top and the bottom
            //with the difference between the scaled height and the desired height
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = (int)((Width - (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = (int)((Height - (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent + 0.5);
            int destHeight = (int)(sourceHeight * nPercent + 0.5);

            Bitmap bmPhoto = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);
            bmPhoto.MakeTransparent();
            System.Drawing.Graphics grPhoto = System.Drawing.Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Transparent);
            grPhoto.SmoothingMode = SmoothingMode.HighQuality;
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            imgPhoto.Dispose();
            grPhoto.Dispose();
            return bmPhoto;
        }

        public static Image Crop(Image imgPhoto, int Width, int Height, AnchorPosition Anchor)
        {
            var sourceWidth = imgPhoto.Width;
            var sourceHeight = imgPhoto.Height;
            var sourceX = 0;
            var sourceY = 0;
            var destX = 0;
            var destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
            {
                nPercent = nPercentW;
                switch (Anchor)
                {
                    case AnchorPosition.Top:
                        destY = 0;
                        break;
                    case AnchorPosition.Bottom:
                        destY = (int)(Height - (sourceHeight * nPercent));
                        break;
                    default:
                        destY = (int)((Height - (sourceHeight * nPercent)) / 2);
                        break;
                }
            }
            else
            {
                nPercent = nPercentH;
                switch (Anchor)
                {
                    case AnchorPosition.Left:
                        destX = 0;
                        break;
                    case AnchorPosition.Right:
                        destX = (int)(Width - (sourceWidth * nPercent));
                        break;
                    default:
                        destX = (int)((Width - (sourceWidth * nPercent)) / 2);
                        break;
                }
            }

            int destWidth = (int)(sourceWidth * nPercent + 0.5);
            int destHeight = (int)(sourceHeight * nPercent + 0.5);

            var bmPhoto = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);
            bmPhoto.MakeTransparent();
            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Transparent);
            grPhoto.SmoothingMode = SmoothingMode.HighQuality;
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }
    }
}
