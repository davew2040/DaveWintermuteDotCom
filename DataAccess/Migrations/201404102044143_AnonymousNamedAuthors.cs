namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AnonymousNamedAuthors : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BlogComments", "AnonymousAuthorName", c => c.String());
            AddColumn("dbo.GalleryImageComments", "AnonymousAuthorName", c => c.String());
            AddColumn("dbo.MessageBoardPostComments", "AnonymousAuthorName", c => c.String());
            AddColumn("dbo.MessageBoardPosts", "AnonymousAuthorName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MessageBoardPosts", "AnonymousAuthorName");
            DropColumn("dbo.MessageBoardPostComments", "AnonymousAuthorName");
            DropColumn("dbo.GalleryImageComments", "AnonymousAuthorName");
            DropColumn("dbo.BlogComments", "AnonymousAuthorName");
        }
    }
}
