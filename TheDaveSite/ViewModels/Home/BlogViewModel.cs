using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheDaveSite.ViewModels.Home
{
    public class BlogViewModel
    {
        public IEnumerable<DataAccess.Models.BlogPost> RecentPosts { get; set; }
        public int PageNumber { get; set; }
        public int NumberOfPages { get; set; }
        public int PostsPerPage { get; set; }
        public int TotalPostNumber { get; set; }
    }
}