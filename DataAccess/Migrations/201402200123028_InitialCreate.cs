namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BlogComments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RootId = c.Int(nullable: false),
                        ParentId = c.Int(),
                        Content = c.String(nullable: false),
                        PostDate = c.DateTime(nullable: false),
                        AuthorId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BlogPosts", t => t.RootId)
                .ForeignKey("dbo.UserProfile", t => t.AuthorId)
                .ForeignKey("dbo.BlogComments", t => t.ParentId)
                .Index(t => t.RootId)
                .Index(t => t.AuthorId)
                .Index(t => t.ParentId);
            
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        MiddleName = c.String(),
                        Email = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.BlogPosts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Content = c.String(nullable: false),
                        PostDate = c.DateTime(nullable: false),
                        AuthorId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfile", t => t.AuthorId)
                .Index(t => t.AuthorId);
            
            CreateTable(
                "dbo.MessageBoards",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false, maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MessageBoardPosts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        ParentBoardId = c.Int(nullable: false),
                        Content = c.String(nullable: false),
                        PostDate = c.DateTime(nullable: false),
                        AuthorId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfile", t => t.AuthorId)
                .ForeignKey("dbo.MessageBoards", t => t.ParentBoardId)
                .Index(t => t.AuthorId)
                .Index(t => t.ParentBoardId);
            
            CreateTable(
                "dbo.MessageBoardPostComments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParentPostId = c.Int(nullable: false),
                        ParentCommentId = c.Int(),
                        Content = c.String(nullable: false),
                        PostDate = c.DateTime(nullable: false),
                        AuthorId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfile", t => t.AuthorId)
                .ForeignKey("dbo.MessageBoardPostComments", t => t.ParentCommentId)
                .ForeignKey("dbo.MessageBoardPosts", t => t.ParentPostId)
                .Index(t => t.AuthorId)
                .Index(t => t.ParentCommentId)
                .Index(t => t.ParentPostId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MessageBoardPosts", "ParentBoardId", "dbo.MessageBoards");
            DropForeignKey("dbo.MessageBoardPostComments", "ParentPostId", "dbo.MessageBoardPosts");
            DropForeignKey("dbo.MessageBoardPostComments", "ParentCommentId", "dbo.MessageBoardPostComments");
            DropForeignKey("dbo.MessageBoardPostComments", "AuthorId", "dbo.UserProfile");
            DropForeignKey("dbo.MessageBoardPosts", "AuthorId", "dbo.UserProfile");
            DropForeignKey("dbo.BlogComments", "ParentId", "dbo.BlogComments");
            DropForeignKey("dbo.BlogComments", "AuthorId", "dbo.UserProfile");
            DropForeignKey("dbo.BlogComments", "RootId", "dbo.BlogPosts");
            DropForeignKey("dbo.BlogPosts", "AuthorId", "dbo.UserProfile");
            DropIndex("dbo.MessageBoardPosts", new[] { "ParentBoardId" });
            DropIndex("dbo.MessageBoardPostComments", new[] { "ParentPostId" });
            DropIndex("dbo.MessageBoardPostComments", new[] { "ParentCommentId" });
            DropIndex("dbo.MessageBoardPostComments", new[] { "AuthorId" });
            DropIndex("dbo.MessageBoardPosts", new[] { "AuthorId" });
            DropIndex("dbo.BlogComments", new[] { "ParentId" });
            DropIndex("dbo.BlogComments", new[] { "AuthorId" });
            DropIndex("dbo.BlogComments", new[] { "RootId" });
            DropIndex("dbo.BlogPosts", new[] { "AuthorId" });
            DropTable("dbo.MessageBoardPostComments");
            DropTable("dbo.MessageBoardPosts");
            DropTable("dbo.MessageBoards");
            DropTable("dbo.BlogPosts");
            DropTable("dbo.UserProfile");
            DropTable("dbo.BlogComments");
        }
    }
}
