namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResourceLikes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ResourceLikes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AuthorId = c.Int(nullable: false),
                        DateLiked = c.DateTime(nullable: false),
                        ResourceId = c.Int(nullable: false),
                        ResourceTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfile", t => t.AuthorId)
                .Index(t => t.AuthorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ResourceLikes", "AuthorId", "dbo.UserProfile");
            DropIndex("dbo.ResourceLikes", new[] { "AuthorId" });
            DropTable("dbo.ResourceLikes");
        }
    }
}
