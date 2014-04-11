using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheDaveSite.ViewModels.MessageBoard
{
    public class NewMessageBoardComment
    {
        public int BoardId { get; set; }
        public int PostId { get; set; }
        public int CommentId { get; set; }
        public string Content { get; set; }
    }
}