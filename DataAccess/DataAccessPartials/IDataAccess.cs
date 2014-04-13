using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DataAccess
{
    public class GalleryProperties
    {
        public string Description { get; set; }
        public string Title { get; set; }
        public bool IsPublic { get; set; }
    }

    public interface IDataAccessProxy : IDisposable
    {
        #region Blog
        IEnumerable<BlogPost> GetBlogPostsByNumber(int startingPost, int count);
        BlogPost GetSingleBlogPost(int id);
        void AddNewBlogComment(int postId, int? commentId, string content, string authorName);
        void AddPost(BlogPost newPost);
        void EditBlogPost(BlogPost postData);
        int GetNumberOfBlogPosts();
        #endregion 

        #region Message Boards

        IEnumerable<MessageBoard> GetAllMessageBoards();
        MessageBoard GetMessageBoard(int id);
        IEnumerable<MessageBoardPost> GetMessageBoardPosts(int id, int startingPost, int count);
        int GetMessageBoardPostCount(int id);
        void AddNewMessageBoardPost(int boardId, string title, string content, string authorName);
        void AddNewMessageBoardComment(int boardId, int postId, int? commentId, string content, string authorName);

        #endregion 

        #region Galleries

        List<Gallery> GetAllGalleries();
        Gallery GetGallery(int id);
        int CreateGallery(GalleryProperties model);
        void EditGallery(int id, GalleryProperties model);
        void AddImageComment(int imageId, string commentText, string authorName);
        void DeleteImageComment(int commentId);
        void DeleteGallery(int id);
        GalleryImage GetGalleryImage(int id);
        void DeleteGalleryImage(int id);
        void AddImagesToGallery(int galleryId, List<HttpPostedFileBase> images);
        int GetGalleryKeyForImage(int imageId);
        // Accept string containing a list of pipe (|) separated indices.
        void SetGalleryOrder(int galleryId, string orderList);

        #endregion

        #region Images

        StoredImageProperties GetFullImageProperties(int imageId);
        StoredImageProperties GetViewerImageProperties(int imageId);
        StoredImageProperties GetThumbnailImageProperties(int imageId);
        StoredImageProperties GetFullImage(int imageId);
        StoredImageProperties GetViewerImage(int imageId);
        StoredImageProperties GetThumbnailImage(int imageId);

        #endregion 

        #region Banned List

        IEnumerable<BannedEntry> GetBanList();
        void AddBannedEntry(BannedEntry newEntry);
        BannedEntry GetBannedEntryByHost(string hostName);
        BannedEntry GetBannedEntryById(int id);

        #endregion 

        #region Visitor Log

        void AddVisit(string ipAddress, string host);
        IEnumerable<VisitorLogEntry> GetVists();

        #endregion
    }

    public interface IUsersProxy : IDisposable
    {
        UserProfile GetUserProfile(int userId);
        void CreateNewUser(UserProfile profile, string password);
        UserProfile GetUserProfileByEmail(string emailAddress);
    }

    public class Proxies
    {
        public static IDataAccessProxy DataAccessProxyInstance
        {
            get
            {
                try
                {
                    return new DataAccessProxy();
                }
                catch (Exception e)
                {
                    int x = 42;
                }
                return new DataAccessProxy();
            }
        }

        public static IUsersProxy UserProxyInstance
        {
            get
            {
                return new UsersProxy();
            }
        }

        public static void Initialize()
        {
            DataAccessProxy.Initialize();
        }
    }
}
