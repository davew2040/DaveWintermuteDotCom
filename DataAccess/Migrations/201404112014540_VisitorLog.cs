namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VisitorLog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VisitorLogEntries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IpAddress = c.String(nullable: false),
                        HostName = c.String(),
                        VisitTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.VisitorLogEntries");
        }
    }
}
