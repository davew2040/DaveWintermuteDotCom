using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class Gallery
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Author")]
        public int AuthorId { get; set; }
        public UserProfile Author { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public bool IsPublic { get; set; }

        // Contains a list of pipe (|) separated indicies. 
        public string ImageOrder { get; set; }

        [InverseProperty("RootGallery")]
        public virtual ICollection<GalleryImage> Images { get; set; }

        public Gallery()
        {
            IsPublic = false;
            CreatedDate = DateTime.Now;
        }
    }

    public class GalleryImage
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [InverseProperty("ParentImage")]
        public virtual ICollection<GalleryImageComment> Comments { get; set; }

        [ForeignKey("RootGallery")]
        public int RootGalleryId { get; set; }
        public Gallery RootGallery { get; set; }

        [Required]
        [ForeignKey("Image")]
        public int ImageId { get; set; }
        public StoredImage Image { get; set; }
    }

    public class GalleryImageComment
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(2000)]
        public string CommentBody { get; set; }

        [ForeignKey("Author")]
        public int? AuthorId { get; set; }
        public UserProfile Author { get; set; }

        public string AnonymousAuthorName { get; set; }

        [Required]
        [ForeignKey("ParentImage")]
        public int ParentImageId { get; set; }
        public GalleryImage ParentImage { get; set; }

        [Required]
        public DateTime CommentDate { get; set; }
    }
}
