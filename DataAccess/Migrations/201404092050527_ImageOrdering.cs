namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImageOrdering : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BannedEntries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Label = c.String(),
                        IpAddress = c.String(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Galleries", "ImageOrder", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Galleries", "ImageOrder");
            DropTable("dbo.BannedEntries");
        }
    }
}
