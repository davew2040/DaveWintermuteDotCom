using DataAccess;
using System.Web.Mvc;
using System.Collections;
using System.Linq;
using TheDaveSite.ViewModels.MessageBoard;
using System;
using TheDaveSite.Utils;
using WebMatrix.WebData;

namespace TheDaveSite.Controllers
{
    public class MessageBoardController : Controller
    {
        public const int POSTS_PER_PAGE = 10;

        public ActionResult Index()
        {
            MessageBoardIndexViewModel viewModel = new MessageBoardIndexViewModel();
            using (var proxy = Proxies.DataAccessProxyInstance)
            {
                viewModel.MessageBoards = proxy.GetAllMessageBoards();
            }

            return View(viewModel);
        }

        public ActionResult MessageBoard(int Id)
        {
            MessageBoardViewModel viewModel = new MessageBoardViewModel();
            using (var proxy = Proxies.DataAccessProxyInstance)
            {
                viewModel.PostCount = proxy.GetMessageBoardPostCount(Id);

                var board = proxy.GetMessageBoard(Id);
                viewModel.CurrentMessageBoard = board;
            }

            return View(viewModel);
        }

        [OutputCache(Duration=0)]
        public ActionResult MessageBoardPosts(int id, int pageNumber = 0)
        {
            if (pageNumber < 0)
            {
                pageNumber = 0;
            }

            MessageBoardPostsViewModel viewModel = new MessageBoardPostsViewModel();
            using (var proxy = Proxies.DataAccessProxyInstance)
            {
                var board = proxy.GetMessageBoard(id);
                viewModel.CurrentPosts = proxy.GetMessageBoardPosts(id, pageNumber * POSTS_PER_PAGE, POSTS_PER_PAGE);
                viewModel.PageNumber = pageNumber;
                viewModel.StartingPostNumber = pageNumber * POSTS_PER_PAGE + 1;
                viewModel.EndingPostNumber = pageNumber * POSTS_PER_PAGE + viewModel.CurrentPosts.Count();
            }

            return PartialView("_MessageBoardPosts", viewModel);
        }

        [ValidateInput(false)]
        [HttpPost]
        public JsonResult AddNewMessageBoardComment(int boardId, int postId, int? commentId, string content)
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

                    proxy.AddNewMessageBoardComment(boardId, postId, commentId, content);

                    MailHelper.SendSimpleAdminMail("New messageboard comment:",
                        String.Format("{0} added a new messageboard comment: {1}", WebSecurity.CurrentUserName,
                            MiscHelpers.getApplicationHost(Request)
                                + Url.Action("MessageBoard", "MessageBoard", new { id = boardId })));
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
                Data = new
                {
                    Status = "Successful"
                }
            };
        }

        [ValidateInput(false)]
        [HttpPost]
        public JsonResult AddNewMessageBoardPost(NewMessageBoardPost newPost)
        {
            using (var proxy = Proxies.DataAccessProxyInstance)
            {
                try
                {
                    if (null != proxy.GetBannedEntryByHost(Request.UserHostAddress))
                    {
                        throw new Exception("Host is banned.");
                    }

                    proxy.AddNewMessageBoardPost(newPost.BoardId, newPost.Title, newPost.Content);
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
                Data = new
                {
                    Status = "Successful"
                }
            };
        }
    }
}
