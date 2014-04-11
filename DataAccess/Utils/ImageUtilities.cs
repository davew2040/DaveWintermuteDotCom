using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;

namespace TheDaveSite.Utils
{
    public class ImageUtilities
    {
        public static ImageFormat DefaultFormat
        {
            get
            {
                return ImageFormat.Jpeg;
            }
        }

        public static string DefaultImageContentType
        {
            get
            {
                return "image/jpeg";
            }
        }

        public static Image ScaleFullsizeImage(Image image)
        {
            // Don't expand an image unnecessarily.
            if (image.Width < StoredImage.FULLSIZE_MAX_WIDTH && image.Height < StoredImage.FULLSIZE_MAX_HEIGHT)
            {
                return image;
            }

            return ScaleImage(image, StoredImage.FULLSIZE_MAX_WIDTH, StoredImage.FULLSIZE_MAX_HEIGHT);
        }

        public static Image ScaleViewerImage(Image image)
        {
            return ScaleImage(image, StoredImage.VIEWER_MAX_WIDTH, StoredImage.VIEWER_MAX_HEIGHT);
        }

        public static Image ScaleThumbnailImage(Image image)
        {
            return ScaleImage(image, StoredImage.THUMBNAIL_MAX_WIDTH, StoredImage.THUMBNAIL_MAX_HEIGHT);
        }

        public static Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);
            Graphics g = Graphics.FromImage(newImage);

            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            g.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }
    }
}