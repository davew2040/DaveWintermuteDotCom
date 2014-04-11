namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Gallery : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Galleries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AuthorId = c.Int(nullable: false),
                        Title = c.String(nullable: false, maxLength: 200),
                        Description = c.String(maxLength: 2000),
                        CreatedDate = c.DateTime(nullable: false),
                        IsPublic = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfile", t => t.AuthorId)
                .Index(t => t.AuthorId);
            
            CreateTable(
                "dbo.GalleryImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(maxLength: 1000),
                        RootGalleryId = c.Int(nullable: false),
                        ImageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StoredImages", t => t.ImageId)
                .ForeignKey("dbo.Galleries", t => t.RootGalleryId)
                .Index(t => t.ImageId)
                .Index(t => t.RootGalleryId);
            
            CreateTable(
                "dbo.GalleryImageComments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CommentBody = c.String(maxLength: 2000),
                        AuthorId = c.Int(nullable: false),
                        ParentImageId = c.Int(nullable: false),
                        CommentDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfile", t => t.AuthorId)
                .ForeignKey("dbo.GalleryImages", t => t.ParentImageId)
                .Index(t => t.AuthorId)
                .Index(t => t.ParentImageId);
            
            CreateTable(
                "dbo.StoredImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ImageThumbnail = c.Binary(),
                        FullImage = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.BlogPosts", "LinkedGalleryId", c => c.Int());
            CreateIndex("dbo.BlogPosts", "LinkedGalleryId");
            AddForeignKey("dbo.BlogPosts", "LinkedGalleryId", "dbo.Galleries", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BlogPosts", "LinkedGalleryId", "dbo.Galleries");
            DropForeignKey("dbo.GalleryImages", "RootGalleryId", "dbo.Galleries");
            DropForeignKey("dbo.GalleryImages", "ImageId", "dbo.StoredImages");
            DropForeignKey("dbo.GalleryImageComments", "ParentImageId", "dbo.GalleryImages");
            DropForeignKey("dbo.GalleryImageComments", "AuthorId", "dbo.UserProfile");
            DropForeignKey("dbo.Galleries", "AuthorId", "dbo.UserProfile");
            DropIndex("dbo.BlogPosts", new[] { "LinkedGalleryId" });
            DropIndex("dbo.GalleryImages", new[] { "RootGalleryId" });
            DropIndex("dbo.GalleryImages", new[] { "ImageId" });
            DropIndex("dbo.GalleryImageComments", new[] { "ParentImageId" });
            DropIndex("dbo.GalleryImageComments", new[] { "AuthorId" });
            DropIndex("dbo.Galleries", new[] { "AuthorId" });
            DropColumn("dbo.BlogPosts", "LinkedGalleryId");
            DropTable("dbo.StoredImages");
            DropTable("dbo.GalleryImageComments");
            DropTable("dbo.GalleryImages");
            DropTable("dbo.Galleries");
        }
    }
}
