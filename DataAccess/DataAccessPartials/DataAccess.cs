using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using System.Web;
using WebMatrix.WebData;
using DataAccess.Utils;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Drawing.Imaging;
using TheDaveSite.Utils;
using System.Drawing;

namespace DataAccess
{
    public partial class DataAccessProxy : IDataAccessProxy
    {
        private const long JPEG_ENCODING_QUALITY = 70L;

        static DataAccessProxy()
        {
            Initialize();
        }

        public static void Initialize()
        {
            using (var context = new DaveAppContext())
            {
                context.Database.CreateIfNotExists();
                var poke = context.UserProfiles.FirstOrDefault();
            }
            
            var roles = (SimpleRoleProvider)System.Web.Security.Roles.Provider;
            var membership = (SimpleMembershipProvider)System.Web.Security.Membership.Provider;

            if (!roles.RoleExists("Admin"))
                roles.CreateRole("Admin");

            if (!roles.RoleExists("User"))
                roles.CreateRole("User");

            if (!roles.IsUserInRole(Utils.DatabaseUtils.AdminUserName, "Admin"))
            {
                roles.AddUsersToRoles(new string[] { Utils.DatabaseUtils.AdminUserName }, new string[] { "Admin" });
            }
        }

        public DataAccessProxy()
        {
        }

        public void Dispose()
        {
        }

        public IEnumerable<BlogPost> GetBlogPostsByNumber(int startingPost, int count)
        {
            using (var context = new DaveAppContext())
            {
                var posts = context.BlogPosts
                    .OrderByDescending(x => x.PostDate)
                    .Skip(startingPost)
                    .Take(count)
                    .Include(x => x.Author)
                    .Include(x => x.Comments)
                    .Include(x => x.Comments.Select(b => b.Author))
                    .Include(x => x.LinkedGallery.Images)
                    .ToList();

                foreach (var post in posts)
                {
                    if (post.LinkedGallery != null && !String.IsNullOrEmpty(post.LinkedGallery.ImageOrder))
                    {
                        var orderList = post.LinkedGallery.ImageOrder.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Int32.Parse(x)).ToList();
                        post.LinkedGallery.Images = (ICollection<GalleryImage>)post.LinkedGallery.Images.OrderByIndexList(orderList);
                    }
                }

                return posts;
            }
        }

        public BlogPost GetSingleBlogPost(int id)
        {
            using (var context = new DaveAppContext())
            {
                return context.BlogPosts.Where(post => post.Id == id).FirstOrDefault();
            }
        }

        public void AddPost(BlogPost newPost)
        {
            using (var context = new DaveAppContext())
            {
                context.BlogPosts.Add(newPost);
                context.SaveChanges();
            }
        }


        public void AddNewBlogComment(int postId, int? commentId, string content, string authorName)
        {
            using (var context = new DaveAppContext())
            {
                int? authorId = null;
                if (WebSecurity.IsAuthenticated)
                {
                    authorId = WebSecurity.CurrentUserId;
                }
                BlogComment newComment = new BlogComment()
                {
                    AuthorId = authorId,
                    AnonymousAuthorName = authorName,
                    RootId = postId,
                    ParentId = commentId,
                    PostDate = DateTime.Now,
                    Content = content
                };

                context.BlogPostComments.Add(newComment);
                context.SaveChanges();
            }
        }

        public int GetNumberOfBlogPosts()
        {
            using (var context = new DaveAppContext())
            {
                return context.BlogPosts.Count();
            }
        }

        public IEnumerable<MessageBoard> GetAllMessageBoards()
        {
            using (var context = new DaveAppContext())
            {
                return context.MessageBoards.ToList();
            }
        }

        public MessageBoard GetMessageBoard(int id)
        {
            MessageBoard returnBoard = null;
            using (var context = new DaveAppContext())
            {
                returnBoard = context.MessageBoards.Where(x => x.Id == id).FirstOrDefault();
            }

            return returnBoard;
        }

        public IEnumerable<MessageBoardPost> GetMessageBoardPosts(int id, int startingPost, int count)
        {
            using (var context = new DaveAppContext())
            {
                var target = context.MessageBoards.Where(x => x.Id == id).FirstOrDefault();
                return context.Entry(target)
                    .Collection(x => x.RootPosts)
                    .Query()
                    .OrderByDescending(x => x.PostDate)
                    .Skip(startingPost)
                    .Take(count)
                    .Include(x => x.Author)
                    .Include(x => x.Comments)
                    .Include(x => x.Comments.Select(b => b.Author))
                    .ToList();
            }
        }


        public int GetMessageBoardPostCount(int id)
        {
            using (var context = new DaveAppContext())
            {
                var target = context.MessageBoards.Where(x => x.Id == id).FirstOrDefault();
                return target.RootPosts.Count; 
            }
        }


        public void AddNewMessageBoardComment(int boardId, int postId, int? commentId, string content)
        {
            using (var context = new DaveAppContext())
            {
                int? authorId = null;
                if (WebSecurity.IsAuthenticated)
                {
                    authorId = WebSecurity.CurrentUserId;
                }

                MessageBoardPostComment newComment = new MessageBoardPostComment()
                {
                    AuthorId = authorId,
                    ParentPostId = postId,
                    ParentCommentId = commentId,
                    PostDate = DateTime.Now,
                    Content = content
                };

                var board = context.MessageBoards.Where(x => x.Id == boardId).FirstOrDefault();
                var post = board.RootPosts.Where(x => x.Id == postId).FirstOrDefault();
                post.Comments.Add(newComment);

                context.SaveChanges();
            }
        }


        public void AddNewMessageBoardPost(int boardId, string title, string content)
        {
            using (var context = new DaveAppContext())
            {
                int? authorId = null;
                if (WebSecurity.IsAuthenticated)
                {
                    authorId = WebSecurity.CurrentUserId;
                }

                var newPost = new MessageBoardPost()
                {
                    AuthorId = authorId,
                    Title = title,
                    PostDate = DateTime.Now,
                    Content = content
                };

                var board = context.MessageBoards.Where(x => x.Id == boardId).FirstOrDefault();
                board.RootPosts.Add(newPost);

                context.SaveChanges();
            }
        }


        public List<Gallery> GetAllGalleries()
        {
            using (var context = new DaveAppContext())
            {
                var galleries = context.Galleries.Include(x => x.Author).ToList();

                return galleries;
            }
        }

        public Gallery GetGallery(int id)
        {
            using (var context = new DaveAppContext())
            {
                var targetGallery = context.Galleries.Where(x => x.Id == id)
                    .Include(x => x.Author)
                    .Include(x => x.Images.Select(a => a.Image.ImageThumbnail))
                    .FirstOrDefault();

                if (targetGallery == null)
                {
                    throw new Exception("Gallery [" + id + "] not found.");
                }

                if (!String.IsNullOrEmpty(targetGallery.ImageOrder))
                {
                    var orderList = targetGallery.ImageOrder.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Int32.Parse(x)).ToList();
                    targetGallery.Images = (ICollection<GalleryImage>)targetGallery.Images.OrderByIndexList(orderList);
                }

                return targetGallery;
            }
        }

        public void AddImagesToGallery(int galleryId, List<HttpPostedFileBase> images)
        {
            using (var context = new DaveAppContext())
            {
                var targetGallery = context.Galleries.Where(x => x.Id == galleryId)
                    .Include(x => x.Images)
                    .FirstOrDefault();

                if (targetGallery == null)
                {
                    throw new Exception("Gallery [" + galleryId + "] not found.");
                }

                foreach (var newImage in images)
                {
                    if (!validateImage(newImage))
                    {
                        continue;
                    }

                    var decodedImage = Image.FromStream(newImage.InputStream);
                    var storedImage = buildStoredImageData(decodedImage);

                    context.StoredImages.Add(storedImage);
                    context.SaveChanges();

                    decodedImage.Dispose();

                    targetGallery.Images.Add(
                        new GalleryImage()
                        {
                            Description = String.Empty,
                            ImageId = storedImage.Id
                        });

                    context.SaveChanges();
                }
            }
        }

        private bool validateImage(HttpPostedFileBase postedImage)
        {
            if (postedImage == null || postedImage.ContentLength == 0)
            {
                return false;
            }

            if (postedImage.ContentType.ToLower() != "image/bmp"
                && postedImage.ContentType.ToLower() != "image/gif"
                && postedImage.ContentType.ToLower() != "image/jpeg"
                && postedImage.ContentType.ToLower() != "image/png")
            {
                return false;
            }

            return true;
        }

        private StoredImage buildStoredImageData(Image someImage)
        {
            var fullSizeImage = ImageUtilities.ScaleFullsizeImage(someImage);
            var viewerImage = ImageUtilities.ScaleViewerImage(someImage);
            var thumbImage = ImageUtilities.ScaleThumbnailImage(someImage);

            StoredImage storedImage = new StoredImage()
            {
                FullImage = new StoredImageProperties(){
                    ActualWidth = fullSizeImage.Width,
                    ActualHeight = fullSizeImage.Height,
                    DataFormat = ImageUtilities.DefaultImageContentType,
                    ImageData = new StoredImageData()
                    {
                        ByteData = getImageByteData(fullSizeImage)
                    }
                },
                ViewerImage = new StoredImageProperties()
                {
                    ActualWidth = viewerImage.Width,
                    ActualHeight = viewerImage.Height,
                    DataFormat = ImageUtilities.DefaultImageContentType,
                    ImageData = new StoredImageData()
                    {
                        ByteData = getImageByteData(viewerImage)
                    }
                },
                ImageThumbnail = new StoredImageProperties()
                {
                    ActualWidth = thumbImage.Width,
                    ActualHeight = thumbImage.Height,
                    DataFormat = ImageUtilities.DefaultImageContentType,
                    ImageData = new StoredImageData()
                    {
                        ByteData = getImageByteData(thumbImage)
                    }
                },
            };

            fullSizeImage.Dispose();
            viewerImage.Dispose();
            thumbImage.Dispose();

            return storedImage;
        }

        private byte[] getImageByteData(Image someImage)
        {
            MemoryStream byteStream = new MemoryStream();
            ImageCodecInfo encoder = GetEncoder(ImageUtilities.DefaultFormat);

            System.Drawing.Imaging.Encoder myEncoder =
                    System.Drawing.Imaging.Encoder.Quality;

            EncoderParameters myEncoderParameters = new EncoderParameters(1);

            EncoderParameter defaultQualityEncoderParam = new EncoderParameter(myEncoder, JPEG_ENCODING_QUALITY);
            myEncoderParameters.Param[0] = defaultQualityEncoderParam;

            someImage.Save(byteStream, encoder, myEncoderParameters);

            return byteStream.ToArray();
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            return codecs.Where(x => x.FormatID == format.Guid).FirstOrDefault();
        }

        public StoredImageProperties GetFullImageProperties(int imageId)
        {
            using (var context = new DaveAppContext())
            {
                return context.StoredImages.Where(x => x.Id == imageId)
                    .Include(x => x.FullImage).FirstOrDefault()
                    .FullImage;
            }
        }

        public StoredImageProperties GetViewerImageProperties(int imageId)
        {
            using (var context = new DaveAppContext())
            {
                return context.StoredImages.Where(x => x.Id == imageId)
                    .Include(x => x.ViewerImage).FirstOrDefault()
                    .ViewerImage;
            }
        }

        public StoredImageProperties GetThumbnailImageProperties(int imageId)
        {
            using (var context = new DaveAppContext())
            {
                return context.StoredImages.Where(x => x.Id == imageId)
                    .Include(x => x.ImageThumbnail).FirstOrDefault()
                    .ImageThumbnail;
            }
        }

        public StoredImageProperties GetFullImage(int imageId)
        {
            using (var context = new DaveAppContext())
            {
                return context.StoredImages.Where(x => x.Id == imageId)
                    .Include(x => x.FullImage.ImageData).FirstOrDefault()
                    .FullImage;
            }
        }

        public StoredImageProperties GetViewerImage(int imageId)
        {
            using (var context = new DaveAppContext())
            {
                return context.StoredImages.Where(x => x.Id == imageId)
                    .Include(x => x.ViewerImage.ImageData).FirstOrDefault()
                    .ViewerImage;
            }
        }

        public StoredImageProperties GetThumbnailImage(int imageId)
        {
            using (var context = new DaveAppContext())
            {
                var storedImage = context.StoredImages.Where(x => x.Id == imageId)
                    .Include(x => x.ImageThumbnail.ImageData).FirstOrDefault();
                return storedImage.ImageThumbnail;
            }
        }

        public void DeleteGallery(int id)
        {
            using (var context = new DaveAppContext())
            {
                try
                {

                    var targetGallery = context.Galleries.Where(x => x.Id == id).Include(x => x.Images).FirstOrDefault();

                    foreach (var image in targetGallery.Images.ToList())
                    {
                        deleteGalleryImage(image.Id, context);
                    }

                    var linkedPosts = context.BlogPosts.Where(x => x.LinkedGalleryId == id);
                    foreach (var post in linkedPosts)
                    {
                        post.LinkedGalleryId = null;
                    }

                    context.Galleries.Remove(targetGallery);

                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    var x = 42;
                }
            }
        }

        public GalleryImage GetGalleryImage(int id)
        {
            using (var context = new DaveAppContext())
            {
                return context.GalleryImages.Where(x => x.Id == id)
                    .Include(x => x.Comments.Select(a => a.Author))
                    // include image properties but not actual image byte data
                    .Include(x => x.Image.FullImage)
                    .Include(x => x.Image.ImageThumbnail)
                    .Include(x => x.Image.ViewerImage)
                    .FirstOrDefault();
            }
        }

        public void DeleteGalleryImage(int id)
        {
            using (var context = new DaveAppContext())
            {
                deleteGalleryImage(id, context);
            }
        }

        public void SetGalleryOrder(int galleryId, string orderList)
        {
            using (var context = new DaveAppContext())
            {
                var targetGallery = context.Galleries.Where(x => x.Id == galleryId).FirstOrDefault();
                targetGallery.ImageOrder = orderList;

                context.SaveChanges();
            }
        }

        private void deleteGalleryImage(int id, DaveAppContext context)
        {
            var containingGallery = context.Galleries.Where(a => a.Images.Contains(a.Images.Where(b => b.Id == id).FirstOrDefault())).Include(a => a.Images).FirstOrDefault();
            var targetImage = containingGallery.Images.Where(x => x.Id == id).FirstOrDefault();
            context.Entry(targetImage).Reference(x => x.Image).Load();
            var storedImage = context.StoredImages.Where(x => x.Id == targetImage.Image.Id).FirstOrDefault();

            containingGallery.Images.Remove(targetImage);
            context.GalleryImages.Remove(targetImage);
            context.StoredImages.Remove(storedImage);

            context.SaveChanges();
        }

        public void EditGallery(int id, GalleryProperties model)
        {
            using (var context = new DaveAppContext())
            {
                var targetGallery = context.Galleries.Where(x => x.Id == id)
                    .FirstOrDefault();

                if (targetGallery == null)
                {
                    throw new Exception("Gallery [" + id + "] not found.");
                }

                targetGallery.Title = model.Title;
                targetGallery.Description = model.Description;
                targetGallery.IsPublic = model.IsPublic;

                context.SaveChanges();
            }
        }


        public void AddImageComment(int imageId, string commentText, string authorName)
        {
            using (var context = new DaveAppContext())
            {
                int? userId = null;
                if (WebSecurity.IsAuthenticated)
                {
                    userId = WebSecurity.CurrentUserId;
                }
                var targetGallery = context.GalleryImages.Where(x => x.Id == imageId).Include("Comments").FirstOrDefault();
                targetGallery.Comments.Add(new GalleryImageComment()
                {
                    AnonymousAuthorName = authorName,
                    AuthorId = userId, 
                    CommentBody = commentText,
                    CommentDate = DateTime.Now,
                    ParentImageId = imageId
                });
                context.SaveChanges();
            }
        }

        public void DeleteImageComment(int commentId)
        {
            using (var context = new DaveAppContext())
            {
                var targetImage = context.GalleryImages.Where(x => x.Comments.Contains(x.Comments.Where(a => a.Id == commentId).FirstOrDefault())).FirstOrDefault();
                var targetComment = targetImage.Comments.Where(x => x.Id == commentId).FirstOrDefault();

                targetImage.Comments.Remove(targetComment);

                context.GalleryImageComments.Remove(targetComment);

                context.SaveChanges();
            }
        }


        public int CreateGallery(GalleryProperties model)
        {
            using (var context = new DaveAppContext())
            {
                Gallery newGallery = new Gallery()
                {
                    AuthorId = WebSecurity.CurrentUserId,
                    CreatedDate = DateTime.Now,
                    Description = model.Description,
                    IsPublic = model.IsPublic,
                    Title = model.Title
                };

                context.Galleries.Add(newGallery);

                context.SaveChanges();

                return newGallery.Id;
            }
        }

        public void EditBlogPost(BlogPost postData)
        {
            using (var context = new DaveAppContext())
            {
                var targetPost = context.BlogPosts.Where(x => x.Id == postData.Id).FirstOrDefault();

                targetPost.Content = postData.Content;
                targetPost.ImageLinks = postData.ImageLinks;
                targetPost.Title = postData.Title;

                if (postData.LinkedGalleryId == -1)
                {
                    targetPost.LinkedGalleryId = null;
                }
                else
                {
                    targetPost.LinkedGalleryId = postData.LinkedGalleryId;
                }

                context.SaveChanges();
            }
        }


        public int GetGalleryKeyForImage(int imageId)
        {
            using (var context = new DaveAppContext())
            {
                var searchImage = context.GalleryImages.Where(a => a.Id == imageId).FirstOrDefault();
                var galleries = context.Galleries.Include(x => x.Images).ToList();
                var gallery = galleries.Where(x => x.Images.Contains(searchImage)).FirstOrDefault();

                return gallery.Id;
            }
        }
    }

    public class UsersProxy : IUsersProxy
    {
        public Models.UserProfile GetUserProfile(int userId)
        {
            using (var context = new DaveAppContext())
            {
                return context.UserProfiles.Where(x => x.UserId == userId).FirstOrDefault();
            }
        }

        public void Dispose()
        {
            // Leave empty for now.
        }


        public void CreateNewUser(UserProfile profile, string password)
        {
            WebSecurity.CreateUserAndAccount(profile.UserName, password,
                new
                {
                    FirstName = profile.FirstName,
                    MiddleName = profile.MiddleName,
                    LastName = profile.LastName,
                    Email = profile.Email
                }, false);
        }


        public UserProfile GetUserProfileByEmail(string emailAddress)
        {
            using (var context = new DaveAppContext())
            {
                return context.UserProfiles.Where(x => x.Email.ToLower() == emailAddress.ToLower()).FirstOrDefault();
            }
        }
    }
}
