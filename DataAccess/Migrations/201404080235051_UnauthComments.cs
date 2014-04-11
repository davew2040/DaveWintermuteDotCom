namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UnauthComments : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BlogComments", "AuthorId", "dbo.UserProfile");
            DropForeignKey("dbo.MessageBoardPosts", "AuthorId", "dbo.UserProfile");
            DropForeignKey("dbo.MessageBoardPostComments", "AuthorId", "dbo.UserProfile");
            DropIndex("dbo.BlogComments", new[] { "AuthorId" });
            DropIndex("dbo.MessageBoardPosts", new[] { "AuthorId" });
            DropIndex("dbo.MessageBoardPostComments", new[] { "AuthorId" });
            AlterColumn("dbo.BlogComments", "AuthorId", c => c.Int());
            AlterColumn("dbo.MessageBoardPosts", "AuthorId", c => c.Int());
            AlterColumn("dbo.MessageBoardPostComments", "AuthorId", c => c.Int());
            CreateIndex("dbo.BlogComments", "AuthorId");
            CreateIndex("dbo.MessageBoardPosts", "AuthorId");
            CreateIndex("dbo.MessageBoardPostComments", "AuthorId");
            AddForeignKey("dbo.BlogComments", "AuthorId", "dbo.UserProfile", "UserId");
            AddForeignKey("dbo.MessageBoardPosts", "AuthorId", "dbo.UserProfile", "UserId");
            AddForeignKey("dbo.MessageBoardPostComments", "AuthorId", "dbo.UserProfile", "UserId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MessageBoardPostComments", "AuthorId", "dbo.UserProfile");
            DropForeignKey("dbo.MessageBoardPosts", "AuthorId", "dbo.UserProfile");
            DropForeignKey("dbo.BlogComments", "AuthorId", "dbo.UserProfile");
            DropIndex("dbo.MessageBoardPostComments", new[] { "AuthorId" });
            DropIndex("dbo.MessageBoardPosts", new[] { "AuthorId" });
            DropIndex("dbo.BlogComments", new[] { "AuthorId" });
            AlterColumn("dbo.MessageBoardPostComments", "AuthorId", c => c.Int(nullable: false));
            AlterColumn("dbo.MessageBoardPosts", "AuthorId", c => c.Int(nullable: false));
            AlterColumn("dbo.BlogComments", "AuthorId", c => c.Int(nullable: false));
            CreateIndex("dbo.MessageBoardPostComments", "AuthorId");
            CreateIndex("dbo.MessageBoardPosts", "AuthorId");
            CreateIndex("dbo.BlogComments", "AuthorId");
            AddForeignKey("dbo.MessageBoardPostComments", "AuthorId", "dbo.UserProfile", "UserId");
            AddForeignKey("dbo.MessageBoardPosts", "AuthorId", "dbo.UserProfile", "UserId");
            AddForeignKey("dbo.BlogComments", "AuthorId", "dbo.UserProfile", "UserId");
        }
    }
}
