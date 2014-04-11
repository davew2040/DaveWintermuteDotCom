using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class StoredImage
    {
        [Key]
        public int Id { get; set; }

        public const int FULLSIZE_MAX_WIDTH = 1280;
        public const int FULLSIZE_MAX_HEIGHT = 900;

        public const int VIEWER_MAX_WIDTH = 500;
        public const int VIEWER_MAX_HEIGHT = 400;

        public const int THUMBNAIL_MAX_WIDTH = 125;
        public const int THUMBNAIL_MAX_HEIGHT = 100;

        public StoredImageProperties ImageThumbnail { get; set; }
        public StoredImageProperties ViewerImage { get; set; }
        public StoredImageProperties FullImage { get; set; }
    }

    public class StoredImageProperties
    {
        [Key]
        public int Id { get; set; }

        public int ActualWidth { get; set; }
        public int ActualHeight { get; set; }
        public string DataFormat { get; set; }
        public StoredImageData ImageData { get; set; }
    }

    public class StoredImageData
    {
        [Key]
        public int Id { get; set; }

        public byte[] ByteData { get; set; }
    }
}
