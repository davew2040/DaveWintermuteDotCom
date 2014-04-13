using DataAccess.Models;
using DataAccess.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using WebMatrix.WebData;

namespace DataAccess.Models
{
    public class DaveAppContext : DbContext
    {
        static DaveAppContext()
        {
            Database.SetInitializer<DaveAppContext>(new DaveDbInitializer());
        }

        public DaveAppContext()
            : base(DatabaseUtils.CurrentDataDatabaseConnectionString)
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }


        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<BlogComment> BlogPostComments { get; set; }
        public DbSet<MessageBoard> MessageBoards { get; set; }
        public DbSet<MessageBoardPost> MessageBoardPosts { get; set; }
        public DbSet<MessageBoardPostComment> MessageBoardPostComments { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<GalleryImage> GalleryImages { get; set; }
        public DbSet<GalleryImageComment> GalleryImageComments { get; set; }
        public DbSet<StoredImageProperties> StoredImageProperties { get; set; }
        public DbSet<StoredImageData> StoredImageData { get; set; }
        public DbSet<StoredImage> StoredImages { get; set; }
        public DbSet<ResourceLike> ResourceLikes { get; set; }
        public DbSet<BannedEntry> BannedList { get; set; }
        public DbSet<VisitorLogEntry> VisitorLog { get; set; }
    }

#if DEBUG
    public class DaveDbInitializer : MigrateDatabaseToLatestVersion<DaveAppContext, DataAccess.Migrations.Configuration>
#else 
    public class DaveDbInitializer : MigrateDatabaseToLatestVersion<DaveAppContext, DataAccess.Migrations.Configuration>
#endif
    {
        //protected override void Seed(DaveAppContext context)
        //{
        //    SeedMembership(context);
        //    SeedBlogData(context);
        //    SeedMessageBoardData(context);
        //}

        private void SeedMembership(DaveAppContext context)
        {
            var adminUser = new UserProfile()
                {
                    UserName = Utils.DatabaseUtils.AdminUserName,
                    FirstName = "Dave",
                    LastName = "Wintermute",
                    MiddleName = "W.",
                    Email = "dwinterm@gmail.com"
                };
           var testUser = new UserProfile()
                {
                    UserName = "TestUser",
                    FirstName = "Harry",
                    LastName = "Tester",
                    MiddleName = "B.",
                    Email = "testerguy@tester.com"
                };

           using (var proxy = Proxies.UserProxyInstance)
           {
               proxy.CreateNewUser(adminUser, DatabaseUtils.AdminPassword);
               proxy.CreateNewUser(testUser, "harryTEST");
           }
        }

        private void SeedBlogData(DaveAppContext context)
        {
#if DEBUG
            List<BlogPost> initialPosts = null;
            using (var userProxy = Proxies.UserProxyInstance)
            {
                initialPosts = new List<BlogPost>
                {
                    new BlogPost()
                    {
                        Title = "Second Post!!!",
                        Content = "This is Dave's Second Blog Post!",
                        PostDate = DateTime.Now,
                        AuthorId = context.UserProfiles.FirstOrDefault().UserId
                    },
                    new BlogPost()
                    {
                        Title = "First Post!!!",
                        Content = "This is Dave's First Blog Post!",
                        PostDate = DateTime.Now.AddMonths(-2),
                        AuthorId = context.UserProfiles.FirstOrDefault().UserId
                    }
                };
            }

            initialPosts.ForEach(x => context.BlogPosts.Add(x));
            context.SaveChanges();

            BlogComment firstComment = new BlogComment()
            {
                AuthorId = context.UserProfiles.ToList().ElementAt(1).UserId,
                Content = "Hey Dave, great post!!!",
                PostDate = DateTime.Now,
                ParentComment = null,
                RootId = context.BlogPosts.FirstOrDefault().Id
            };

            BlogComment responseComment = new BlogComment()
            {
                AuthorId = context.UserProfiles.ToList().ElementAt(0).UserId,
                Content = "Why thank you!!",
                PostDate = DateTime.Now,
                ParentComment = firstComment,
                RootId = context.BlogPosts.FirstOrDefault().Id
            };

            context.BlogPostComments.Add(firstComment);
            context.BlogPostComments.Add(responseComment);
            context.SaveChanges();
#endif
        }

        private void SeedMessageBoardData(DaveAppContext context)
        {
            List<MessageBoard> messageBoards = new List<MessageBoard>()
            {
                new MessageBoard()
                {
                    Name = "Feedback",
                    Description = "Any thoughts or comments you have about DaveWintermute.com!"
                },
                new MessageBoard()
                {
                    Name = "The Misc.",
                    Description = "Miscellaneous topics of random discussion-having."
                },
            };
            messageBoards.ForEach(x => context.MessageBoards.Add(x));
            context.SaveChanges();

            #if DEBUG

            List<MessageBoardPost> initialPosts = new List<MessageBoardPost>()
            {
                new MessageBoardPost()
                {
                    AuthorId = context.UserProfiles.FirstOrDefault().UserId,
                    Content = "I really think this is a cool message board!",
                    Title = "Cool!",
                    ParentBoardId = messageBoards.FirstOrDefault().Id
                },
                new MessageBoardPost()
                {
                    AuthorId = context.UserProfiles.FirstOrDefault().UserId,
                    Content = "Who all thinks the new Star Wars movies are going to be amazing???",
                    Title = "Star Wars?",
                    ParentBoardId = messageBoards.FirstOrDefault().Id
                }
            };

            var firstPost = initialPosts.ElementAt(0);

            var miscBoard = context.MessageBoards.Where(x => x.Name == "The Misc.").FirstOrDefault();

            initialPosts.ForEach(x => miscBoard.RootPosts.Add(x));

            for (int i = 1; i < 50; i++)
            {
                var newPost = new MessageBoardPost()
                {
                    AuthorId = context.UserProfiles.FirstOrDefault().UserId,
                    Content = "This is a post with really nothing in it.",
                    Title = "Trash Post #" + i,
                    PostDate = DateTime.Now.AddDays(-i),
                    ParentBoardId = messageBoards.FirstOrDefault().Id
                };

                miscBoard.RootPosts.Add(newPost);
            }

            context.SaveChanges();

            var firstComment = new MessageBoardPostComment()
            {
                AuthorId = context.UserProfiles.ToList().ElementAt(1).UserId,
                Content = "It totally is!",
                ParentPostId = initialPosts.FirstOrDefault().Id,
                ParentCommentId = null,
            };

            firstPost.Comments.Add(firstComment);
            context.SaveChanges();

            var secondComment = new MessageBoardPostComment()
            {
                AuthorId = context.UserProfiles.FirstOrDefault().UserId,
                Content = "Thank you!",
                ParentPostId = initialPosts.FirstOrDefault().Id,
                ParentCommentId = firstComment.Id
            };

            firstPost.Comments.Add(secondComment);
            context.SaveChanges();

#endif
        }
    }

    public class BlogPost
    {
        private IEnumerable<UserProfile> _likes = null;

        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public DateTime PostDate { get; set; }
        public string ImageLinks { get; set; }
        [ForeignKey("LinkedGallery")]
        public int? LinkedGalleryId { get; set; }
        public Gallery LinkedGallery { get; set; }
        [Required]
        [ForeignKey("Author")]
        public int AuthorId { get; set; }
        public UserProfile Author { get; set; }
        [InverseProperty("RootPost")]
        public virtual ICollection<BlogComment> Comments { get; set; }
        [NotMapped]
        public int Likes
        {
            get
            {
                return Likers.Count();
            }
            set { }
        }
        public IEnumerable<UserProfile> Likers
        {
            get
            {
                if (null == _likes)
                {
                    _likes = new List<UserProfile>();
                }

                return _likes;
            }
            set
            {
                _likes = value;
            }
        }
    }

    public class BlogComment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [ForeignKey("RootPost")]
        public int RootId { get; set; }
        public BlogPost RootPost { get; set; }

        [ForeignKey("ParentComment")]
        public int? ParentId { get; set; }
        public BlogComment ParentComment { get; set; }

        [InverseProperty("ParentComment")]
        public virtual ICollection<BlogComment> ResponseComments { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public DateTime PostDate { get; set; }

        public string AnonymousAuthorName { get; set; }

        [ForeignKey("Author")]
        public int? AuthorId { get; set; }
        public UserProfile Author { get; set; }

        public BlogComment()
        {
            PostDate = DateTime.Now;
        }
    }

    public class MessageBoard
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        private List<MessageBoardPost> _rootPosts = null;
        public virtual ICollection<MessageBoardPost> RootPosts
        {
            get
            {
                if (null == _rootPosts)
                {
                    _rootPosts = new List<MessageBoardPost>();
                }
                return _rootPosts;
            }
            set
            {
                _rootPosts = value.ToList();
            }
        }
    }

    public class MessageBoardPost
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [ForeignKey("ParentBoard")]
        public int ParentBoardId { get; set; }
        public virtual MessageBoard ParentBoard { get; set; }

        private List<MessageBoardPostComment> _comments = null;
        [InverseProperty("ParentPost")]
        public virtual ICollection<MessageBoardPostComment> Comments
        {
            get
            {
                if (_comments == null)
                {
                    _comments = new List<MessageBoardPostComment>();
                }
                return _comments;
            }
            set
            {
                _comments = value.ToList();
            }
        }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime PostDate { get; set; }

        public string AnonymousAuthorName { get; set; }

        [ForeignKey("Author")]
        public int? AuthorId { get; set; }
        public UserProfile Author { get; set; }

        public MessageBoardPost()
        {
            PostDate = DateTime.Now;
        }
    }

    public class MessageBoardPostComment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("ParentPost")]
        public int ParentPostId { get; set; }
        public MessageBoardPost ParentPost { get; set; }

        [ForeignKey("ParentComment")]
        public int? ParentCommentId { get; set; }
        public MessageBoardPostComment ParentComment { get; set; }

        [InverseProperty("ParentComment")]
        public virtual ICollection<MessageBoardPostComment> ResponseComments { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime PostDate { get; set; }

        public string AnonymousAuthorName { get; set; }

        [ForeignKey("Author")]
        public int? AuthorId { get; set; }
        public UserProfile Author { get; set; }

        public MessageBoardPostComment()
        {
            PostDate = DateTime.Now;
        }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [DefaultValue("")]
        public string MiddleName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public List<BlogPost> PostsLiked { get; set; }
    }
}
