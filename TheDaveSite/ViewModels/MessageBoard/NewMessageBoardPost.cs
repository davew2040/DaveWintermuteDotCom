using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheDaveSite.ViewModels.MessageBoard
{
    public class NewMessageBoardPost
    {
        public string Title { get; set; }
        public int BoardId { get; set; }
        public string Content { get; set; }
        public string AnonymousAuthorName { get; set; }
    }
}