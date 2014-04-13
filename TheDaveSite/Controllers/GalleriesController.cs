using DataAccess;
using System.Web.Mvc;
using System.Collections;
using System.Linq;
using TheDaveSite.ViewModels.MessageBoard;
using System;
using System.Web;
using System.Collections.Generic;
using System.IO;
using TheDaveSite.ViewModels.Galleries;
using TheDaveSite.Models;
using SendGridMail;
using System.Net.Mail;
using System.Net;
using TheDaveSite.Utils;
using WebMatrix.WebData;

namespace TheDaveSite.Controllers
{
    public class GalleriesController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            MessageBoardIndexViewModel viewModel = new MessageBoardIndexViewModel();
            using (var proxy = Proxies.DataAccessProxyInstance)
            {
                viewModel.MessageBoards = proxy.GetAllMessageBoards();
            }

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Galleries()
        {
            using (var proxy = Proxies.DataAccessProxyInstance)
            {
                var galleries = proxy.GetAllGalleries();

                return View(galleries);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult EditGallery(int id)
        {
            using (var proxy = Proxies.DataAccessProxyInstance)
            {
                var gallery = proxy.GetGallery(id);

                var viewModel = new GalleryAddEditModel()
                {
                    GalleryId = gallery.Id,
                    Description = gallery.Description,
                    Title = gallery.Title,
                    IsPublic = gallery.IsPublic
                };

                return View(viewModel);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult CreateGallery()
        {
            using (var proxy = Proxies.DataAccessProxyInstance)
            {
                var newModel = new GalleryAddEditModel()
                {
                    Title = "",
                    IsPublic = false,
                    Description = ""
                };

                return View(newModel);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateGallery(GalleryAddEditModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int newGalleryId = 0;

            using (var proxy = Proxies.DataAccessProxyInstance)
            {
                newGalleryId = proxy.CreateGallery(
                    new DataAccess.GalleryProperties()
                    {
                        Description = model.Description,
                        Title = model.Title,
                        IsPublic = model.IsPublic
                    });
            }

            return RedirectToAction("ViewGallery", new { id = newGalleryId });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditGallery(GalleryAddEditModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var proxy = Proxies.DataAccessProxyInstance)
            {
                proxy.EditGallery(model.GalleryId.Value, 
                    new DataAccess.GalleryProperties()
                    {
                        Description = model.Description,
                        Title = model.Title,
                        IsPublic = model.IsPublic
                    });
            }

            return RedirectToAction("ViewGallery", new { id = model.GalleryId });
        }

        [HttpGet]
        public ActionResult ViewGallery(int id, int imageId = 0)
        {
            using (var proxy = Proxies.DataAccessProxyInstance)
            {
                ViewGallery_ViewModel viewModel = new ViewGallery_ViewModel()
                {
                    Gallery = proxy.GetGallery(id),
                    ViewImageId = imageId
                };

                return View(viewModel);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult ManageGallery(int id)
        {
            using (var proxy = Proxies.DataAccessProxyInstance)
            {
                var gallery = proxy.GetGallery(id);

                return View(gallery);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public JsonResult ReorderImages(int galleryId, string newOrder)
        {
            using (var proxy = Proxies.DataAccessProxyInstance)
            {
                proxy.SetGalleryOrder(galleryId, newOrder);
            }

            return new JsonResult()
            {
                Data = new
                {
                    Result = "Success"
                }
            };
        }


        [HttpGet]
        [OutputCache(Duration = 0)]
        public ActionResult GalleryImageViewer(int id)
        {
            using (var proxy = Proxies.DataAccessProxyInstance)
            {
                var galleryImage = proxy.GetGalleryImage(id);
                var viewModel = new GalleryImageView_ViewModel()
                {
                    GalleryImage = galleryImage
                };
                return PartialView("~/Views/Galleries/_GalleryImageView.cshtml", viewModel);
            }
        }

        [HttpGet]
        [OutputCache(Duration = 0)]
        public ActionResult GalleryImageContentView(int id)
        {
            using (var proxy = Proxies.DataAccessProxyInstance)
            {
                var galleryImage = proxy.GetGalleryImage(id);
                var viewModel = new GalleryImageView_ViewModel()
                {
                    GalleryImage = galleryImage
                };
                return PartialView("~/Views/Galleries/_GalleryImageViewContent.cshtml", viewModel);
            }
        }

        [HttpGet]
        [OutputCache(Duration = 0)]
        public ActionResult GetImageComments(int id)
        {
            string partialContent = string.Empty;
            using (var proxy = Proxies.DataAccessProxyInstance)
            {
                var galleryImage = proxy.GetGalleryImage(id);

                return PartialView("~/Views/Galleries/_GalleryImageComments.cshtml", galleryImage);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public JsonResult DeleteGalleryImage(int imageId)
        {
            using (var proxy = Proxies.DataAccessProxyInstance)
            {
                proxy.DeleteGalleryImage(imageId);
            }

            return new JsonResult()
            {
                Data = new
                {
                    Result = "Success"
                }
            };
        }

        [HttpPost]
        public JsonResult AddImageComment(int imageId, string commentBody, string authorName)
        {
            int galleryId = 0;
            using (var proxy = Proxies.DataAccessProxyInstance)
            {
                if (null != proxy.GetBannedEntryByHost(Request.UserHostAddress))
                {
                    throw new Exception("Host is banned.");
                }
                proxy.AddImageComment(imageId, commentBody, authorName);
                galleryId = proxy.GetGalleryKeyForImage(imageId);
            }

            MailHelper.SendSimpleAdminMail("New image comment:",
                String.Format("{0} ({1}) added: {2}", WebSecurity.CurrentUserName, Request.UserHostAddress + "/" + Request.UserHostName,
                    MiscHelpers.getApplicationHost(Request) 
                    + Url.Action("ViewGallery", "Galleries", new { id = galleryId, imageId = imageId })));

            return new JsonResult()
            {
                Data = new
                {
                    Result = "Success"
                }
            };
        }

        [HttpPost]
        public JsonResult DeleteImageComment(int commentId)
        {
            using (var proxy = Proxies.DataAccessProxyInstance)
            {
                proxy.DeleteImageComment(commentId);
            }

            return new JsonResult()
            {
                Data = new
                {
                    Result = "Success"
                }
            };
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public void DeleteGallery(int galleryId)
        {
            using (var proxy = Proxies.DataAccessProxyInstance)
            {
                proxy.DeleteGallery(galleryId);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult AddImagesToGallery(int galleryId)
        {
            return View(galleryId);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult UploadImagesToGallery(int galleryId, IEnumerable<HttpPostedFileBase> images)
        {
            using (var proxy = Proxies.DataAccessProxyInstance)
            {
                proxy.AddImagesToGallery(galleryId, images.ToList());
            }
            return RedirectToAction("ViewGallery", new { id = galleryId });
        }
    }
}
