namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GalleryImageComments : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GalleryImageComments", "AuthorId", "dbo.UserProfile");
            DropIndex("dbo.GalleryImageComments", new[] { "AuthorId" });
            AlterColumn("dbo.GalleryImageComments", "AuthorId", c => c.Int());
            CreateIndex("dbo.GalleryImageComments", "AuthorId");
            AddForeignKey("dbo.GalleryImageComments", "AuthorId", "dbo.UserProfile", "UserId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GalleryImageComments", "AuthorId", "dbo.UserProfile");
            DropIndex("dbo.GalleryImageComments", new[] { "AuthorId" });
            AlterColumn("dbo.GalleryImageComments", "AuthorId", c => c.Int(nullable: false));
            CreateIndex("dbo.GalleryImageComments", "AuthorId");
            AddForeignKey("dbo.GalleryImageComments", "AuthorId", "dbo.UserProfile", "UserId");
        }
    }
}
