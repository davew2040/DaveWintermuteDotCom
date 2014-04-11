using DataAccess;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using TheDaveSite.Utils;
using TheDaveSite.ViewModels.Home;
using WebMatrix.WebData;

namespace TheDaveSite.Controllers
{
    public class HomeController : Controller
    {
        public const int BLOG_ENTRIES_PER_PAGE = 5;

        public ActionResult Blog(int pageNumber = 1)
        {
            using (var proxy = Proxies.DataAccessProxyInstance)
            {
                var numberOfPosts = proxy.GetNumberOfBlogPosts();
                int maxPageNumber = (int)(Math.Ceiling((double)numberOfPosts / (double)BLOG_ENTRIES_PER_PAGE));
                if (pageNumber > maxPageNumber)
                {
                    pageNumber = maxPageNumber;
                }
                var posts = proxy.GetBlogPostsByNumber((pageNumber - 1) * BLOG_ENTRIES_PER_PAGE, BLOG_ENTRIES_PER_PAGE);

                return View(new BlogViewModel()
                {
                    RecentPosts = posts,
                    NumberOfPages = maxPageNumber,
                    PageNumber = pageNumber,
                    PostsPerPage = BLOG_ENTRIES_PER_PAGE,
                    TotalPostNumber = numberOfPosts
                });
            }
        }

        public ActionResult Resume()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        [ValidateInput(false)]
        [HttpPost]
        public JsonResult AddNewBlogComment(int postId, int? commentId, string content, string authorName)
        {
            if (commentId == 0)
            {
                commentId = null;
            }
            using (var proxy = Proxies.DataAccessProxyInstance)
            {
                try
                {
                    if (null != proxy.GetBannedEntryByHost(Request.UserHostAddress))
                    {
                        throw new Exception("Host is banned.");
                    }

                    proxy.AddNewBlogComment(postId, commentId, content, authorName);

                    MailHelper.SendSimpleAdminMail("New blog comment:",
                        String.Format("{0} added a new blog comment: {1}", WebSecurity.CurrentUserName,
                            MiscHelpers.getApplicationHost(Request)
                                + Url.Action("Blog", "Home")));

                }
                catch (Exception e)
                {
                    return new JsonResult()
                    {
                        Data = new
                        {
                            Status = "Failed",
                            ErrorDetail = e.Message
                        }
                    };
                }
            }

            return new JsonResult()
            {
                Data = new {
                    Status = "Successful"
                }
            };
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult AuthorNewBlogPost()
        {
            using (var dataProxy = Proxies.DataAccessProxyInstance)
            {
                var viewModel = new AddEditBlogPostViewModel()
                {
                    Galleries = getGalleryDropDownList(dataProxy)
                };
              
                return View(viewModel);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult EditBlogPost(int postId)
        {
            using (var dataProxy = Proxies.DataAccessProxyInstance)
            {
                var blogPost = dataProxy.GetSingleBlogPost(postId);

                var viewModel = new AddEditBlogPostViewModel()
                {
                    Id = postId,
                    Content = blogPost.Content,
                    Title = blogPost.Title,
                    ImageLinks = blogPost.ImageLinks,
                    LinkedGalleryId = blogPost.LinkedGalleryId,
                    Galleries = getGalleryDropDownList(dataProxy)
                };

                return View(viewModel);
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult EditBlogPost(AddEditBlogPostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                using (var dataProxy = Proxies.DataAccessProxyInstance)
                {
                    model.Galleries = getGalleryDropDownList(dataProxy);

                    return View(model);
                }
            }

            using (var dataProxy = Proxies.DataAccessProxyInstance)
            {
                dataProxy.EditBlogPost(new BlogPost()
                {
                    Id = model.Id,
                    Content = model.Content,
                    ImageLinks = model.ImageLinks,
                    Title = model.Title,
                    LinkedGalleryId = model.LinkedGalleryId
                });

                return RedirectToAction("Blog");
            }
        }

        private List<Gallery> getGalleryDropDownList(IDataAccessProxy proxy)
        {
            var galleries = proxy.GetAllGalleries();

            foreach (var gallery in galleries)
            {
                gallery.Title = gallery.Title + " (" + gallery.CreatedDate + ")";
            }

            galleries.Insert(0, new Gallery()
            {
                Title = "(None)",
                Id = -1
            });

            return galleries;
        }

        [ValidateInput(false)]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult AuthorNewBlogPost(AddEditBlogPostViewModel postModel)
        {
            return View(postModel);
        }

        [ValidateInput(false)]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult PreviewNewBlogPost(AddEditBlogPostViewModel postModel)
        {
            using (var dataProxy = Proxies.DataAccessProxyInstance)
            using (var usersProxy = Proxies.UserProxyInstance)
            {
                var user = usersProxy.GetUserProfile(WebSecurity.CurrentUserId);

                BlogPost post = new BlogPost()
                {
                    Author = user,
                    Title = postModel.Title,
                    Content = postModel.Content,
                    Comments = new List<BlogComment>(),
                    ImageLinks = postModel.ImageLinks
                };

                postModel.PreviewBlogPost = post;

                return View(postModel);
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        [Authorize(Roles="Admin")]
        public ActionResult PostNewBlogPost(AddEditBlogPostViewModel postModel)
        {
            try
            {
                if (ModelState.IsValid && validatePostModel(postModel))
                {
                    using (var proxy = Proxies.DataAccessProxyInstance)
                    {
                        var newPost = new BlogPost()
                        {
                            Title = postModel.Title,
                            Content = postModel.Content,
                            PostDate = DateTime.Now,
                            AuthorId = WebSecurity.CurrentUserId,
                            ImageLinks = postModel.ImageLinks
                        };

                        if (postModel.LinkedGalleryId == -1)
                        {
                            newPost.LinkedGalleryId = null;
                        }
                        else
                        {
                            newPost.LinkedGalleryId = postModel.LinkedGalleryId;
                        }

                        proxy.AddPost(newPost);
                    }
                    return RedirectToAction("Blog");
                }
                else
                {
                    return RedirectToAction("AuthorNewBlogPost", postModel);
                }
            }
            catch(Exception e)
            {
                return View(postModel);
            }
        }

        private bool validatePostModel(AddEditBlogPostViewModel model)
        {
            if (string.IsNullOrEmpty(model.ImageLinks))
            {
                return true;
            }

            var splitString = model.ImageLinks.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            var regex = new Regex(@"^http(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_]*)?$");

            foreach (var s in splitString)
            {
                if (regex.Match(s).Captures.Count == 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
