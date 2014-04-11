namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImageUpdates : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StoredImageProperties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ActualWidth = c.Int(nullable: false),
                        ActualHeight = c.Int(nullable: false),
                        DataFormat = c.String(),
                        ImageData_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StoredImageDatas", t => t.ImageData_Id)
                .Index(t => t.ImageData_Id);
            
            CreateTable(
                "dbo.StoredImageDatas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ByteData = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.StoredImages", "FullImage_Id", c => c.Int());
            AddColumn("dbo.StoredImages", "ImageThumbnail_Id", c => c.Int());
            AddColumn("dbo.StoredImages", "ViewerImage_Id", c => c.Int());
            CreateIndex("dbo.StoredImages", "FullImage_Id");
            CreateIndex("dbo.StoredImages", "ImageThumbnail_Id");
            CreateIndex("dbo.StoredImages", "ViewerImage_Id");
            AddForeignKey("dbo.StoredImages", "FullImage_Id", "dbo.StoredImageProperties", "Id");
            AddForeignKey("dbo.StoredImages", "ImageThumbnail_Id", "dbo.StoredImageProperties", "Id");
            AddForeignKey("dbo.StoredImages", "ViewerImage_Id", "dbo.StoredImageProperties", "Id");
            DropColumn("dbo.StoredImages", "ImageThumbnail");
            DropColumn("dbo.StoredImages", "FullImage");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StoredImages", "FullImage", c => c.Binary());
            AddColumn("dbo.StoredImages", "ImageThumbnail", c => c.Binary());
            DropForeignKey("dbo.StoredImages", "ViewerImage_Id", "dbo.StoredImageProperties");
            DropForeignKey("dbo.StoredImages", "ImageThumbnail_Id", "dbo.StoredImageProperties");
            DropForeignKey("dbo.StoredImages", "FullImage_Id", "dbo.StoredImageProperties");
            DropForeignKey("dbo.StoredImageProperties", "ImageData_Id", "dbo.StoredImageDatas");
            DropIndex("dbo.StoredImages", new[] { "ViewerImage_Id" });
            DropIndex("dbo.StoredImages", new[] { "ImageThumbnail_Id" });
            DropIndex("dbo.StoredImages", new[] { "FullImage_Id" });
            DropIndex("dbo.StoredImageProperties", new[] { "ImageData_Id" });
            DropColumn("dbo.StoredImages", "ViewerImage_Id");
            DropColumn("dbo.StoredImages", "ImageThumbnail_Id");
            DropColumn("dbo.StoredImages", "FullImage_Id");
            DropTable("dbo.StoredImageDatas");
            DropTable("dbo.StoredImageProperties");
        }
    }
}
