using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace TheDaveSite.Models
{
    public class GalleryAddEditModel
    {
        public int? GalleryId { get; set; }
        public string Description { get; set; }
        [Required]
        public string Title { get; set; }
        public bool IsPublic { get; set; }
    }
}
