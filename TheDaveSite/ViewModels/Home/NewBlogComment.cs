using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheDaveSite.ViewModels.Home
{
    public class NewBlogComment
    {
        public int PostId { get; set; }
        public int CommentId { get; set; }
        public string Content { get; set; }
        public string AuthorName { get; set; }
    }
}