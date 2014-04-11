using DataAccess;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheDaveSite.Utils;

namespace TheDaveSite.Controllers
{
    public class DatabaseImageController : Controller
    {
        //
        // GET: /DatabaseImage/

        private static object Locker = new object();

        public ActionResult GetFullsizeDatabaseImage(int id)
        {
            using (var proxy = Proxies.DataAccessProxyInstance)
            {
                var imageData = proxy.GetFullImage(id);

                return new ImageResult()
                {
                    ImageData = imageData
                };
            }
        }

        public ActionResult GetViewerDatabaseImage(int id)
        {
            using (var proxy = Proxies.DataAccessProxyInstance)
            {
                var imageData = proxy.GetFullImage(id);

                return new ImageResult()
                {
                    ImageData = imageData
                };
            }
        }

        public ActionResult GetDatabaseImageThumbnail(int id)
        {
            using (var proxy = Proxies.DataAccessProxyInstance)
            {
                var imageData = proxy.GetThumbnailImage(id);

                return new ImageResult()
                {
                    ImageData = imageData
                };
            }
        }
    }

    public class ImageResult : ActionResult
    {
        public ImageResult() { }

        public StoredImageProperties ImageData { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            // verify properties
            if (ImageData == null)
            {
                throw new ArgumentNullException("Image");
            }

            // output
            context.HttpContext.Response.Clear();

            context.HttpContext.Response.ContentType = ImageData.DataFormat;
            context.HttpContext.Response.OutputStream.Write(ImageData.ImageData.ByteData, 0, ImageData.ImageData.ByteData.Length);
        }
    } 
}
