using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataAccess.Models;

namespace TheDaveSite.ViewModels.MessageBoard
{
    public class MessageBoardViewModel
    {
        public DataAccess.Models.MessageBoard CurrentMessageBoard { get; set; }
        public int PostCount { get; set; }
    }

    public class MessageBoardPostsViewModel
    {
        public IEnumerable<MessageBoardPost> CurrentPosts { get; set; }
        public int PageNumber { get; set; }
        public int StartingPostNumber { get; set; }
        public int EndingPostNumber { get; set; }
    }
}