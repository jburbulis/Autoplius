namespace Autoplius.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SearchesItems", "Searches_Id", "dbo.Searches");
            DropIndex("dbo.SearchesItems", new[] { "Searches_Id" });
            RenameColumn(table: "dbo.SearchesItems", name: "Searches_Id", newName: "SearchesId");
            AlterColumn("dbo.SearchesItems", "SearchesId", c => c.Int(nullable: false));
            CreateIndex("dbo.SearchesItems", "SearchesId");
            AddForeignKey("dbo.SearchesItems", "SearchesId", "dbo.Searches", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SearchesItems", "SearchesId", "dbo.Searches");
            DropIndex("dbo.SearchesItems", new[] { "SearchesId" });
            AlterColumn("dbo.SearchesItems", "SearchesId", c => c.Int());
            RenameColumn(table: "dbo.SearchesItems", name: "SearchesId", newName: "Searches_Id");
            CreateIndex("dbo.SearchesItems", "Searches_Id");
            AddForeignKey("dbo.SearchesItems", "Searches_Id", "dbo.Searches", "Id");
        }
    }
}
