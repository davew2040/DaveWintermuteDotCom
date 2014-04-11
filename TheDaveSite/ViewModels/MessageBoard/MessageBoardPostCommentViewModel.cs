using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheDaveSite.ViewModels.MessageBoard
{
    public class MessageBoardPostCommentViewModel
    {
        public MessageBoardPostComment Comment { get; set; }
        public ICollection<MessageBoardPostComment> AllComments { get; set; }
        public int Level { get; set; }
    }
}