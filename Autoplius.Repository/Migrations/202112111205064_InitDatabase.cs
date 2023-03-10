namespace Autoplius.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitDatabase : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Searches", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.SearchesItems", "Searches_Id", c => c.Int());
            CreateIndex("dbo.SearchesItems", "Searches_Id");
            AddForeignKey("dbo.SearchesItems", "Searches_Id", "dbo.Searches", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SearchesItems", "Searches_Id", "dbo.Searches");
            DropIndex("dbo.SearchesItems", new[] { "Searches_Id" });
            DropColumn("dbo.SearchesItems", "Searches_Id");
            DropColumn("dbo.Searches", "Active");
        }
    }
}
