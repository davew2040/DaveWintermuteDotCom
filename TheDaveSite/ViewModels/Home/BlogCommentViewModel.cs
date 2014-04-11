using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheDaveSite.ViewModels.Home
{
    public class BlogCommentViewModel
    {
        public BlogComment Comment { get; set; }
        public ICollection<BlogComment> AllComments { get; set; }
        public int Level { get; set; }
    }
}