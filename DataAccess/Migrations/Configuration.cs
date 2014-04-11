namespace DataAccess.Migrations
{
    using DataAccess.Models;
    using DataAccess.Utils;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using WebMatrix.WebData;

    public class Configuration : DbMigrationsConfiguration<DataAccess.Models.DaveAppContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;
            ContextKey = "DataAccess.Models.DaveAppContext";
        }
        protected override void Seed(DaveAppContext context)
        {
            // Only seed on the first run. 
            if (!context.UserProfiles.Any())
            {
                SeedMembership(context);
                SeedBlogData(context);
                SeedMessageBoardData(context);
            }

#if DEBUG


            if (!context.Galleries.Any())
            {
                context.Galleries.Add(new DataAccess.Models.Gallery()
                {
                    AuthorId = context.UserProfiles.FirstOrDefault().UserId,
                    Description = "Test Gallery, this is!",
                    CreatedDate = DateTime.Now,
                    IsPublic = true,
                    Title = "Dave's Test Gallery"
                });

                context.SaveChanges();
            }
#endif
        }

        private void SeedMembership(DaveAppContext context)
        {
            if (!WebSecurity.Initialized)
            {
                WebSecurity.InitializeDatabaseConnection(DatabaseUtils.CurrentDataDatabaseConnectionString, "UserProfile", "UserId", "UserName", autoCreateTables: true);
            }

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
                if (!WebSecurity.UserExists(adminUser.UserName))
                {
                    proxy.CreateNewUser(adminUser, DatabaseUtils.AdminPassword);
                }
                if (!WebSecurity.UserExists(testUser.UserName))
                {
                    proxy.CreateNewUser(testUser, "harryTEST");
                }
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
}
