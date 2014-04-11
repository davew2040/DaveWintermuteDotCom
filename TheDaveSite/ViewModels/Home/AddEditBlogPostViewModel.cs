using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TheDaveSite.ViewModels.Home
{
    [Bind(Exclude = "PreviewBlogPost,Galleries")]
    public class AddEditBlogPostViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public string ImageLinks { get; set; }

        public BlogPost PreviewBlogPost { get; set; }

        public List<Gallery> Galleries { get; set; }
        public int? LinkedGalleryId { get; set; }
    }
}