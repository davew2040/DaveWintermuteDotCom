using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class ResourceLike
    {
        public enum ResourceType
        {
            BlogPost = 1,
            BlogComment,
            GalleryImage,
            MessageBoardPost,
            MessageBoardPostComment
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Author")]
        public int AuthorId { get; set; }
        public UserProfile Author { get; set; }

        public DateTime DateLiked { get; set; }

        [Required]
        public int? ResourceId { get; set; }

        [Required]
        public int? ResourceTypeId { get; set; }
    }
}
