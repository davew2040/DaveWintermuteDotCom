namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImageLinks : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BlogPosts", "ImageLinks", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BlogPosts", "ImageLinks");
        }
    }
}
